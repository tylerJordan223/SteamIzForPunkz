using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerSpinAttack : MonoBehaviour
{
    private RaycastHit2D[] hits;
    [Header("Connected Objects")]
    [SerializeField] private Transform atrans;
    [SerializeField] private Transform spinParent;
    [SerializeField] private GameObject projectile;
    private PlayerStats pstats;
    private Transform ptrans;
    private SpriteRenderer aColor;

    //changable variables
    [Header("Base Values")]
    [SerializeField] public float base_range = 1.5f;
    [SerializeField] public float pdamage = 1f;
    [SerializeField] private LayerMask alayer;
    public float size;

    [Header("Melee Attack")]
    //base values
    private float base_maxSpeed;
    public float base_size;
    private float base_spinAcceleration;
    private bool canControl;
    private float attackRange;
    //regular values
    private float speedToAttack;
    private float maxSpeed;
    private float spinAcceleration;
    //constantly changing rotation
    private float rotationSpeed;

    //charging
    [Header("Charge Attack")]
    private bool charging;
    public float base_chargeTime = 1f;
    public float base_rechargeTime = 3f;
    public float base_chargedSize = 6f;
    [SerializeField] SpriteRenderer CannonSprite;
    [SerializeField] public Sprite baseCharge;
    [SerializeField] public Sprite charging_s;
    [SerializeField] public Sprite charged_s;
    [SerializeField] public Sprite cooldown_s;

    [Header("Special")]
    [SerializeField] public Sprite specialCharge;
    [SerializeField] public Sprite special_charging_s;
    [SerializeField] public Sprite special_charged_s;

    private void Start()
    {
        //set the attack point to the right color
        aColor = atrans.gameObject.GetComponent<SpriteRenderer>();

        //set the right base stats for melee
        base_size = 1f;
        base_maxSpeed = 15f;
        base_spinAcceleration = 0.2f;
        canControl = true;
        attackRange = 0.5f;

        //stats that change throughout
        maxSpeed = base_maxSpeed;
        spinAcceleration = base_spinAcceleration;
        speedToAttack = 12.5f;
        size = base_size;

        //base stats for charge attack
        base_chargeTime = 1f;
        base_rechargeTime = 3f;
        base_chargedSize = base_size + 1f;

        //necessary objects
        ptrans = this.gameObject.transform;
        pstats = this.gameObject.GetComponent<PlayerStats>();

        charging = false;
    }

    private void Update()
    {
        //CHARGE ATTACK
        if (Input.GetKeyDown(KeyCode.Space) && pstats.charges > 0 && spinParent.parent.GetComponent<PlayerScript>().canControl)
        {
            if (!charging)
            {
                StartCoroutine(Charge());
            }
        }

        //update weapon size
        size = base_size + pstats.dynSize;
        atrans.localScale = new Vector3(size, size, 1f);

        //attack if rotation is a certain speed
        if (rotationSpeed * Mathf.Sign(rotationSpeed) > speedToAttack)
        {
            Attack();
        }

        canControl = ptrans.gameObject.GetComponent<PlayerScript>().canControl;

        UpdateAnimation();
    }

    //for all things movement
    private void FixedUpdate()
    {

        //movement
        if(canControl)
        {
            if (Input.GetMouseButton(0) && (rotationSpeed < (base_maxSpeed + pstats.dynMaxSpeed)))
            {
                if(rotationSpeed < (base_maxSpeed + pstats.dynMaxSpeed))
                {
                    rotationSpeed += (base_spinAcceleration + (pstats.dynSpinAcceleration * 0.02f));
                }
                spinParent.Rotate(new Vector3(0f, 0f, 1f), rotationSpeed);
            }else if (Input.GetMouseButton(1) && (rotationSpeed > -(base_maxSpeed + pstats.dynMaxSpeed)))
            {
                if(rotationSpeed > -(base_maxSpeed + pstats.dynMaxSpeed))
                {
                    rotationSpeed -= (base_spinAcceleration + (pstats.dynSpinAcceleration * 0.02f));
                }
                spinParent.Rotate(new Vector3(0f, 0f, 1f), rotationSpeed);
            }else
            {
                //slows down the rotation by the same speed until its 0
                if(rotationSpeed != 0)
                {
                    rotationSpeed += 0.2f * -Mathf.Sign(rotationSpeed);
                }
                //if close to 0 then make it 0
                if(rotationSpeed < 0.2f && rotationSpeed > -0.2)
                {
                    rotationSpeed = 0;
                }

                spinParent.Rotate(new Vector3(0f, 0f, 1f), rotationSpeed);
            }
        }

        if(pstats.special == 3 && !charging)
        {
            CannonSprite.sprite = specialCharge;
        }
    }

    private IEnumerator Charge()
    {
        //begin charging
        charging = true;
        //set the color to charging
        if(pstats.special == 3)
        {
            CannonSprite.sprite = special_charging_s;
        }
        else
        {
            CannonSprite.sprite = charging_s;   
        }

        //save values to be re-applied possibly later
        float maxSpeed_holder = base_maxSpeed;
        float spinAcceleration_holder = base_spinAcceleration;
        float dynSize_holder = pstats.dynSize;

        //loop infinite size alteration until normal size or fully charged
        while(true)
        {
            if(Input.GetKey(KeyCode.Space) && spinParent.parent.GetComponent<PlayerScript>().canControl)
            {
                //break if fully charged
                if (size == (base_chargedSize + pstats.dynChargedSize))
                {
                    break;
                }
                //increase in size if held  
                pstats.dynSize += 0.2f;
                //slow down
                base_spinAcceleration *= 0.7f;
                base_maxSpeed -= 2.5f;
            }
            else
            {
                //break loop if normal size
                if(size == (base_size + dynSize_holder))
                {
                    break;
                }
                else
                {
                    //decrease if not held
                    pstats.dynSize -= 0.2f;
                    //speed up
                    base_spinAcceleration /= 0.7f;
                    base_maxSpeed += 2.5f;
                }
            }
            //wait a second between checks
            yield return new WaitForSeconds(base_chargeTime - (pstats.dynChargeTime * 0.1f));
        }
        
        //if fully charged, wait until release and attack
        if(size == (base_chargedSize + pstats.dynChargedSize))
        {
            //set the color to charged
            if(pstats.special == 3)
            {
                CannonSprite.sprite = special_charged_s;
            }
            else
            {
                CannonSprite.sprite = charged_s;
            }
            
            while(true)
            {
                if(!Input.GetKey(KeyCode.Space))
                {
                    //ATTACK!
                    RangeAttack();
                    //reset to base
                    pstats.dynSize = dynSize_holder;
                    size = base_size + pstats.dynSize;

                    //wait a second before returning controls
                    canControl = false;
                    CannonSprite.sprite = cooldown_s;
                    yield return new WaitForSeconds(base_rechargeTime + pstats.dynRechargeTime);

                    //return controls back to normal
                    base_spinAcceleration = spinAcceleration_holder;
                    base_maxSpeed = maxSpeed_holder;
                    canControl = true;

                    //break out of the inf loop
                    break;
                }
                //to make sure it doesnt infinite loop
                yield return new WaitForSeconds(0.1f);
            }
        }

        //no longer charging and color back to normal
        CannonSprite.sprite = baseCharge;
        charging = false;
    }

    private void Attack()
    {
        //cast to get all the colliders in the attack range
        hits = Physics2D.CircleCastAll(atrans.position, attackRange, transform.right, 0f, alayer);

        //check all of the hits
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.CompareTag("enemy"))
            {
                //get the enemy's health
                EnemyHealth eh = hits[i].collider.gameObject.GetComponent<EnemyHealth>();
                EnemyMovement em = hits[i].collider.gameObject.GetComponent<EnemyMovement>();

                //if the collision was an enemy then damage it
                if (eh != null && !em.flying)
                {
                    eh.Damage(pdamage * pstats.dynMeleeDamage);
                }
            }else if (hits[i].collider.CompareTag("boss"))
            {
                BossHealth bh = hits[i].collider.gameObject.GetComponent<BossHealth>();

                if(bh != null)
                {
                    bh.Damage(pdamage * pstats.dynMeleeDamage);
                }
            }
        }
    }

    private void RangeAttack()
    {
        //create a bullet and direct it 
        GameObject p = Instantiate(projectile);
        //range the damage
        p.GetComponent<ProjectileScript>().projectileDamage += pstats.dynBlastDamage;
        p.GetComponent<ProjectileScript>().ricochetCount = (int)pstats.maxRicochets;
        //move the position and rotation
        p.transform.position = ptrans.transform.position;
        p.transform.up = (atrans.position - ptrans.position).normalized;
        //remove a charge from the shot
        pstats.charges--;

        //reset if this was the final attack
        if(pstats.special == 3)
        {
            pstats.special = 0;
            CannonSprite.sprite = cooldown_s;
        }
    }

    private void UpdateAnimation()
    {
        //updating the sorting layer of the attacking arm
        if (atrans.position.y > ptrans.position.y)
        {
            spinParent.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = ptrans.Find("Sprites").GetChild(0).GetComponent<SpriteRenderer>().sortingOrder - 1;
            spinParent.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = ptrans.Find("Sprites").GetChild(0).GetComponent<SpriteRenderer>().sortingOrder - 2;
        }
        else
        {
            spinParent.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = ptrans.Find("Sprites").GetChild(0).GetComponent<SpriteRenderer>().sortingOrder + 2;
            spinParent.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = ptrans.Find("Sprites").GetChild(0).GetComponent<SpriteRenderer>().sortingOrder + 1;
        }
    }

    //DEBUG PURPOSES ONLY//
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(atrans.position, attackRange);
    }
}

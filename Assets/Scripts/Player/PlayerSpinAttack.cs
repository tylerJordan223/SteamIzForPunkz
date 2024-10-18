using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerSpinAttack : MonoBehaviour
{
    private RaycastHit2D[] hits;
    [Header("Connected Objects")]
    [SerializeField] private Transform atrans;
    [SerializeField] private Transform spinParent;
    private Transform ptrans;
    private SpriteRenderer aColor;

    //changable variables
    [Header("Changable Values")]
    [SerializeField] public Color myColor;
    [SerializeField] public float range = 1.5f;
    [SerializeField] public float pdamage = 1f;
    [SerializeField] private LayerMask alayer;

    //dynamics
    [Header("Melee Attack Dynamics")]
    private float maxSpeed;
    private float spinAcceleration;
    private float rotationSpeed;
    private float speedToAttack;
    public float dynMaxSpeed = 1f;
    public float dynSpinAcceleration = 1f;

    //charging
    [Header("Charge Attack Dynamics")]
    private bool charging;
    public float chargeTime = 1f;
    public float rechargeTime = 3f;
    public float dynSize = 1f;
    public float chargedSize = 6f;
    [SerializeField] public Color chargingColor;
    [SerializeField] public Color chargedColor;
    [SerializeField] public Color cooldownColor;

    private void Start()
    {
        //set the attack point to the right color
        aColor = atrans.gameObject.GetComponent<SpriteRenderer>();

        //set the right base stats
        maxSpeed = 15f;
        spinAcceleration = 0.2f;
        speedToAttack = 12.5f;

        ptrans = this.gameObject.transform;

        charging = false;
    }

    private void Update()
    {
        //CHARGE ATTACK
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!charging)
            {
                StartCoroutine(Charge());
            }
        }

        //update weapon size
        atrans.localScale = new Vector3(dynSize, dynSize, 1f);

        //attack if rotation is a certain speed
        if (rotationSpeed * Mathf.Sign(rotationSpeed) > speedToAttack)
        {
            Attack();
        }
    }

    //for all things movement
    private void FixedUpdate()
    {
        Debug.Log(dynSpinAcceleration);
        Debug.Log(dynMaxSpeed);

        //movement
        if (Input.GetMouseButton(0) && (rotationSpeed < (maxSpeed * dynMaxSpeed)))
        {
            if(rotationSpeed < (maxSpeed * dynMaxSpeed))
            {
                rotationSpeed += (spinAcceleration * dynSpinAcceleration);
            }
            spinParent.Rotate(new Vector3(0f, 0f, 1f), rotationSpeed);
        }else if (Input.GetMouseButton(1) && (rotationSpeed > -(maxSpeed * dynMaxSpeed)))
        {
            if(rotationSpeed > -(maxSpeed * dynMaxSpeed))
            {
                rotationSpeed -= (spinAcceleration * dynSpinAcceleration);
            }
            spinParent.Rotate(new Vector3(0f, 0f, 1f), rotationSpeed);
        }else
        {
            Debug.Log("Slowing Down!");
            //slows down the rotation by the same speed until its 0
            if(rotationSpeed != 0)
            {
                rotationSpeed += spinAcceleration * -Mathf.Sign(rotationSpeed);
            }
            //if close to 0 then make it 0
            if(rotationSpeed < spinAcceleration && rotationSpeed > -spinAcceleration)
            {
                rotationSpeed = 0;
            }

            spinParent.Rotate(new Vector3(0f, 0f, 1f), rotationSpeed);
        }
    }

    private IEnumerator Charge()
    {
        //begin charging
        charging = true;
        //set the color to charging
        aColor.color = chargingColor;

        //loop infinite size alteration until normal size or fully charged
        while(true)
        {
            if(Input.GetKey(KeyCode.Space))
            {
                //increase in size if held  
                dynSize += 1f;
                //slow down
                dynSpinAcceleration -= 0.1f;
                dynMaxSpeed -= 0.17f;

                //break if fully charged
                if(dynSize == chargedSize)
                {
                    break;
                }
            }
            else
            {
                //break loop if normal size
                if(dynSize == 1f)
                {
                    break;
                }
                else
                {
                    //decrease if not held
                    dynSize -= 1f;
                    //speed up
                    dynSpinAcceleration += 0.1f;
                    dynMaxSpeed += 0.17f;
                }
            }
            //wait a second between checks
            yield return new WaitForSeconds(chargeTime);
        }
        
        //if fully charged, wait until release and attack
        if(dynSize == chargedSize)
        {
            //set the color to charged
            aColor.color = chargedColor;
            while(true)
            {
                if(!Input.GetKey(KeyCode.Space))
                {
                    //ATTACK!
                    RangeAttack();
                    //reset to base
                    dynSize = 1;

                    //wait a second before returning controls
                    dynSpinAcceleration = 0f;
                    rotationSpeed = 0f;
                    aColor.color = cooldownColor;
                    yield return new WaitForSeconds(rechargeTime);

                    //return controls back to normal
                    dynSpinAcceleration = 1f;
                    dynMaxSpeed = 1f;
                    aColor.color = myColor;

                    //break out of the inf loop
                    break;
                }
                //to make sure it doesnt infinite loop
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        charging = false;
    }

    private void Attack()
    {
        //cast to get all the colliders in the attack range
        hits = Physics2D.CircleCastAll(atrans.position, range, transform.right, 0f, alayer);

        //check all of the hits
        for (int i = 0; i < hits.Length; i++)
        {
            //get the enemy's health
            EnemyHealth eh = hits[i].collider.gameObject.GetComponent<EnemyHealth>();
            EnemyMovement em = hits[i].collider.gameObject.GetComponent<EnemyMovement>();

            //if the collision was an enemy then damage it
            if (eh != null)
            {
                eh.Damage(pdamage);
            }
        }
    }

    private void RangeAttack()
    {

    }

    //DEBUG PURPOSES ONLY//
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(atrans.position, range);
    }
}

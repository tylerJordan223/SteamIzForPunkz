using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private RaycastHit2D[] hits;
    [SerializeField] private Transform atrans;
    private Transform ptrans;

    //changable variables
    [SerializeField] public float range = 1.5f;
    [SerializeField] public float pdamage = 1f;
    [SerializeField] private LayerMask alayer;

    //limiting the amount of attacks
    [SerializeField] public float timeBetweenAttacks;
    private float attackTimer;

    //movement
    Vector2 inputVector;
    Vector2 lastInputVector;

    private void Start()
    {
        ptrans = this.gameObject.transform;
    }

    private void Update()
    {
        //update input
        inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //update last input if actual input and not 0
        if ((inputVector.x != 0 || inputVector.y != 0))
        {
            lastInputVector = inputVector;
        }

        //update position of attack point based on vector using the playertransform.
        atrans.position = ptrans.position + new Vector3(lastInputVector.x, lastInputVector.y, 0f);

        if (Input.GetKeyDown(KeyCode.Space) && (attackTimer >= timeBetweenAttacks))
        {
            Attack();
            attackTimer = 0;
        }

        //update the time counter
        attackTimer += Time.deltaTime;
    }

    private void Attack()
    {
        //cast to get all the colliders in the attack range
        hits = Physics2D.CircleCastAll(atrans.position, range, transform.right, 0f, alayer);
        
        //check all of the hits
        for(int i = 0; i < hits.Length; i++)
        {
            //get the enemy's health
            EnemyHealth eh = hits[i].collider.gameObject.GetComponent<EnemyHealth>();
            EnemyMovement em = hits[i].collider.gameObject.GetComponent<EnemyMovement>();

            //if the collision was an enemy then damage it
            if (eh != null)
            {
                eh.Damage(pdamage);
                em.Knockback();
            }
        }
    }

    //DEBUG PURPOSES ONLY//
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(atrans.position, range);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerSpinAttack : MonoBehaviour
{
    private RaycastHit2D[] hits;
    [SerializeField] private Transform atrans;
    [SerializeField] private Transform spinParent;
    private Transform ptrans;
    private float rotationSpeed;

    //changable variables
    [SerializeField] public float range = 1.5f;
    [SerializeField] public float pdamage = 1f;
    [SerializeField] private LayerMask alayer;

    private void Start()
    {
        ptrans = this.gameObject.transform;
    }

    private void FixedUpdate()
    {
        //movement
        if (Input.GetMouseButton(0))
        {
            if(rotationSpeed < 15f)
            {
                rotationSpeed += 0.2f;
            }
            spinParent.Rotate(new Vector3(0f, 0f, 1f), rotationSpeed);
        }else if(Input.GetMouseButton(1))
        {
            if(rotationSpeed > -15f)
            {
                rotationSpeed -= 0.2f;
            }
            spinParent.Rotate(new Vector3(0f, 0f, 1f), rotationSpeed);
        }
        else
        {
            //slows down the rotation by the same speed until its 0
            if(rotationSpeed != 0)
            {
                rotationSpeed += 0.2f * -Mathf.Sign(rotationSpeed);
            }
            //if close to 0 then make it 0
            if(rotationSpeed < 0.2f && rotationSpeed > -0.2f)
            {
                rotationSpeed = 0;
            }

            spinParent.Rotate(new Vector3(0f, 0f, 1f), rotationSpeed);
        }

        //attack if rotation is a certain speed
        if(rotationSpeed * Mathf.Sign(rotationSpeed) > 12.5f)
        {
            Attack();
        }

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

    //DEBUG PURPOSES ONLY//
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(atrans.position, range);
    }
}

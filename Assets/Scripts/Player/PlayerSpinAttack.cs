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

    //changable variables
    [Header("Changable Values")]
    [SerializeField] public float range = 1.5f;
    [SerializeField] public float pdamage = 1f;
    [SerializeField] private LayerMask alayer;

    private float maxSpeed;
    private float spinAcceleration;
    private float rotationSpeed;
    private float speedToAttack;
    public float dynMaxSpeed = 1f;
    public float dynSpinAcceleration = 1f;

    private void Start()
    {
        maxSpeed = 15f;
        spinAcceleration = 0.2f;
        speedToAttack = 12.5f;

        ptrans = this.gameObject.transform;
    }

    private void FixedUpdate()
    {
        //movement
        if( !( ( rotationSpeed * Mathf.Sign(rotationSpeed) ) > (maxSpeed * dynMaxSpeed) ) )
        {
            if (Input.GetMouseButton(0))
            {
                if(rotationSpeed < (maxSpeed * dynMaxSpeed))
                {
                    rotationSpeed += (spinAcceleration * dynSpinAcceleration);
                }
                spinParent.Rotate(new Vector3(0f, 0f, 1f), rotationSpeed);
            }else if(Input.GetMouseButton(1))
            {
                if(rotationSpeed > -(maxSpeed * dynMaxSpeed))
                {
                    rotationSpeed -= (spinAcceleration * dynSpinAcceleration);
                }
                spinParent.Rotate(new Vector3(0f, 0f, 1f), rotationSpeed);
            }
        }
        else
        {
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

        Debug.Log(rotationSpeed);

        //move slower if space is held
        if(Input.GetKey(KeyCode.Space))
        {
            dynSpinAcceleration = 0.5f;
            dynMaxSpeed = 0.2f;
        }
        else
        {
            dynSpinAcceleration = 1f;
            dynMaxSpeed = 1f;
        }

        //attack if rotation is a certain speed
        if(rotationSpeed * Mathf.Sign(rotationSpeed) > speedToAttack)
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

    private void RangeAttack()
    {

    }

    //DEBUG PURPOSES ONLY//
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(atrans.position, range);
    }
}

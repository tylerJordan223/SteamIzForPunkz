using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    /*
     * This is the basic script for all enemy movement
     * movement types can be turned on or off
     */

    //different movement types
    public bool directional;
    public bool astar;

    //enemy variables
    private Transform trans;
    private Rigidbody2D rb;
    private Transform player;
    public float speed;
    public float minDistanceFromPlayer;
    private bool punching;
    private Vector2 punchDirection;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        trans = GetComponent<Transform>();
        punching = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            punchDirection = trans.position - player.position;
            if(punching)
            {
                StopCoroutine(Punch());
                StartCoroutine(Punch());
            }
            else
            {
                StartCoroutine(Punch());
            }
        }
    }
    private void FixedUpdate()
    {
        //pick the type of movement
        if(directional)
        {
            DirectionalMovement();
        }else if(astar)
        {

        }

        //check for punch knockback
        if(punching)
        {
            rb.AddForce(punchDirection.normalized * 200, ForceMode2D.Force);
        }

        //slow down any velocity pushed
        rb.velocity *= new Vector2(0.3f,0.3f);
    }

    private void DirectionalMovement()
    {
        //get distance from player
        float distance = Vector2.Distance(trans.position, player.position);
        //move towards player
        if (distance > minDistanceFromPlayer)
        {
            trans.position += (player.position - trans.position).normalized * speed * Time.deltaTime;
        }
        else if (distance < minDistanceFromPlayer - 0.5)
        {
            trans.position -= (player.position - trans.position).normalized * speed * Time.deltaTime;
        }
    }

    private IEnumerator Punch()
    {
        punching = true;
        yield return new WaitForSeconds(0.25f);
        punching = false;
    }
}

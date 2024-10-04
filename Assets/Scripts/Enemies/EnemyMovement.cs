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
    private bool kb;
    private Vector2 kbDirection;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        trans = GetComponent<Transform>();
        kb = false;
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
        if(kb)
        {
            rb.AddForce(kbDirection.normalized * 200, ForceMode2D.Force);
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

    public void Knockback()
    {
        kbDirection = trans.position - player.position;
        if (kb)
        {
            StopCoroutine(IKnockback());
            StartCoroutine(IKnockback());
        }
        else
        {
            StartCoroutine(IKnockback());
        }
    }

    private IEnumerator IKnockback()
    {
        kb = true;
        yield return new WaitForSeconds(0.25f);
        kb = false;
    }
}

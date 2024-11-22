using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private GameObject player;
    public float speed;
    public float minDistanceFromPlayer;
    private bool kb;
    private Vector2 kbDirection;
    public bool dead;
    [SerializeField] public Color aliveColor;
    private Vector3 targetDirection;

    //details for object avoidance
    [Header("Object Avoidance")]
    [SerializeField] private float ObstacleDetectionRadius;
    [SerializeField] private float ObstacleDetectionDistance;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private float rotationSpeed;
    private RaycastHit2D[] oHits;

    private void Start()
    {
        oHits = new RaycastHit2D[10];
        player = null;
        rb = GetComponent<Rigidbody2D>();
        trans = GetComponent<Transform>();
        kb = false;
        dead = false;
        GetComponent<SpriteRenderer>().color = aliveColor;
    }
    private void FixedUpdate()
    {
        //pick the type of movement, and only move if alive and player in room
        if(!dead && gameObject.GetComponent<EnemyHealth>().my_room.active == true)
        {
            if(directional)
            {
                DirectionalMovement();
                //manages object avoidance
                ObjectAvoidance();
            }else if(astar)
            {

            }
            //actually add the movement
            trans.position += speed * Time.deltaTime * targetDirection.normalized;
        }

        //check for punch knockback
        if(kb)
        {
            rb.AddForce(kbDirection.normalized * 200, ForceMode2D.Force);
        }

        //slow down any velocity pushed
        rb.velocity *= new Vector2(0.3f,0.3f);

        if(player == null && GameObject.Find("Tric"))
        {
            player = GameObject.Find("Tric");
        }
    }

    private void DirectionalMovement()
    {
        //get distance from player
        float distance = Vector2.Distance(trans.position, player.transform.position);
        //move towards player
        if (distance > minDistanceFromPlayer)
        {
            targetDirection = player.transform.position - trans.position;
        }
        else if (distance < minDistanceFromPlayer - 0.5)
        {
            //move backwards
            targetDirection = trans.position - player.transform.position;
        }
    }

    private void ObjectAvoidance()
    {
        //create a contact filter for the layer mask
        ContactFilter2D cf = new ContactFilter2D();
        cf.SetLayerMask(obstacleLayerMask);
        //stores all collisions made inside of an array
        int hits = Physics2D.CircleCast(transform.position, ObstacleDetectionRadius, transform.up, cf, oHits, ObstacleDetectionDistance);

        for(int i = 0; i < hits; i++)
        {
            RaycastHit2D h = oHits[i];
            if(h.collider.gameObject == gameObject)
            {
                continue;
            }

            //at this point i have the object im colliding with
        }
    }

    public void Knockback()
    {
        kbDirection = trans.position - player.transform.position;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && !dead)
        {
            collision.gameObject.GetComponent<PlayerScript>().Damage(1f);
        }
    }
}

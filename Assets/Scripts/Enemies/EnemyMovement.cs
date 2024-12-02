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
    private WorldNode my_node;

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
        
        //get the node and center it
        my_node = DataManager.g.getNode(transform);
        transform.position = my_node.getPosition3();
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
                AstarMovement();
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

        //update my node
        if(DataManager.g.getNode(transform) != my_node)
        {
            my_node = DataManager.g.getNode(transform);
        }
    }

    private void DirectionalMovement()
    {
        //get distance from player
        float distance = Vector2.Distance(trans.position, player.transform.position);
        //move towards player
        if (distance > minDistanceFromPlayer)
        {
            targetDirection = DataManager.g.playerNode.getPosition3() - trans.position;
        }
        else if (distance < minDistanceFromPlayer - 0.5)
        {
            //move backwards
            targetDirection = DataManager.g.playerNode.getPosition3() - player.transform.position;
        }
    }

    private void AstarMovement()
    {
        List<WorldNode> path = DataManager.g.AStarSearchToPlayer(my_node);

        if(path.Count > 0)
        {
            WorldNode next = path[0];
            Vector2 target = next.getPosition2();

            //if the enemy isnt close enough then keep moving
            if(Vector2.Distance(trans.position, target) >= 0.05f)
            {
                //move towards the next node
                targetDirection = next.getPosition3() - trans.position;
            }
            else
            {
                //set the enemy to the node to make sure it doesnt get stuck
                transform.position = target;
                path.Remove(path[0]);
            }
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

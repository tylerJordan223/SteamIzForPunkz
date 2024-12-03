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

    //enemy types
    public bool flying;

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
    private Animator anim;

    //for flying enemies
    BoxCollider2D flying_enemy;
    CapsuleCollider2D shadow;
    private bool attacking;

    private void Start()
    {
        player = null;
        rb = GetComponent<Rigidbody2D>();
        trans = GetComponent<Transform>();
        kb = false;
        dead = false;
        //sets the alive color at the child sprite
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = aliveColor;
        anim = GetComponent<Animator>();

        //get the node and center it
        my_node = DataManager.g.getNode(transform);
        transform.position = my_node.getPosition3();

        //set the flying variables only if its flying
        if(flying)
        {
            flying_enemy = transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>();
            flying_enemy.enabled = false;
            shadow = GetComponent<CapsuleCollider2D>();
            shadow.enabled = true;
            anim.SetBool("flying", flying);
            attacking = false;
        }
    }

    private void FixedUpdate()
    {
        //pick the type of movement, and only move if alive and player in room
        if(!dead && gameObject.GetComponent<EnemyHealth>().my_room.active == true && !attacking)
        {
            if(directional)
            {
                DirectionalMovement();
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

    private IEnumerator FlyingAnimation()
    {
        attacking = true;
        //shadow.enabled = false;
        anim.SetBool("flying", false);

        yield return new WaitForSeconds(5f);

        //if it died during landing it doesnt get up
        if(!dead)
        {
            flying_enemy.enabled = false;
            anim.SetBool("flying", true);
            yield return new WaitForSeconds(0.5f);
            flying = true;
            attacking = false;

            //an extra second before it can attack again (cooldown)
            yield return new WaitForSeconds(1f);
            shadow.enabled = true;
        }
    }

    public void FlyingAttack()
    {
        flying_enemy.enabled = true;
        flying = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && !dead)
        {
            collision.gameObject.GetComponent<PlayerScript>().Damage(1f);
        }
    }

    //for flying enemies only
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !dead && flying)
        {
            StartCoroutine(FlyingAnimation());
        }
    }
}

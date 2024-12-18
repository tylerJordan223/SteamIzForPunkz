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

    //a check to see if its in a boss room
    public bool isWithBoss;

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
    [SerializeField] Animator anim;

    //for flying enemies
    BoxCollider2D flying_enemy;
    CapsuleCollider2D shadow;
    private bool attacking;

    //timer
    private float active_timer;
    private float time_to_activate;

    private void Start()
    {
        player = null;
        rb = GetComponent<Rigidbody2D>();
        trans = GetComponent<Transform>();
        kb = false;
        dead = false;
        //sets the alive color at the child sprite
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = aliveColor;

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

        //randomizing the speed if boss
        if(isWithBoss)
        {
            speed = Random.Range(1, 5);
        }

        //timer to start moving
        active_timer = 0f;
        time_to_activate = 1f;
    }

    private void FixedUpdate()
    {
        if (player == null && GameObject.Find("Tric"))
        {
            player = GameObject.Find("Tric");
        }

        //pick the type of movement, and only move if alive and player in room
        if (!dead && gameObject.GetComponent<EnemyHealth>().my_room != null && !attacking)
        {
            if(gameObject.GetComponent<EnemyHealth>().my_room.locked)
            {
                if(active_timer >= time_to_activate)
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

                //increase the timer to activate
                active_timer += Time.deltaTime;
            }
            else
            {
                active_timer = 0f;
            }
        }

        UpdateAnimation();

        //specifically for boss
        if (!dead && isWithBoss && !attacking)
        {
            if (directional)
            {
                DirectionalMovement();
            }
            else if (astar)
            {
                AstarMovement();
            }

            //actually add the movement
            trans.position += speed * Time.deltaTime * targetDirection.normalized;
        }

        //check for punch knockback
        if (kb)
        {
            rb.AddForce(kbDirection.normalized * 200, ForceMode2D.Force);
        }

        //slow down any velocity pushed
        rb.velocity *= new Vector2(0.3f,0.3f);

        //update my node
        if(DataManager.g.getNode(transform) != my_node)
        {
            my_node = DataManager.g.getNode(transform);
        }

        //kills a boss enemy if the fight is over
        if(isWithBoss)
        {
            if(GameObject.Find("Tric").GetComponent<PlayerStats>().special == 3)
            {
                Destroy(this.gameObject);
            }
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
        if(player != null)
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
        directional = false;
        //shadow.enabled = false;
        anim.SetBool("flying", false);

        yield return new WaitForSeconds(1.2f);

        AudioManager.instance.PlaySingleSFX(AudioManager.instance.fly_drop);

        yield return new WaitForSeconds(5f);

        //if it died during landing it doesnt get up
        if(!dead)
        {
            flying_enemy.enabled = false;
            anim.SetBool("flying", true);
            yield return new WaitForSeconds(0.5f);
            flying = true;
            attacking = false;
            directional = true;

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

    private void UpdateAnimation()
    {
        if(flying)
        {
            if(dead)
            {
                anim.SetBool("dead", true);
            }
        }
        else if (!isWithBoss)
        {
            if(gameObject.GetComponent<EnemyHealth>().my_room.locked)
            {
                anim.SetBool("moving", true);
            }
            if(dead)
            {
                anim.SetBool("dead", true);
            }

            //update direction
            if(targetDirection != null)
            {
                anim.SetFloat("x", targetDirection.normalized.x);
                anim.SetFloat("y", targetDirection.normalized.y);
            }

            anim.speed = speed;
        }
        else if(directional)
        {
            anim.SetBool("moving", true);

            if (targetDirection != null)
            {
                anim.SetFloat("x", targetDirection.normalized.x);
                anim.SetFloat("y", targetDirection.normalized.y);
            }

            anim.speed = speed;
        }
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

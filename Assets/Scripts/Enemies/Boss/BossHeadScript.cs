using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHeadScript : MonoBehaviour
{
    //objects to manage

    [SerializeField] public GameObject player;
    [SerializeField] GameObject right_hand;
    [SerializeField] GameObject left_hand;
    [SerializeField] BoxCollider2D weakPoint;

    [Header("Boss Drop")]
    [SerializeField] GameObject bossDrop;

    //animators
    [SerializeField] Animator idle_anim;
    [SerializeField] Animator movement_anim;

    [Header("Boss Attributes")]
    [SerializeField] float speed;
    [SerializeField] GameObject enemy_spawner;

    private bool idle = false;
    public bool dead = false;
    public bool active = false;

    //attacking system
    public float damage_timer;
    public float time_between_attacks;

    private void Start()
    {
        DisableIdle();
        weakPoint.enabled = false;
        damage_timer = 0f;
    }

    private void Update()
    {
        if (idle)
        {
            if ((34 <= transform.position.x && transform.position.x <= 55) && (34 < player.transform.position.x && player.transform.position.x < 55))
            {
                if (transform.position.x < player.transform.position.x - 0.5)
                {
                    transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
                }
                else if (transform.position.x > player.transform.position.x + 0.5)
                {
                    transform.position -= new Vector3(speed * Time.deltaTime, 0f, 0f);
                }
            }
            else if ((34 < transform.position.x && transform.position.x < 55))
            {
                if (player.transform.position.x > 55)
                {
                    transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
                }
                else if (player.transform.position.x < 34)
                {
                    transform.position -= new Vector3(speed * Time.deltaTime, 0f, 0f);
                }
            }
            else
            {
                if (34 > transform.position.x)
                {
                    transform.position = new Vector3(34f, transform.position.y, 0f);
                }
                else if (transform.position.x > 55)
                {
                    transform.position = new Vector3(55f, transform.position.y, 0f);
                }
            }

            //handling a new attack
            damage_timer += Time.deltaTime;
            if(damage_timer > time_between_attacks && !active)
            {
                PickAttack();
            }
        }

    }

    public void PickAttack()
    {
        int hand_choice = Random.Range(1, 4);

        if(hand_choice == 3 && !dead)
        {
            StartCoroutine(SpawnAttack());
            active = true;
        }
        else if(hand_choice == 2 && right_hand)
        {
            right_hand.GetComponent<BossHandScript>().FollowPlayer();
            active = true;
        }
        else if(hand_choice == 1 && left_hand)
        {
            left_hand.GetComponent<BossHandScript>().FollowPlayer();
            active = true;
        }

        //makes sure if the attack picked was broken to reroll until it finds a suitable attack
        if(!active)
        {
            PickAttack();
        }
    }

    public void EnableIdle()
    {
        idle_anim.enabled = true;
        idle = true;
    }

    public void DisableIdle()
    {
        idle_anim.enabled = false;
        idle = false;
    }

    public void ResetAttack()
    {
        active = false;
        damage_timer = 0f;
    }

    public IEnumerator SpawnAttack()
    {
        DisableIdle();
        movement_anim.SetBool("spawnAttack", true);
        //wait until its done spawning
        yield return new WaitForSeconds(3f);

        weakPoint.enabled = true;

        for (int i = 0; i < 5; i++)
        {
            if (!dead)
            {
                GameObject e = Instantiate(enemy_spawner);
                e.transform.position = transform.Find("WeakPoint").position;
                e.transform.position = new Vector3(e.transform.position.x, e.transform.position.y - 1f, e.transform.position.z);
                yield return new WaitForSeconds(1f);
            }
        }

        //wait for a bit to allow player to hit the weak point
        yield return new WaitForSeconds(10f);

        if (!dead)
        {
            weakPoint.enabled = false;
            movement_anim.SetBool("spawnAttack", false);
            yield return new WaitForSeconds(3f);
        }

        EnableIdle();
        ResetAttack();
        damage_timer = 0f;
    }

    //for when its weak point is damaged enough
    public void StartBreak()
    {
        dead = true;
        weakPoint.enabled = false;
        movement_anim.SetBool("broken", true);
    }

    public void Break()
    {
        GameObject bd = Instantiate(bossDrop);
        bd.GetComponent<ItemScript>().transform.position = transform.position;
        bd.GetComponent<ItemScript>().starting_position = transform.position;
        bd.GetComponent<ItemScript>().goal_position = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
    }
}

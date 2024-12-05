using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHeadScript : MonoBehaviour
{
    //objects to manage

    [SerializeField] public GameObject player;
    [SerializeField] GameObject right_hand;
    [SerializeField] GameObject left_hand;
    [SerializeField] Animator anim;

    [Header("Boss Attributes")]
    [SerializeField] float speed;

    private bool idle = false;
    public bool dead = false;
    public bool active = false;

    //attacking system
    public float damage_timer;
    public float time_between_attacks;

    private void Start()
    {
        DisableIdle();
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

            damage_timer += Time.deltaTime;
            if(damage_timer > time_between_attacks && !active)
            {
                PickAttack();
            }
        }

    }

    public void PickAttack()
    {
        active = true;
        int hand_choice = Random.Range(1, 3);

        if(hand_choice == 2 && right_hand)
        {
            right_hand.GetComponent<BossHandScript>().FollowPlayer();   
        }else if(hand_choice == 1 && left_hand)
        {
            left_hand.GetComponent<BossHandScript>().FollowPlayer();
        }

        //head attack is the last one
    }

    public void EnableIdle()
    {
        anim.enabled = true;
        idle = true;
    }

    public void DisableIdle()
    {
        anim.enabled = false;
        idle = false;
    }

    public void ResetAttack()
    {
        active = false;
        damage_timer = 0f;
    }
}

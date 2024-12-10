using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossHealth : MonoBehaviour
{
    //differentiates between the two
    private BossHandScript my_hand;
    private BossHeadScript my_head;

    [Header("Boss Part")]
    public bool head;
    public bool hand;

    [Header("Health Variables")]
    [SerializeField] float maxHealth;
    public float health;

    [Header("Damage Variables")]
    //limiting the amount of damage (for spin attack)
    [SerializeField] public float timeBetweenDamage;
    private SpriteRenderer sr;
    private float damageTimer;

    private void Start()
    {
        if(head)
        {
            my_head = transform.parent.GetComponent<BossHeadScript>();
        }else if(hand)
        {
            my_hand = transform.parent.GetComponent<BossHandScript>();
        }

        //get various important objects
        sr = GetComponent<SpriteRenderer>();
        damageTimer = timeBetweenDamage;
        health = maxHealth;
    }
    private void Update()
    {
        damageTimer += Time.deltaTime;

        //dont need to check for death because death will be an explosion for the boss
        if(health > 0)
        {
            if (damageTimer < timeBetweenDamage)
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f);
            }
            else
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            }
        }
    }

    public void Damage(float d)
    {
        if (damageTimer > timeBetweenDamage)
        {
            //reset the timer
            damageTimer = 0f;


            if (health > 0)
            {
                health -= d;
                //** PLAY DAMAGE ANIMATION
                if (health <= 0)
                {
                    if(head)
                    {
                        my_head.StartBreak();
                    }else if(hand)
                    {
                        my_hand.dead = true;
                    }
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && hand && !my_hand.dead)
        {
            collision.gameObject.GetComponent<PlayerScript>().Damage(1f);
        }
    }
}

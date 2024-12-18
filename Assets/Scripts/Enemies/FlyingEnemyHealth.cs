using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyHealth : MonoBehaviour
{
    //enemy variables
    private FlyingEnemyMovement em;
    public RoomScript my_room;

    [Header("Health Variables")]
    [SerializeField] float maxHealth;
    public float health;

    [Header("Damage Variables")]
    //limiting the amount of damage (for spin attack)
    [SerializeField] public float timeBetweenDamage;
    [SerializeField] public Color deathColor;
    private SpriteRenderer sr;
    private float damageTimer;

    [Header("Drops on Death")]
    [SerializeField] private GameObject drop;

    private void Start()
    {
        health = maxHealth;
        em = GetComponent<FlyingEnemyMovement>();
        sr = GetComponent<SpriteRenderer>();
        damageTimer = timeBetweenDamage;
    }

    private void Update()
    {
        damageTimer += Time.deltaTime;

        //update visually
        if (!em.dead)
        {
            if (damageTimer < timeBetweenDamage)
            {
                sr.color = new Color(em.aliveColor.r, em.aliveColor.g, em.aliveColor.b, 0.5f);
            }
            else
            {
                sr.color = new Color(em.aliveColor.r, em.aliveColor.g, em.aliveColor.b, 1f);
            }
        }
    }

    public void Damage(float d)
    {
        if (damageTimer > timeBetweenDamage)
        {
            //reset the timer
            damageTimer = 0f;
            //** PLAY DAMAGE ANIMATION
            //knockback
            em.Knockback();

            if (health > 0)
            {
                health -= d;

                if (health <= 0)
                {
                    Die();
                }
            }
        }
    }
    private void Die()
    {
        //kill the enemy
        this.gameObject.GetComponent<FlyingEnemyMovement>().dead = true;
        this.gameObject.GetComponent<SpriteRenderer>().color = deathColor;
        //drop money
        int amount_of_coins = Random.Range(1, (int)(4 * GameObject.Find("Tric").GetComponent<PlayerStats>().luck));
        //make a random amount of coins based on random
        for (int i = 0; i < amount_of_coins; i++)
        {
            Instantiate(drop, this.transform.position, this.transform.rotation);
        }
        //let the room know its got one less enemy
        my_room.enemy_count--;
    }
}

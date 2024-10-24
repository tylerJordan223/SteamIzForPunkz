using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    //enemy variables
    private EnemyMovement em;

    [Header("Health Variables")]
    [SerializeField] float maxHealth;
    public float health;

    [Header("Damage Variables")]
    //limiting the amount of damage (for spin attack)
    [SerializeField] public float timeBetweenDamage;
    [SerializeField] public Color deathColor;
    private float damageTimer;

    [Header("Drops on Death")]
    [SerializeField] private GameObject drop;

    private void Start()
    {
        health = maxHealth;
        em = GetComponent<EnemyMovement>();
    }

    private void Update()
    {
        damageTimer += Time.deltaTime;
    }

    public void Damage(float d)
    {
        if(damageTimer > timeBetweenDamage)
        {
            //reset the timer
            damageTimer = 0f;
            //** PLAY DAMAGE ANIMATION
            //knockback
            em.Knockback();
            
            if(health > 0)
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
        this.gameObject.GetComponent<EnemyMovement>().dead = true;
        this.gameObject.GetComponent<SpriteRenderer>().color = deathColor;
        //drop money
        int amount_of_coins = Random.Range(1, (int) (4*GameObject.Find("Tric").GetComponent<PlayerScript>().luck) );
        //make a random amount of coins based on random
        for(int i = 0; i < amount_of_coins; i++)
        {
            Instantiate(drop, this.transform.position, this.transform.rotation);
        }
    }
}

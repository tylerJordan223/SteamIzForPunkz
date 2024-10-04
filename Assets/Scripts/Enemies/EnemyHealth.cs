using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float maxHealth;
    public float health;

    //limiting the amount of damage (for spin attack)
    [SerializeField] public float timeBetweenDamage;
    [SerializeField] public Color deathColor;
    private float damageTimer;

    private void Start()
    {
        health = maxHealth;
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
            this.gameObject.GetComponent<EnemyMovement>().Knockback();

            health -= d;

            if(health < d)
            {
                Die();
            }
        }
    }
    private void Die()
    {
        //Destroy(this.gameObject);
        this.gameObject.GetComponent<EnemyMovement>().dead = true;
        this.gameObject.GetComponent<SpriteRenderer>().color = deathColor;
    }
}

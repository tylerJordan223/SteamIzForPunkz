using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float maxHealth;
    public float health;

    private void Start()
    {
        health = maxHealth;
    }

    public void Damage(float d)
    {
        health -= d;

        if(health < d)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    [SerializeField] GameObject parent_hand;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !parent_hand.GetComponent<BossHandScript>().attacking)
        {
            parent_hand.GetComponent<BossHandScript>().SmashAttack();
        }
    }
}

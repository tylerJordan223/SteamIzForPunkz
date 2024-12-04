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

    private bool idle = true;
    public bool dead = false;

    private void Start()
    {
        DisableIdle();
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
        }
        
    }

    public void EnableIdle()
    {
        anim.enabled = true;
    }

    public void DisableIdle()
    {
        anim.enabled = false;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandScript : MonoBehaviour
{
    [SerializeField] GameObject boss_head;
    [SerializeField] GameObject player;
    [SerializeField] Animator anim;
    [SerializeField] GameObject my_shadow;

    private float head_offset;
    public bool idle;
    public bool attacking;

    private void Start()
    {
        DisableIdle();
        head_offset = transform.position.x - boss_head.transform.position.x;
        idle = false;
        attacking = false;
    }

    private void Update()
    {
        if(idle)
        {
            if(!anim.enabled)
            {
                EnableIdle();
                my_shadow.SetActive(false);
            }
            transform.position = Vector3.Lerp(transform.position, new Vector3(boss_head.transform.position.x + head_offset, boss_head.transform.position.y, 0f), Time.deltaTime);
        }else if(attacking)
        {
            if(anim.enabled)
            {
                DisableIdle();
                my_shadow.SetActive(true);
            }
            transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x, player.transform.position.y + (transform.position.y - my_shadow.transform.position.y), 0f), Time.deltaTime);
        }

    }


    public void EnableIdle()
    {
        anim.enabled = true;
        idle = true;
    }

    public void DisableIdle()
    {
        idle = false;
        anim.enabled = false;
    }

    public void FollowPlayer()
    {
        DisableIdle();
        attacking = true;
    }
}

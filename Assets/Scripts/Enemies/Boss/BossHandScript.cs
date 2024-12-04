using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHandScript : MonoBehaviour
{
    [SerializeField] GameObject boss_head;
    [SerializeField] GameObject player;
    [SerializeField] Animator parent_anim;
    [SerializeField] Animator anim;
    [SerializeField] GameObject my_shadow;

    private float head_offset;
    public bool idle;
    public bool attacking;
    public bool active;

    private void Start()
    {
        DisableIdle();
        my_shadow.SetActive(false);
        head_offset = transform.position.x - boss_head.transform.position.x;
        idle = false;
        active = false;
        attacking = false;
    }

    private void Update()
    {
        if(idle)
        {
            if(!parent_anim.enabled)
            {
                EnableIdle();
            }
            transform.position = Vector3.Lerp(transform.position, new Vector3(boss_head.transform.position.x + head_offset, boss_head.transform.position.y, 0f), Time.deltaTime);
        }else if(active)
        {
            if(parent_anim.enabled)
            {
                DisableIdle();
            }
            transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x, player.transform.position.y + (transform.position.y - my_shadow.transform.position.y), 0f), Time.deltaTime);
        }

    }


    public void EnableIdle()
    {
        parent_anim.enabled = true;
        my_shadow.SetActive(false);
        idle = true;
    }

    public void DisableIdle()
    {
        idle = false;
        my_shadow.SetActive(true);
        parent_anim.enabled = false;
    }

    public void FollowPlayer()
    {
        DisableIdle();
        active = true;
    }

    public void SmashAttack()
    {
        attacking = true;
        StartCoroutine(HandDrop());
    }

    public IEnumerator HandDrop()
    {
        my_shadow.GetComponent<CircleCollider2D>().enabled = false;
        anim.SetBool("Drop", true);
        active = false;

        yield return new WaitForSeconds(1f);

        transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>().enabled = true;

        yield return new WaitForSeconds(5f);

        transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>().enabled = false;
        anim.SetBool("Drop", false);

        yield return new WaitForSeconds(0.5f);

        idle = true;
        attacking = false;

        //an extra second before it can attack again (cooldown)
        yield return new WaitForSeconds(1f);

        my_shadow.GetComponent<CircleCollider2D>().enabled = true;
        
    }
}

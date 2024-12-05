using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
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

    [Header("Drops on Death")]
    [SerializeField] private GameObject drop;

    //materials
    [Header("Handling Materials")]
    [SerializeField] Material normal;
    [SerializeField] Material crackle;
    private Material sprite_mat;
    private float fade;
    private float fade_dx;
    public bool dead;

    private void Start()
    {
        //setting the initial material to be nothing on the sprite
        sprite_mat = normal;
        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material = sprite_mat;
        fade = 0f;
        fade_dx = -0.1f;

        //setting other necessary variables
        DisableIdle();
        my_shadow.SetActive(false);
        head_offset = transform.position.x - boss_head.transform.position.x;
        
        //flags
        idle = false;
        active = false;
        attacking = false;
        dead = false;
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

        //only update if the hands are actively moving
        if(idle || active || attacking)
        {
            UpdateShadow();
        }

        //handle death
        if(dead)
        {
            if(sprite_mat == normal)
            {
                Debug.Log("crackle");
                sprite_mat = crackle;
                transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material = sprite_mat;
            }

            if(fade <= 0.10)
            {
                fade_dx = 1f;
            }else if(fade >= 0.90)
            {
                fade_dx = -1f;
            }

            fade += fade_dx * Time.deltaTime;

            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Crack", fade);
        }else
        {
            if(sprite_mat == crackle)
            {
                sprite_mat = normal;
                transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material = sprite_mat;
            }
        }
    }


    public void EnableIdle()
    {
        parent_anim.enabled = true;
        idle = true;
    }

    public void DisableIdle()
    {
        idle = false;
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

        if(!dead)
        {
            transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>().enabled = false;
            anim.SetBool("Drop", false);

            yield return new WaitForSeconds(0.5f);

            idle = true;
            attacking = false;

            //an extra second before it can attack again (cooldown)
            yield return new WaitForSeconds(1f);

            my_shadow.GetComponent<CircleCollider2D>().enabled = true;
        }
        else
        {
            anim.SetBool("Dead", true);
        }

        boss_head.GetComponent<BossHeadScript>().ResetAttack();
    }

    //makes sure the shadow only shows when its supposed to
    public void UpdateShadow()
    {
        if (my_shadow.transform.position.y < 24f && !my_shadow.activeSelf)
        {
            my_shadow.SetActive(true);
        }
        else if (my_shadow.transform.position.y > 24f && my_shadow.activeSelf)
        {
            my_shadow.SetActive(false);
        }
    }

    public void Die()
    {
        GameObject charge_item = Instantiate(drop);
        charge_item.transform.position = transform.position;
        charge_item.GetComponent<BossItemScript>().goal_position = new Vector3(transform.position.x, transform.position.y - 3, transform.position.z);
        Destroy(this.gameObject);
    }
}

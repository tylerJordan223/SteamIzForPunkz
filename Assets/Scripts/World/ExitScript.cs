using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    //script to handle the object that leads player to the next floor

    private bool on_player = false;
    private bool exiting = false;
    [SerializeField] List<SpriteRenderer> fake_tric;
    [SerializeField] Animator anim;
    [SerializeField] Material outline;
    [SerializeField] Color outline_color;
    [SerializeField] Material normal;
    private Material my_material;

    private void Start()
    {
        my_material = normal;
        GetComponent<SpriteRenderer>().material = normal;
    }

    private void Update()
    {
        if(on_player && Input.GetKeyDown(KeyCode.E))
        {
            GameObject.Find("Tric").GetComponent<PlayerScript>().canControl = false;
            DoExit();
        }

        //handle materials
        if(on_player && !exiting)
        {
            my_material = outline;
            GetComponent<SpriteRenderer>().material = my_material;
            GetComponent<SpriteRenderer>().material.SetColor("_Color", outline_color);
        }
        else
        {
            my_material = normal;
            GetComponent<SpriteRenderer>().material = my_material;
        }
    }

    public void DoExit()
    {
        GameObject.Find("Tric").transform.Find("Sprites").GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = -15;
        GameObject.Find("Tric").transform.Find("Sprites").GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = -15;
        AudioManager.instance.PlaySingleSFX(AudioManager.instance.exit);

        anim.SetBool("exiting", true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            on_player = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            on_player = false;
        }
    }

    
}

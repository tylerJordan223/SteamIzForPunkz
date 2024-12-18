using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MachineScript : MonoBehaviour
{
    [SerializeField] ItemMachineScript my_machine;
    [SerializeField] SpriteRenderer my_sr;
    [SerializeField] Animator anim;

    private void Update()
    {
        //updating the position so its moves with the player properly
        if(my_machine.on_player)
        {
            if(transform.position.y < GameObject.Find("Tric").transform.position.y)
            {
                my_sr.sortingOrder = GameObject.Find("Tric").transform.Find("Sprites").GetChild(0).GetComponent<SpriteRenderer>().sortingOrder + 3;
                transform.GetChild(0).GetComponent<TextMeshPro>().sortingOrder = my_sr.sortingOrder+2;
            }
            else
            {
                my_sr.sortingOrder = GameObject.Find("Tric").transform.Find("Sprites").GetChild(0).GetComponent<SpriteRenderer>().sortingOrder - 3;
                transform.GetChild(0).GetComponent<TextMeshPro>().sortingOrder = my_sr.sortingOrder+2;
            }
        }
    }

    //method to be used in animation to trigger the item spawn
    public void EndAnimation()
    {
        anim.SetBool("cycling", false);
        my_machine.SpinForItem();
    }

    public void StartAnimation()
    {
        anim.SetBool("cycling", true);
        my_machine.spinning = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            my_machine.on_player = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            my_machine.on_player = false;
        }
    }
}

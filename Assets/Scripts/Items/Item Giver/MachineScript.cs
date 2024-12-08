using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MachineScript : MonoBehaviour
{
    [SerializeField] ItemMachineScript my_machine;
    [SerializeField] SpriteRenderer my_sr;

    private void Update()
    {
        //updating the position so its moves with the player properly
        if(my_machine.on_player)
        {
            if(transform.position.y < GameObject.Find("Tric").transform.position.y)
            {
                my_sr.sortingOrder = GameObject.Find("Tric").GetComponent<SpriteRenderer>().sortingOrder + 2;
                transform.GetChild(0).GetComponent<TextMeshPro>().sortingOrder = my_sr.sortingOrder+1;
            }
            else
            {
                my_sr.sortingOrder = GameObject.Find("Tric").GetComponent<SpriteRenderer>().sortingOrder - 2;
                transform.GetChild(0).GetComponent<TextMeshPro>().sortingOrder = my_sr.sortingOrder+1;
            }
        }
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

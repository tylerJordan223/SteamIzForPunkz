using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    //script to handle the object that leads player to the next floor

    private bool on_player = false;

    private void Update()
    {
        if(on_player && Input.GetKeyDown(KeyCode.E))
        {
            GameObject.Find("Tric").GetComponent<PlayerScript>().canControl = false;
            DataManager.StartLoad();
        }
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

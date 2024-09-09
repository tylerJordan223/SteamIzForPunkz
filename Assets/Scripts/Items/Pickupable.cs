using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    //THIS SCRIPT IS FOR EVERY PICKUPABLE ITEM//

    //DESCRIPTION//
    /*
     * This is the base script for every pickupable item
     * will add the item to a list of pickupable items
     * this is not to be confused with consumables which will be added to an inventory
     * this is just to hold one item at a time
     */

    //Picking up the object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            //mark that player is intersected and save player object slot
            collision.GetComponent<PlayerScript>().itemList.Add(this.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            //unset player object slot and turn on player flag
            collision.GetComponent<PlayerScript>().itemList.Remove(this.gameObject);
        }
    }
}

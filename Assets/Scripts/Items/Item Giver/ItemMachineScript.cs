using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemMachineScript : MonoBehaviour
{
    //various objects
    [Header("Connected Objects")]
    [SerializeField] Transform starting_position;
    [SerializeField] Transform landing_spot;
    [SerializeField] GameObject item_machine;
    [SerializeField] TextMeshPro cost_text;
    private GameObject my_item;

    [Header("PossibleItems")]
    [SerializeField] List<GameObject> items;
    [SerializeField] GameObject chargeItem;
    [SerializeField] GameObject healthItem;
    public bool health_machine;
    public bool charge_machine;


    [Header("Materials")]
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Material normal;
    [SerializeField] Material outline;
    [SerializeField] Color can_use;
    [SerializeField] Color cant_use;
    private Material my_material;

    private int cost;
    public bool on_player;
    public bool spinning;

    private void Start()
    {
        my_material = normal;
        sr.material = my_material;
        spinning = false;

        //setting the initial cost (scales per floor)
        cost = 5 + DataManager.floorNumber * 5;
        cost_text.text = cost.ToString();
    }

    private void Update()
    {
        //doesnt update if player has not picked up the item
        if(my_item == null && !spinning)
        {
            //handling materials
            if (on_player)
            {
                my_material = outline;
                sr.material = my_material;
                if(GameObject.Find("Tric").GetComponent<PlayerStats>().moneyCount >= cost)
                {
                    sr.material.SetColor("_Color", can_use);
                }
                else
                {
                    sr.material.SetColor("_Color", cant_use);
                }
            }
            else
            {
                my_material = normal;
                sr.material = my_material;
            }

            if(on_player && Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }

            //update regularly unless there is an item
            cost_text.text = cost.ToString();
        }
        else
        {
            cost_text.text = "";

            my_material = normal;
            sr.material = my_material;
        }
    }

    public void Interact()
    {
        if(GameObject.Find("Tric").GetComponent<PlayerStats>().moneyCount >= cost)
        {
            //update the players money
            GameObject.Find("Tric").GetComponent<PlayerStats>().moneyCount -= cost;
            cost += 5;

            //disable the Glow
            my_material = normal;
            sr.material = my_material;

            //start the animation
            item_machine.GetComponent<MachineScript>().StartAnimation();
        }
    }

    public void SpinForItem()
    {
        if(health_machine)
        {
            my_item = healthItem;
        }else if(charge_machine)
        {
            my_item = chargeItem;
        }
        else
        {
            my_item = items[Random.Range(0, items.Count)];
        }

        //a quick check to mkae sure it doesnt double up on any items that you can only have one of
        bool can_spawn = true;
        foreach(ItemScript i in DataManager.inventory)
        {
            if (i != null)
            {
                
                if(i.item_name.Equals(my_item.GetComponent<ItemScript>().item_name))
                {
                    can_spawn = false;
                }
            }
        }

        if ( can_spawn || my_item.GetComponent<ItemScript>().repeatable)
        {
            //pick a random item
            my_item = Instantiate(my_item);
            my_item.transform.position = starting_position.position;
            my_item.GetComponent<ItemScript>().goal_position = landing_spot.position;
            spinning = false;
        }
        else
        {
            //reroll if it cant use the selected item
            SpinForItem();
        }
    }
}

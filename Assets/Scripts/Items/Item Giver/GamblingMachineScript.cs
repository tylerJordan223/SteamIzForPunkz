using GInput;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamblingMachineScript : MonoBehaviour
{
    //various objects
    [Header("Connected Objects")]
    [SerializeField] Transform starting_position;
    [SerializeField] Transform landing_spot;
    private List<GameObject> items;
    private GameObject current_item;

    [Header("Materials")]
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Material normal;
    [SerializeField] Material outline;
    [SerializeField] Color can_use;
    [SerializeField] Color cant_use;
    private Material my_material;

    public bool on_player;

    private GameInput input;
    public bool opened;

    private void Start()
    {
        my_material = normal;
        sr.material = my_material;

        items = new List<GameObject>();
        opened = false;
    }

    private void OnEnable()
    {
        input = new GameInput();
        input.Player.Interact.Enable();
    }

    private void OnDisable()
    {
        input.Player.Interact.Disable();
    }

    private void Update()
    {
        //doesnt update if player has not picked up the item
        if (items.Count == 0 && !opened)
        {
            if(!input.Player.enabled)
            {
                input.Player.Interact.Enable();
            }

            //handling materials
            if (on_player)
            {
                my_material = outline;
                sr.material = my_material;

                sr.material.SetColor("_Color", can_use);
            }
            else
            {
                my_material = normal;
                sr.material = my_material;
            }

            if (on_player && input.Player.Interact.IsPressed())
            {
                OpenMenu();
            }
        }
        else
        {

            my_material = normal;
            sr.material = my_material;
        }
    }

    //function to open the menu
    public void OpenMenu()
    {
        input.Player.Interact.Disable();
        UISlotScript.instance.activateSlots(this);
        opened = true;
    }
}

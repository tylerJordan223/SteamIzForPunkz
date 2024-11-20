using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    //the script for all the stat changing items

    //descriptions
    [SerializeField] public string item_name;
    [SerializeField] public string description;
    [SerializeField] public string stats_up;
    [SerializeField] public string stats_down;
    [SerializeField] public Sprite isprite;

    //rarities
    [Header("Item Rarity")]
    public bool common;
    public bool rare;
    public bool epic;
    public bool legendary;

    //possible stat changes
    [Header("Stats")]
    public float speed;
    public float dash_speed;
    public float melee_damage;
    public float blast_damage;
    public float max_charges;
    public float max_health;
    public float luck;
    public float range_from_player;
    public float max_spin_speed;
    public float spin_acceleration;
    public float time_to_recharge;
    public float weapon_size;
    public float size_when_charged;
    public float time_to_charge;
    public float max_ricochets;

    //things that will be the same between all items

    //materials
    [Header("Handling Materials")]
    [SerializeField] Material normal;
    [SerializeField] Material glow;
    [SerializeField] Material vanish;
    private Material my_mat;
    private bool on_player;
    private float fade;
    private bool picked_up;

    //rarity colors
    [SerializeField] Color common_c;
    [SerializeField] Color rare_c;
    [SerializeField] Color epic_c;
    [SerializeField] Color legendary_c;
    public Color glow_color;

    private void Start()
    {
        //setting the initial material to be nothing
        this.gameObject.GetComponent<SpriteRenderer>().material = normal;

        on_player = false;
        picked_up = false;
        fade = 1f;
        glow_color = common_c;
    }

    private void Update()
    {
        //code for vanishing
        if (picked_up && fade > 0f)
        {
            fade -= Time.deltaTime;

            if (fade <= 0f && picked_up)
            {
                GameObject.FindWithTag("Player").GetComponent<PlayerStats>().NewItem(this);
                Destroy(this.gameObject);
            }
        }
        else if (fade < 1f)
        {
            fade += Time.deltaTime;

            if (fade >= 1f)
            {
                fade = 1f;
            }
        }

        //actual pickup
        if (Input.GetKey(KeyCode.E) && on_player)
        {
            this.gameObject.GetComponent<SpriteRenderer>().material = vanish;
            this.gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Color", glow_color);
            picked_up = true;
        }

        //only run this if its using the right material
        if (this.gameObject.GetComponent<SpriteRenderer>().material.HasFloat("_Fade"))
        {
            this.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //on the player
            on_player = true;
            //using the glow shader
            if(!picked_up)
            {
                this.gameObject.GetComponent<SpriteRenderer>().material = glow;
            }
            //setting the rarity of the glow
            if (common)
            {
                glow_color = common_c;
            }
            else if (rare)
            {
                glow_color = rare_c;
            }
            else if (epic)
            {
                glow_color = epic_c;
            }
            else if (legendary)
            {
                glow_color = legendary_c;
            }

            //sets the color
            this.gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Color", glow_color);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //off the player
            on_player = false;
            if (!picked_up)
            {
                this.gameObject.GetComponent<SpriteRenderer>().material = normal;
            }
        }
    }
}
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private PlayerScript player;

    [Header("Display Text")]
    [SerializeField] TextMeshProUGUI moneyCount;
    [SerializeField] TextMeshProUGUI playerStats;

    //FOR DEBUG USE
    int selectedStat;

    void Start()
    {
        //do a check to make sure the player is in the scene 
        if(GameObject.Find("Tric"))
        {
            player = GameObject.Find("Tric").GetComponent<PlayerScript>();
            moneyCount.text = player.moneyCount.ToString();
        }
        else
        {
            moneyCount.text = "0";
        }
    }

    // Update is called once per frame
    void Update()
    {
        //re-update until it finds the player just in case
        if(player == null)
        {
            if (GameObject.Find("Tric"))
            {
                player = GameObject.Find("Tric").GetComponent<PlayerScript>();
            }
        }
        else
        {
            //everything that the UI needs to reflect
            moneyCount.text = player.moneyCount.ToString();

            //update stats
            UpdateStats();
        }
    }

    private void UpdateStats()
    {
        if(!player.debug)
        {
            //disable everything if debug mode is off
            playerStats.text = "";
            this.transform.Find("Player Stats").GetChild(0).gameObject.GetComponent<Image>().enabled = false;
        }
        else
        {
            //make the back visible
            this.transform.Find("Player Stats").GetChild(0).gameObject.GetComponent<Image>().enabled = true;

            //initialize an empty list
            List<string> stats = new List<string>();
            //add each stat to it
            stats.Add("Speed: " + player.dynSpeed);
            stats.Add("DashS: " + player.dynDashSpeed);
            stats.Add("MDmg: " + player.dynMeleeDamage);
            stats.Add("RDmg: " + player.dynRangeDamage);
            stats.Add("MCharge: " + player.dynMaxCharges);
            stats.Add("mHealth: " + player.maxHealth);
            stats.Add("Luck: " + player.luck);

            string new_text = "";
            for (int i = 0; i < stats.Count; i++)
            {
                if (i == player.selected_stat)
                {
                    new_text += "*";
                }
                new_text += stats[i] + "\n";
            }

            playerStats.text = new_text;
        }
    }
}

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private PlayerStats pstats;

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
            pstats = GameObject.Find("Tric").GetComponent<PlayerStats>();
            moneyCount.text = pstats.moneyCount.ToString();
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
        if(pstats == null)
        {
            if (GameObject.Find("Tric"))
            {
                pstats = GameObject.Find("Tric").GetComponent<PlayerStats>();
            }
        }
        else
        {
            //everything that the UI needs to reflect
            moneyCount.text = pstats.moneyCount.ToString();

            //update stats
            UpdateStats();
        }
    }

    private void UpdateStats()
    {
        if(!pstats.debug)
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
            for(int i = 0; i < pstats.stats.Length; i++)
            {
                stats.Add(pstats.stats_names[i] + ": " + pstats.stats[i]);
            }

            string new_text = "";
            for (int i = 0; i < stats.Count; i++)
            {
                if (i == pstats.selected_stat)
                {
                    new_text += "*";
                }
                new_text += stats[i] + "\n";
            }

            playerStats.text = new_text;
        }
    }
}

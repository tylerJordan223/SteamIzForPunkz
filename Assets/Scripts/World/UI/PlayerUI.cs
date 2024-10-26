using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private PlayerStats pstats;

    [Header("Display Text")]
    [SerializeField] TextMeshProUGUI moneyCount;
    [SerializeField] TextMeshProUGUI playerStats;
    [SerializeField] TextMeshProUGUI weaponStats;

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
        if (!pstats.debug)
        {
            //disable everything if debug mode is off
            playerStats.text = "";
            weaponStats.text = "";
            this.transform.Find("Player Stats").GetChild(0).gameObject.GetComponent<Image>().enabled = false;
            this.transform.Find("Player Stats").GetChild(2).gameObject.GetComponent<Image>().enabled = false;
        }
        else if (pstats.selected_stat < pstats.stats.Length)
        {
            //make the back visible
            this.transform.Find("Player Stats").GetChild(0).gameObject.GetComponent<Image>().enabled = true;
            this.transform.Find("Player Stats").GetChild(2).gameObject.GetComponent<Image>().enabled = false;
            weaponStats.text = "";

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
        else
        {
            //swap the backs
            this.transform.Find("Player Stats").GetChild(0).gameObject.GetComponent<Image>().enabled = false;
            this.transform.Find("Player Stats").GetChild(2).gameObject.GetComponent<Image>().enabled = true;
            playerStats.text = "";

            //initialize an empty list
            List<string> stats = new List<string>();
            //add each stat to it
            for (int i = 0; i < pstats.weapon_stats.Length; i++)
            {
                stats.Add(pstats.weapon_stats_names[i] + ": " + pstats.weapon_stats[i]);
            }

            string new_text = "";
            for (int i = 0; i < stats.Count; i++)
            {
                if (i == pstats.selected_stat - (pstats.stats.Length))
                {
                    new_text += "*";
                }
                new_text += stats[i] + "\n";
            }

            weaponStats.text = new_text;
        }
        
    }
}

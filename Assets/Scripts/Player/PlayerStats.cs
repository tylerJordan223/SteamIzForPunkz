using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //Script to handle all of the player's stats and items
    [Header("Player Stats")]
    public float[] stats;
    public string[] stats_names;

    public float dynSpeed
    {
        get
        {
            return stats[0];
        }
        set
        {
            stats[0] = value;
        }
    }
    public float dynDashSpeed
    {
        get
        {
            return stats[1];
        }
        set
        {
            stats[1] = value;
        }
    }
    public float dynMeleeDamage
    {
        get
        {
            return stats[2];
        }
        set
        {
            stats[2] = value;
        }
    }
    public float dynBlastDamage
    {
        get
        {
            return stats[3];
        }
        set
        {
            stats[3] = value;
        }
    }
    public float dynMaxCharges
    {
        get
        {
            return stats[4];
        }
        set
        {
            stats[4] = value;
        }
    }
    public float maxHealth
    {
        get
        {
            return stats[5];
        }
        set
        {
            stats[5] = value;
        }
    }
    public float luck
    {
        get
        {
            return stats[6];
        }
        set
        {
            stats[6] = value;
        }
    }
    public float health
    {
        get
        {
            return stats[7];
        }
        set
        {
            stats[7] = value;
        }
    }
    public float charges
    {
        get
        {
            return stats[8];
        }
        set
        {
            stats[8] = value;
        }
    }

    [Header("Weapon Stats")]
    public float[] weapon_stats;
    public string[] weapon_stats_names;

    public float dynRange
    {
        get
        {
            return weapon_stats[0];
        }
        set
        {
            weapon_stats[0] = value;
        }
    }
    public float dynMaxSpeed
    {
        get
        {
            return weapon_stats[1];
        }
        set
        {
            weapon_stats[1] = value;
        }
    }
    public float dynSpinAcceleration
    {
        get
        {
            return weapon_stats[2];
        }
        set
        {
            weapon_stats[2] = value;
        }
    }
    public float dynRechargeTime
    {
        get
        {
            return weapon_stats[3];
        }
        set
        {
            weapon_stats[3] = value;
        }
    }
    public float dynSize
    {
        get
        {
            return weapon_stats[4];
        }
        set
        {
            weapon_stats[4] = value;
        }
    }
    public float dynChargedSize
    {
        get
        {
            return weapon_stats[5];
        }
        set
        {
            weapon_stats[5] = value;
        }
    }
    public float dynChargeTime
    {
        get
        {
            return weapon_stats[6];
        }
        set
        {
            weapon_stats[6] = value;
        }
    }

    [Header("Money")]
    public float moneyCount;

    //debug
    [Header("")]
    public bool debug;
    public int selected_stat;

    private void Start()
    {
        //set the names of each stats
        stats_names = new string[] { "Speed", "dashSpeed", "mDamage", "bDamage", "maxCharges", "maxHealth", "luck", "health", "charges"};
        weapon_stats_names = new string[] { "Range", "maxSpeed", "Spin Acc", "RechargeTime", "Size", "ChargedSize", "ChargeTime"};

        //if the stats are not initialized:
        if (stats.Length == 0)
        {
            //create new array to store things
            stats = new float[9];
            weapon_stats = new float[7];

            //stats 
            dynSpeed = 1f;
            dynDashSpeed = 1f;
            dynMeleeDamage = 1f;
            dynBlastDamage = 5f;
            dynMaxCharges = 3f;
            maxHealth = 3f;
            luck = 1f;

            health = 3f;
            charges = 3f;

            moneyCount = 0f;

            //weapon stats
            dynRange = 0f;
            dynMaxSpeed = 0f;
            dynSpinAcceleration = 0f;
            dynRechargeTime = 0f;
            dynSize = 0f;
            dynChargedSize = 0f;
            dynChargeTime = 0f;
        }

        //debug
        debug = false;
        selected_stat = 0;
    }

    private void Update()
    {
        //activates debug stats
        if (Input.GetKeyDown(KeyCode.P))
        {
            debug = !debug;
        }

        //DEBUG STAT CHANGER
        if (debug)
        {
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                if (selected_stat != stats.Length - 1)
                {
                    selected_stat++;
                }
                else
                {
                    selected_stat = 0;
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                if (selected_stat != 0)
                {
                    selected_stat--;
                }
                else
                {
                    selected_stat = stats.Length;
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                if (stats[selected_stat] > 0)
                {
                    stats[selected_stat]--;
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                stats[selected_stat]++;
            }
        }
    }
}

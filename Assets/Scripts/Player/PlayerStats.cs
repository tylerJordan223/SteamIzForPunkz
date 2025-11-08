using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    //handling the visuals and list
    public ItemUI iui;

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
    public float maxRicochets
    {
        get
        {
            return weapon_stats[7];
        }
        set
        {
            weapon_stats[7] = value;
        }
    }

    //maximum stat values;
    private float max_speed = 2.0f;
    private float max_possible_charges = 8f;
    private float max_possible_health = 8f;
    private float max_luck = 5f;
    private float max_spinspeed = 10f;
    private float max_acceleration = 20f;
    private float max_recharge_time = 8f;
    private float max_size; //currently not working
    private float max_charged_size; //currently not working
    private float max_possible_ricochets = 5f;
    private float max_charge_time = 5f;

    //minimum stats
    private float minimum_speed = 0.75f;
    private float minimum_possible_charges = 1f;
    private float minimum_possible_health = 1f;
    private float minimum_luck = 0.5f;
    private float minimum_spinspeed = -2f;
    private float minimum_acceleration = -2f;
    private float minimum_recharge_time = -1.5f;
    private float minimum_size = 0f; //currently not working
    private float minimum_charged_size = 0f; //currently not working
    private float minimum_possible_ricochets = 0f;
    private float minimum_charge_time = -5f;

    //special boss stats
    public float special
    {
        get
        {
            return stats[9];
        }
        set
        {
            stats[9] = value;
        }
    }

    [Header("Money")]
    public float moneyCount;

    //debug
    [Header("")]
    public static bool debug;
    public int selected_stat;

    //additional things
    private List<float> can_negative;

    private void Start()
    {
        //grabbing the UI and initializing the inventory
        iui = GameObject.Find("ItemShowcase").GetComponent<ItemUI>();

        //set the names of each stats
        stats_names = new string[] { "Speed", "dashSpeed", "mDamage", "bDamage", "maxCharges", "maxHealth", "luck", "health", "charges", "special"};
        weapon_stats_names = new string[] { "Range", "maxSpeed", "Spin Acc", "RechargeTime", "Size", "ChargedSize", "ChargeTime", "Ricochets"};

        //if the stats are not initialized:
        if (stats.Length == 0)
        {
            //create new array to store things
            stats = new float[10];
            weapon_stats = new float[8];

            //stats 
            dynSpeed = 1f;
            dynDashSpeed = 1f;
            dynMeleeDamage = 1f;
            dynBlastDamage = 5f;
            dynMaxCharges = 3f;
            maxHealth = 3f;
            luck = 5f;
            special = 0f;

            health = (float)DataManager.playerHealth;
            charges = (float)DataManager.playerCharges;

            moneyCount = (float)DataManager.playerMoney;

            //weapon stats
            dynRange = 0f;
            dynMaxSpeed = 0f;
            dynSpinAcceleration = 0f;
            dynRechargeTime = 0f;
            dynSize = 0f;
            dynChargedSize = 0f;
            dynChargeTime = 0f;
            maxRicochets = 1f;
        }

        //will allow these values to go negative
        can_negative = new List<float>();
        can_negative.Add(dynRechargeTime);
        can_negative.Add(dynChargedSize);
        can_negative.Add(dynChargeTime);

        //debug
        debug = false;
        selected_stat = 0;

        //load inventory from data manager
        LoadInventory();
    }

    private void Update()
    {
        //special stat specifically:
        if(special > 0 && charges < special)
        {
            charges = special;
        }

        //activates debug stats

        /*
        if (Input.GetKeyDown(KeyCode.P))
        {
            debug = !debug;
        }
        */

        //DEBUG STAT CHANGER
        if (debug)
        {
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                if (selected_stat != ((stats.Length) + (weapon_stats.Length -1)))
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
                    selected_stat = ((stats.Length - 1) + (weapon_stats.Length));
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                if(selected_stat > stats.Length-1)
                {
                    if (weapon_stats[selected_stat - stats.Length] > 0)
                    {
                        weapon_stats[selected_stat - stats.Length]--;
                    }
                }
                else
                {

                    if (stats[selected_stat] > 0)
                    {
                        stats[selected_stat]--;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                if (selected_stat > stats.Length - 1)
                {
                    weapon_stats[selected_stat - stats.Length]++;
                }
                else
                {
                    stats[selected_stat]++;
                }
            }

            //RESTARTS GAME!!!
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        
    }

    public void NewItem(ItemScript item)
    {
        //updating all the stats
        dynSpeed += item.speed;
        dynDashSpeed += item.dash_speed;
        dynMeleeDamage += item.melee_damage;
        dynBlastDamage += item.blast_damage;
        if(dynMaxCharges + item.max_charges > 0)
        {
            dynMaxCharges += item.max_charges;
        }
        if(maxHealth + item.max_health > 1f)
        {
            maxHealth += item.max_health;
        }
        luck += item.luck;
        dynRange += item.range_from_player;
        dynMaxSpeed += item.max_spin_speed;
        dynSpinAcceleration += item.spin_acceleration;
        dynRechargeTime += item.time_to_recharge;
        dynChargeTime += item.time_to_charge;
        dynSize += item.weapon_size;
        dynChargedSize += item.size_when_charged;
        maxRicochets += item.max_ricochets;
        special += item.special;
        if(charges + item.charges > 0f)
        {
            charges += item.charges;
        }
        if(health + item.health > 0f)
        {
            health += item.health;
        }

        //scale damages
        if(dynBlastDamage < -2)
        {
            dynBlastDamage = -2;
        }

        if(dynMeleeDamage < 0)
        {
            dynMeleeDamage = 0;
        }

        //check everything and make sure its not in a bad place

        //playerspeed
        if(dynSpeed > max_speed)
        {
            dynSpeed = max_speed;
        }else if(dynSpeed < minimum_speed)
        {
            dynSpeed = minimum_speed;
        }

        //max charges
        if(dynMaxCharges > max_possible_charges)
        {
            dynMaxCharges = max_possible_charges;
        }else if(dynMaxCharges < minimum_possible_charges)
        {
            dynMaxCharges = minimum_possible_charges;
        }

        //max health
        if(maxHealth > max_possible_health)
        {
            maxHealth = max_possible_health;
        }else if(maxHealth < minimum_possible_health)
        {
            maxHealth = minimum_possible_health;
        }

        //luck
        if(luck > max_luck)
        {
            luck = max_luck;
        }else if(luck < minimum_luck)
        {
            luck = minimum_luck;
        }
        
        //rotational speed
        if(dynMaxSpeed > max_spinspeed)
        {
            dynMaxSpeed = max_spinspeed;
        }else if(dynMaxSpeed < minimum_spinspeed)
        {
            dynMaxSpeed = minimum_spinspeed;
        }

        //acceleration
        if(dynSpinAcceleration > max_acceleration)
        {
            dynSpinAcceleration = max_acceleration;
        }else if(dynSpinAcceleration < minimum_acceleration)
        {
            dynSpinAcceleration = minimum_acceleration;
        }

        //time it takes to recharge
        if(dynRechargeTime > max_recharge_time)
        {
            dynRechargeTime = max_recharge_time;
        }else if(dynRechargeTime < minimum_recharge_time)
        {
            dynRechargeTime = minimum_recharge_time;
        }

        //time it takes between chargges
        if(dynChargeTime > max_charge_time)
        {
            dynChargeTime = max_charge_time;
        }else if(dynChargeTime < minimum_charge_time)
        {
            dynChargeTime = minimum_charge_time;
        }

        //ricochets
        if(maxRicochets > max_possible_ricochets)
        {
            maxRicochets = max_possible_ricochets;
        }else if(maxRicochets < minimum_possible_ricochets)
        {
            maxRicochets = minimum_possible_ricochets;
        }


        //adding item to the list of held items
        ItemScript new_item;
        new_item = item;
        DataManager.inventory.Add(new_item);

        //make sure it doesnt overfill
        if(health > maxHealth)
        {
            health = maxHealth;
        }

        if(charges > dynMaxCharges)
        {
            charges = dynMaxCharges;
        }

        //showing the item in the UI
        StartCoroutine(iui.show_item(item));
    }

    //updates the player at the start of each floor
    public void LoadInventory()
    {
        foreach(ItemScript item in DataManager.inventory)
        {
            dynSpeed += item.speed;
            dynDashSpeed += item.dash_speed;
            dynMeleeDamage += item.melee_damage;
            dynBlastDamage += item.blast_damage;
            dynMaxCharges += item.max_charges;
            maxHealth += item.max_health;
            luck += item.luck;
            dynRange += item.range_from_player;
            dynMaxSpeed += item.max_spin_speed;
            dynSpinAcceleration += item.spin_acceleration;
            dynRechargeTime += item.time_to_recharge;
            dynChargeTime += item.time_to_charge;
            dynSize += item.weapon_size;
            dynChargedSize += item.size_when_charged;
            maxRicochets += item.max_ricochets;
            special += item.special;
        }

        //finished loading once the player has all their items
        DataManager.EndLoad();
    }
}

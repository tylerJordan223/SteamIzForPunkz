using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    //player for the sake of health
    PlayerStats pstats;
    private float health;
    private float max_health;

    //images of heart
    [SerializeField] Sprite full_heart;
    [SerializeField] Sprite empty_heart;

    //actual health objects
    private List<Image> hearts;
    [SerializeField] Image first_heart;

    private void Start()
    {
        //find the player
        pstats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        //make the list and get the first heart
        hearts = new List<Image>();
        hearts.Add(this.gameObject.transform.GetChild(0).GetComponent<Image>());
    }

    private void Update()
    {
        //update to get the player's health
        health = pstats.health;
        max_health = pstats.maxHealth;

        //loop to make sure there are enough hearts
        if (hearts.Count < max_health)
        {
            for(int i = 0; i < max_health; i++)
            {
                if (i > hearts.Count-1)
                {
                    //create a new heart
                    GameObject new_heart_go = Instantiate(first_heart).gameObject;
                    //rename it properly
                    new_heart_go.name = "heart" + (i+1).ToString();
                    //parent it properly
                    new_heart_go.transform.SetParent(this.gameObject.transform, false);
                    //set the image
                    Image new_heart = new_heart_go.GetComponent<Image>();
                    //add to the list
                    hearts.Add(new_heart);
                }
            }
        }

        //loop to make sure there aren't too many hearts
        if (hearts.Count > max_health)
        {
            for(int i = hearts.Count-1; i >= 0; i--)
            {
                if(i+1 > max_health)
                {
                    Image destroyed_heart = hearts[i];
                    hearts.Remove(hearts[i]);
                    Destroy(destroyed_heart.gameObject);
                }
            }
        }
         
        //loop to update the heart visuals
        for(int f = 0; f < hearts.Count; f++)
        {
            //if the heart is above the current health then its empty
            if(f+1 > health)
            {
                hearts[f].sprite = empty_heart;
            }
            //if the heart is below or equal to health its full
            else
            {
                hearts[f].sprite = full_heart;
            }
        }
    }
}

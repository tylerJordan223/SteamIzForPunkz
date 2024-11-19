using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class ChargeBar : MonoBehaviour
{
    //player for the sake of dynamics
    private PlayerStats pstats;
    private float charges;
    private float max_charges;

    //images of cells
    [SerializeField] Sprite full_cell;
    [SerializeField] Sprite empty_cell;

    //actual charge objects
    private List<Image> cells;
    [SerializeField] Image first_cell;

    private void Start()
    {
        //try to find the player
        if (GameObject.Find("Tric"))
        {
            pstats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        }
        else
        {
            pstats = null;
        }

        //make the list and get the first cell
        cells = new List<Image>();
        cells.Add(this.gameObject.transform.GetChild(1).GetComponent<Image>());
    }

    private void Update()
    {
        if(pstats != null)
        { 
            //update to get the player's cell count
            charges = pstats.charges;
            max_charges = pstats.dynMaxCharges;

            //loop to make sure there are enough cells 
            if (cells.Count < max_charges)
            {
                for (int i = 0; i < max_charges; i++)
                {
                    if (i > cells.Count - 1)
                    {
                        //create a new cell
                        GameObject new_cell_go = Instantiate(first_cell).gameObject;
                        //rename it properly
                        new_cell_go.name = "Cell" + (i + 1).ToString();
                        //parent it properly
                        new_cell_go.transform.SetParent(this.gameObject.transform, false);
                        new_cell_go.transform.SetSiblingIndex(this.transform.childCount - 2);
                        //set the image
                        Image new_cell = new_cell_go.GetComponent<Image>();
                        //add to the list
                        cells.Add(new_cell);
                    }
                }
            }

            //loop to make sure there aren't too many cells
            if (cells.Count > max_charges)
            {
                for (int i = cells.Count - 1; i >= 0; i--)
                {
                    if (i + 1 > max_charges)
                    {
                        Image destroyed_cell = cells[i];
                        cells.Remove(cells[i]);
                        Destroy(destroyed_cell.gameObject);
                    }
                }
            }

            //loop to update the cell visuals
            for (int f = 0; f < cells.Count; f++)
            {
                //if the cell count is above the current charge count then its empty
                if (f + 1 > charges)
                {
                    cells[f].sprite = empty_cell;
                }
                //if the cell number is below or equal to charges its full
                else
                {
                    cells[f].sprite = full_cell;
                }
            }
        }
        else
        {
            if (GameObject.Find("Tric"))
            {
                pstats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            }
        }
    }
}

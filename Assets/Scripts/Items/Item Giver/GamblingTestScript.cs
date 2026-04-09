using GInput;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GamblingTestScript : MonoBehaviour
{
    //script to handle everything related to the slot machine//

    //info for the spinning of the reels
    [Header("Wheel Information")]
    public float spin_speed;


    private int common_item;
    private int rare_item;
    private int legendary_item;

    private List<int> items_list;

    //tracking information
    private int[,] gambling_grid = new int[4, 3];
    private int[,] visual_grid = new int[4, 3];
    private int[,] full_grid = new int[20, 3];

    //input
    private bool spinning;
    private GameInput input;

    //set the percentage possibilities
    public int percent_common;
    public int percent_rare;
    public int percent_legendary;

    //information
    private float common_won;
    private float rare_won;
    private float legendary_won;
    private float empty_won;
    private float loss_rate;
    public float total_spins;

    private int common_count;
    private int rare_count;
    private int legendary_count;
    private int empty_count;
    
    private void Start()
    {
        spinning = false;

        //intiialize the size of the array
        for (int i = 0; i < gambling_grid.GetLength(1); i++)
        {
            for (int j = 0; j < gambling_grid.GetLength(0); j++)
            {
                //each row is going to be the same
                gambling_grid[j, i] = j;
                visual_grid[j, i] = 0;
            }
        }

        /*
        //prints out the current number values
        Debug.Log($"[{gambling_grid[0, 0]}, {gambling_grid[0, 1]}, {gambling_grid[0, 2]}] \n " +
            $"[{gambling_grid[1, 0]}, {gambling_grid[1, 1]}, {gambling_grid[1, 2]}] " +
            $"\n [{gambling_grid[2, 0]}, {gambling_grid[2, 1]}, {gambling_grid[2, 2]}] " +
            $"\n [{gambling_grid[3, 0]}, {gambling_grid[3, 1]}, {gambling_grid[3, 2]}]");

        //prints the item values
        Debug.Log($"[{visual_grid[0, 0]}, {visual_grid[0, 1]}, {visual_grid[0, 2]}] \n " +
            $"[{visual_grid[1, 0]}, {visual_grid[1, 1]}, {visual_grid[1, 2]}] " +
            $"\n [{visual_grid[2, 0]}, {visual_grid[2, 1]}, {visual_grid[2, 2]}] " +
            $"\n [{visual_grid[3, 0]}, {visual_grid[3, 1]}, {visual_grid[3, 2]}]");
        */

        common_won = 0;
        rare_won = 0;
        legendary_won = 0;
        empty_won = 0;
        loss_rate = 0;

        //get the first items
    }

    private void OnEnable()
    {
        //enable necessary input
        input = new GameInput();
        input.Player.Interact.Enable();
    }

    private void OnDisable()
    {
        //disable necessary input
        input.Player.Interact.Disable();
    }

    private void Update()
    {
        if (input.Player.Interact.WasPressedThisFrame())
        {
            spinning = true;
            RefreshItems();
            BeginRoll();
        }
    }

    //function to initialize the coroutines that spin the slots
    private void BeginRoll()
    {
        common_won = 0;
        rare_won = 0;
        legendary_won = 0;
        loss_rate = 0;
        empty_won = 0;

        //run this for a certain number of tests
        for(int j = 0; j < total_spins; j++)
        {
            //for each reel run the coroutine to spin it
            for (int i = 0; i < 3; i++)
            {
                SpinReel(i);
            }
        }

        //print out all the calculations
        Debug.Log($"WIN RATE: \n" +
                  $"Common Item: {common_won} \n" +
                  $"Rare Item: {rare_won} \n" +
                  $"Legendary Item: {legendary_won} \n" +
                  $"Loss: {loss_rate} \n" +
                  $"Empty Item: {empty_won}");

        //print out percentages
        Debug.Log($"WIN RATE PERCENTAGES: \n" +
                  $"Common Item: {(100f * (common_won / total_spins))} \n" +
                  $"Rare Item: {(100f * (rare_won / total_spins))} \n" +
                  $"Legendary Item: {(100f * (legendary_won / total_spins))} \n" +
                  $"Loss: {(100f * (loss_rate / total_spins))} \n" +
                  $"Empty Item: {(100f * (empty_won / total_spins))}");
    }

    //selects the 3 items per slot
    private void RefreshItems()
    {
        //load in a common/rare item and set price
        common_item = 1;

        rare_item = 2;

        legendary_item = 3;


        //re-populate items list
        items_list = new List<int>();
        items_list.Add(-1);
        items_list.Add(common_item);
        items_list.Add(rare_item);
        items_list.Add(legendary_item);

        //run function to fill out board//
        FillBoard();
    }

    //making a Fisher-Yates Shuffle
    public void Shuffle(List<int> list)
    {
        int slot1 = list.Count;
        while (slot1 > 1)
        {
            //move from top down
            slot1--;
            //get a random slot for the second one
            int slot2 = Random.Range(0, slot1 + 1);
            //tuple swap
            (list[slot2], list[slot1]) = (list[slot1], list[slot2]);
        }
    }

    //function that fills out the board according to likelihood
    private void FillBoard()
    {
        //dont pick random items use percentages//
        
        List<int> items_per_column = new List<int>();

        //common item//
        for(int i = 0; i < percent_common / 5; i++)
        {
            items_per_column.Add(common_item);
        }

        //rare item//
        for (int i = 0; i < percent_rare / 5; i++)
        {
            items_per_column.Add(rare_item);
        }

        //legendary item
        for (int i = 0; i < percent_legendary / 5; i++)
        {
            items_per_column.Add(legendary_item);
        }

        //math to get # of empty items//
        int total_percent = percent_common + percent_rare + percent_legendary;
        int percent_blank = 100 - total_percent;

        for(int i = 0; i < percent_blank / 5; i++)
        {
            items_per_column.Add(0);
        }

        //shuffle the items in each column and populate//
        for (int i = 0; i < full_grid.GetLength(1); i++)
        {
            Shuffle(items_per_column);
            for (int j = 0; j < full_grid.GetLength(0); j++)
            {
                full_grid[j, i] = items_per_column[j];
            }
        }

        //then fill out the board visually
        for (int i = 0; i < visual_grid.GetLength(0); i++)
        {
            for (int j = 0; j < visual_grid.GetLength(1); j++)
            {
                visual_grid[i, j] = full_grid[gambling_grid[i, j], j];
            }
        }

        empty_count = 0;
        common_count = 0;
        rare_count = 0;
        legendary_count = 0;

        //find out how many of each there is
        foreach (int i in full_grid)
        {
            if (i == 0)
            {
                empty_count += 1;
            }
            if (i == common_item)
            {
                common_count += 1;
            }
            if (i == rare_item)
            {
                rare_count += 1;
            }
            if (i == legendary_item)
            {
                legendary_count += 1;
            }
        }

        //print out all the numbers
        Debug.Log($"ITEM COUNTS: \n" +
                  $"Common Item: {common_count} \n" +
                  $"Rare Item: {rare_count} \n" +
                  $"Legendary Item: {legendary_count} \n" +
                  $"Empty Item: {empty_count}");
    }

    #region checks
    //function used to check for 3 in a line horizontal
    private int CheckForSuccessHorizontal(int r, int c)
    {
        //if it gets to the last column then return success//
        if (c == visual_grid.GetLength(1) - 1)
        {
            return full_grid[gambling_grid[r, c], c];
        }
        else if (visual_grid[r, c + 1] == visual_grid[r, c])
        {
            //if the one to the right is the same keep going
            return CheckForSuccessHorizontal(r, c + 1);
        }
        else
        {
            //breaks the chain
            return -1;
        }
    }

    //function used to check for 3 in a line vertical
    private int CheckForSuccessVertical(int r, int c)
    {
        //if it gets to the last row then return success//
        if (r == visual_grid.GetLength(0) - 1)
        {
            return full_grid[gambling_grid[r, c], c];
        }
        else if (visual_grid[r + 1, c] == visual_grid[r, c])
        {
            //if the one to the bottom is the same keep going
            return CheckForSuccessVertical(r + 1, c);
        }
        else
        {
            //breaks the chain
            return -1;
        }
    }

    //function used to check for 3 in a line diagonal down
    private int CheckForSuccessDiagonalDown(int r, int c)
    {
        //if it gets to the last row then return success//
        if (r == visual_grid.GetLength(0) - 1 && c == visual_grid.GetLength(1) - 1)
        {
            return full_grid[gambling_grid[r, c], c];
        }
        else if (visual_grid[r + 1, c + 1] == visual_grid[r, c])
        {
            //if the one to the bottom is the same keep going
            return CheckForSuccessDiagonalDown(r + 1, c + 1);
        }
        else
        {
            //breaks the chain
            return -1;
        }
    }

    //function used to check for 3 in a line diagonal up
    private int CheckForSuccessDiagonalUp(int r, int c)
    {
        //if it gets to the last row then return success//
        if (r == 1 && c == visual_grid.GetLength(1) - 1)
        {
            return full_grid[gambling_grid[r, c], c];
        }
        else if (visual_grid[r - 1, c + 1] == visual_grid[r, c])
        {
            //if the one to the bottom is the same keep going
            return CheckForSuccessDiagonalUp(r - 1, c + 1);
        }
        else
        {
            //breaks the chain
            return -1;
        }
    }
    #endregion checks

    private void SpinReel(int delay)
    {
        float temp = 0f;
        while(temp < (delay + Random.Range(0.1f, 0.3f)))
        {
            temp += Time.deltaTime;
        }

        //starting velocity
        float velocity = 0.1f;
        float timer = 0.0f;

        //STARTING POSITION//
        float position = 16;

        while (timer < 5)
        {
            position -= velocity * spin_speed * Time.deltaTime;

            if (position < -16)
            {
                position = 16 - (position + 16);

                //update the grid
                for (int j = 0; j < gambling_grid.GetLength(0); j++)
                {
                    gambling_grid[j, delay] -= 1;
                    if (gambling_grid[j, delay] == -1)
                    {
                        gambling_grid[j, delay] = 11;
                    }
                }

                //upgrade grid visually
                for (int i = 0; i < visual_grid.GetLength(0); i++)
                {
                    for (int j = 0; j < visual_grid.GetLength(1); j++)
                    {
                        visual_grid[i, j] = full_grid[gambling_grid[i, j], j];
                    }
                }
            }

            //increase velocity overtime
            if (velocity < 1)
            {
                velocity += 0.001f + Random.Range(0.001f, 0.005f); //slight randomness to keep it interesting
            }

            //increase the timer and wait until end of frame
            timer += Time.deltaTime;
        }

        //slowdown until slow enough to do this
        while (velocity > 0.1f)
        {
            position -= velocity * spin_speed * Time.deltaTime;

            if (position < -16)
            {
                //update the grid
                for (int j = 0; j < gambling_grid.GetLength(0); j++)
                {
                    gambling_grid[j, delay] -= 1;
                    if (gambling_grid[j, delay] == -1)
                    {
                        gambling_grid[j, delay] = 11;
                    }
                }

                //upgrade grid visually
                for (int i = 0; i < visual_grid.GetLength(0); i++)
                {
                    for (int j = 0; j < visual_grid.GetLength(1); j++)
                    {
                        visual_grid[i, j] = full_grid[gambling_grid[i, j], j];
                    }
                }

                position = 16 - (position + 16);
            }

            //decrease velocity overtime
            if (velocity > 0.1f)
            {
                velocity -= 0.001f + Random.Range(0.001f, 0.005f);
            }

        }

        position = 16;

        //if its the last wheel stopping then no longer spinning
        if (delay == 2)
        {
            //RUN CODE TO CALCULATE ITEM OUTPUT
            spinning = false;

            //show output//

            /*
            //prints out the current number values
            Debug.Log($"GAMBLING: \n [{gambling_grid[0, 0]}, {gambling_grid[0, 1]}, {gambling_grid[0, 2]}] \n " +
                $"[{gambling_grid[1, 0]}, {gambling_grid[1, 1]}, {gambling_grid[1, 2]}] " +
                $"\n [{gambling_grid[2, 0]}, {gambling_grid[2, 1]}, {gambling_grid[2, 2]}] " +
                $"\n [{gambling_grid[3, 0]}, {gambling_grid[3, 1]}, {gambling_grid[3, 2]}]");

            Debug.Log($"VISUAL: \n [{visual_grid[0, 0]}, {visual_grid[0, 1]}, {visual_grid[0, 2]}] \n " +
                $"[{visual_grid[1, 0]}, {visual_grid[1, 1]}, {visual_grid[1, 2]}] " +
                $"\n [{visual_grid[2, 0]}, {visual_grid[2, 1]}, {visual_grid[2, 2]}] " +
                $"\n [{visual_grid[3, 0]}, {visual_grid[3, 1]}, {visual_grid[3, 2]}]");
            */

            //OUTPUTS//

            List<int> check = new List<int>();

            //horizontal
            for (int i = 1; i < gambling_grid.GetLength(0); i++)
            {
                //check for each spot
                check.Add(CheckForSuccessHorizontal(i, 0));
            }

            //vertical
            for (int i = 0; i < gambling_grid.GetLength(1); i++)
            {
                //check for each spot
                check.Add(CheckForSuccessVertical(1, i));
            }

            //diagonal up
            check.Add(CheckForSuccessDiagonalDown(1, 0));

            //diagonal down
            check.Add(CheckForSuccessDiagonalUp(gambling_grid.GetLength(0) - 1, 0));

            bool won = false;
            bool common = false;
            bool rare = false;
            bool legendary = false;
            bool empty = false;

            foreach (int i in check)
            {
                if (i != -1)
                {
                    //means something was won
                    if(i == 0 && !empty)
                    {
                        empty_won += 1;
                        empty = true;
                    }
                    if (i == common_item && !common)
                    {
                        common_won += 1;
                        won = true;
                        common = true;
                    }
                    if (i == rare_item && !rare)
                    {
                        rare_won += 1;
                        won = true;
                        rare = true;
                    }
                    if (i == legendary_item && !legendary)
                    {
                        legendary_won += 1;
                        won = true;
                        legendary = true;
                    }
                }
            }

            if(!won)
            {
                loss_rate += 1;
            }
        }
    }
}

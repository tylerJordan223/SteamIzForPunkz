using GInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlotScript : MonoBehaviour
{
    //script to handle everything related to the slot machine//

    //info for the spinning of the reels
    [Header("Wheel Information")]
    [SerializeField] List<GameObject> reels = new List<GameObject>();
    [SerializeField] List<Image> images_on_wheel;
    public float spin_speed;

    [Header("Item Possibility Information")]
    [SerializeField] Image common_item_image;
    [SerializeField] GameObject common_item;
    [SerializeField] Image rare_item_image;
    [SerializeField] GameObject rare_item;
    [SerializeField] Image legendary_item_image;
    [SerializeField] GameObject legendary_item;

    [SerializeField] GameObject EMPTY_item;

    private List<GameObject> items_list;

    //tracking information
    private int[,] gambling_grid = new int[4,3];
    private Image[,] visual_grid = new Image[4, 3];
    private GameObject[,] full_grid = new GameObject[12, 3];

    //input
    private bool spinning;
    private GameInput input;

    private void Start()
    {
        spinning = false;

        //intiialize the size of the array
        for(int i = 0; i < gambling_grid.GetLength(1); i++)
        {
            for(int j = 0; j < gambling_grid.GetLength(0); j++)
            {
                //each row is going to be the same
                gambling_grid[j,i] = j;
                visual_grid[j, i] = images_on_wheel[(i*gambling_grid.GetLength(0)) + j];
            }
        }

        //prints out the current number values
        Debug.Log($"[{gambling_grid[0,0]}, {gambling_grid[0, 1]}, {gambling_grid[0, 2]}] \n " +
            $"[{gambling_grid[1, 0]}, {gambling_grid[1, 1]}, {gambling_grid[1, 2]}] " +
            $"\n [{gambling_grid[2, 0]}, {gambling_grid[2, 1]}, {gambling_grid[2, 2]}] " +
            $"\n [{gambling_grid[3, 0]}, {gambling_grid[3, 1]}, {gambling_grid[3, 2]}]");

        //prints the item values
        Debug.Log($"[{visual_grid[0, 0]}, {visual_grid[0, 1]}, {visual_grid[0, 2]}] \n " +
            $"[{visual_grid[1, 0]}, {visual_grid[1, 1]}, {visual_grid[1, 2]}] " +
            $"\n [{visual_grid[2, 0]}, {visual_grid[2, 1]}, {visual_grid[2, 2]}] " +
            $"\n [{visual_grid[3, 0]}, {visual_grid[3, 1]}, {visual_grid[3, 2]}]");

        //get the first items
        RefreshItems();
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
        if(input.Player.Interact.IsPressed() && !spinning)
        {
            spinning = true;
            BeginRoll();
        }
    }

    //function to initialize the coroutines that spin the slots
    private void BeginRoll()
    {
        //for each reel run the coroutine to spin it
        for(int i = 0; i < reels.Count; i++)
        {
            StartCoroutine(SpinReel(reels[i].GetComponent<RectTransform>(), i));
        }
    }

    private void RefreshItems()
    {
        //load in a common/rare item
        common_item = ItemList.instance.GetRandomCommonItem();
        rare_item = ItemList.instance.GetRandomRareItem();

        //20% chance to be legendary and 80% to be Epic
        int chance = Random.Range(0, 10);

        if (chance < 8)
        {
            legendary_item = ItemList.instance.GetRandomEpicItem();
        }
        else
        {
            legendary_item = ItemList.instance.GetRandomLegendaryItem();
        }

        //change the items visually
        common_item_image.sprite = common_item.GetComponent<SpriteRenderer>().sprite;
        rare_item_image.sprite = rare_item.GetComponent<SpriteRenderer>().sprite;
        legendary_item_image.sprite = legendary_item.GetComponent<SpriteRenderer>().sprite;

        //re-populate items list
        items_list = new List<GameObject>();
        items_list.Add(EMPTY_item);
        items_list.Add(common_item);
        items_list.Add(rare_item);
        items_list.Add(legendary_item);

        //run function to fill out board//
        FillBoard();
    }

    //function that fills out the board according to likelihood
    private void FillBoard()
    {
        //for now just pick random items to populate it
        for(int i = 0; i < full_grid.GetLength(0); i++)
        {
            for(int j = 0; j < full_grid.GetLength(1); j++)
            {
                full_grid[i, j] = items_list[Random.Range(0, items_list.Count)];
            }
        }

        //then fill out the board visually
        for(int i = 0; i < visual_grid.GetLength(0); i++)
        {
            for(int j = 0; j < visual_grid.GetLength(1); j++)
            {
                visual_grid[i, j].sprite = full_grid[gambling_grid[i, j] , j].GetComponent<SpriteRenderer>().sprite;
            }
        }
    }

    private IEnumerator SpinReel(RectTransform r, int delay)
    {
        //wait based on delay
        yield return new WaitForSeconds(delay);

        //starting velocity
        float velocity = 0.1f;
        float timer = 0.0f;

        while(timer < 5f)
        {
            Vector3 pos = r.localPosition;
            pos.y -= velocity * spin_speed * Time.deltaTime;
            if(pos.y < -16)
            {
                pos.y = 16 - (pos.y + 16);

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
                        visual_grid[i, j].sprite = full_grid[gambling_grid[i, j], j].GetComponent<SpriteRenderer>().sprite;
                    }
                }
            }

            r.localPosition = pos;
            
            //increase velocity overtime
            if(velocity < 1)
            {
                velocity += 0.001f;
            }

            //increase the timer and wait until end of frame
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //slowdown until slow enough to do this
        while (true)
        {
            Vector3 pos = r.localPosition;
            pos.y -= velocity * spin_speed * Time.deltaTime;
            if (pos.y < -16)
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
                        visual_grid[i, j].sprite = full_grid[gambling_grid[i, j], j].GetComponent<SpriteRenderer>().sprite;
                    }
                }

                //check for end of loop
                if (velocity <= 0.1f)
                {
                    //align properly and break
                    pos.y = 16;
                    r.localPosition = pos;
                    break;
                }

                pos.y = 16 - (pos.y + 16);
            }

            r.localPosition = pos;

            //decrease velocity overtime
            if (velocity > 0.1f)
            {
                velocity -= 0.001f;
            }

            yield return new WaitForEndOfFrame();
        }

        //if its the last wheel stopping then no longer spinning
        if(delay == reels.Count-1)
        {
            //RUN CODE TO CALCULATE ITEM OUTPUT
            spinning = false;

            //show output
            //prints out the current number values
            Debug.Log($"[{gambling_grid[0, 0]}, {gambling_grid[0, 1]}, {gambling_grid[0, 2]}] \n " +
                $"[{gambling_grid[1, 0]}, {gambling_grid[1, 1]}, {gambling_grid[1, 2]}] " +
                $"\n [{gambling_grid[2, 0]}, {gambling_grid[2, 1]}, {gambling_grid[2, 2]}] " +
                $"\n [{gambling_grid[3, 0]}, {gambling_grid[3, 1]}, {gambling_grid[3, 2]}]");
        }

        yield return null;
    }
}

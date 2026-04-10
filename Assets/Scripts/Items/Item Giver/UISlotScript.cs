using GInput;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UISlotScript : MonoBehaviour
{
    //singleton
    public static UISlotScript instance;
    private void Awake()
    {
        if(instance)
        {
            DestroyImmediate(this.gameObject);
        }
        instance = this;
    }

    //script to handle everything related to the slot machine//

    //info for the spinning of the reels
    [Header("Wheel Information")]
    
    [SerializeField] List<GameObject> reels = new List<GameObject>();
    [SerializeField] List<Image> images_on_wheel;
    private List<Image> flash_list;
    public float spin_speed;

    [Header("Item Possibility Information")]
    
    [SerializeField] Image common_item_image;
    [SerializeField] GameObject common_item;
    [SerializeField] Image rare_item_image;
    [SerializeField] GameObject rare_item;
    [SerializeField] Image legendary_item_image;
    [SerializeField] GameObject legendary_item;
    [SerializeField] GameObject EMPTY_item;

    [Header("Price Information")]

    //total price alltogether
    [SerializeField] TextMeshProUGUI total_text;
    private int total;

    //reset price
    [SerializeField] TextMeshProUGUI reset_price_text;
    private int reset_price;

    //common item
    [SerializeField] TextMeshProUGUI common_price_text;
    private int common_price;
    [SerializeField] TextMeshProUGUI common_percent_text;
    private int common_percent;

    //rare item
    [SerializeField] TextMeshProUGUI rare_price_text;
    private int rare_price;
    [SerializeField] TextMeshProUGUI rare_percent_text;
    private int rare_percent;

    //legendary item
    [SerializeField] TextMeshProUGUI legendary_price_text;
    private int legendary_price;
    [SerializeField] TextMeshProUGUI legendary_percent_text;
    private int legendary_percent;

    [Header("Item Options")]

    private List<GameObject> items_list;

    //tracking information
    private int[,] gambling_grid = new int[4,3];
    private Image[,] visual_grid = new Image[4, 3];
    private GameObject[,] full_grid = new GameObject[20, 3];

    //input
    private bool spinning;
    private GameInput input;
    private GamblingMachineScript myMachine;

    //percentage handling
    private int total_percent
    {
        get
        {
            return common_percent + rare_percent + legendary_percent;
        }
    }

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

        //initialize reset price
        reset_price = 5 * DataManager.floorNumber;

        //get the first items
        RefreshItems();

        //disable the canvas
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if(Gamepad.all.Count > 0)
        {
            Debug.Log("controller connected");
        }

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
            deActivateSlots();
        }
    }

    //function to initialize the coroutines that spin the slots
    public void BeginRoll()
    {
        if(total <= PlayerStats.instance.moneyCount && !spinning)
        {
            //cant spin more
            spinning = true;

            PlayerStats.instance.moneyCount -= total;

            //for each reel run the coroutine to spin it
            for (int i = 0; i < reels.Count; i++)
            {
                StartCoroutine(SpinReel(reels[i].GetComponent<RectTransform>(), i));
            }
        }
        else
        {
            //play error sound effect//
        }
    }

    //selects the 3 items per slot
    private void RefreshItems()
    {
        //load in a common/rare item and set price
        common_item = ItemList.instance.GetRandomCommonItem();
        common_price = Random.Range(1, 3);
        common_price_text.text = common_price.ToString("D2");

        rare_item = ItemList.instance.GetRandomRareItem();
        rare_price = Random.Range(3, 5);
        rare_price_text.text = rare_price.ToString("D2");

        //20% chance to be legendary and 80% to be Epic
        int chance = Random.Range(0, 10);

        if (chance < 8)
        {
            legendary_item = ItemList.instance.GetRandomEpicItem();

            //less expensive
            legendary_price = Random.Range(5, 7);
        }
        else
        {
            legendary_item = ItemList.instance.GetRandomLegendaryItem();

            //more expensive
            legendary_price = Random.Range(7, 10);
        }
        //update price visually
        legendary_price_text.text = legendary_price.ToString("D2");

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
        //dont pick random items use percentages//

        List<GameObject> items_per_column = new List<GameObject>();

        //common item//
        for (int i = 0; i < common_percent / 5; i++)
        {
            items_per_column.Add(common_item);
        }

        //rare item//
        for (int i = 0; i < rare_percent / 5; i++)
        {
            items_per_column.Add(rare_item);
        }

        //legendary item
        for (int i = 0; i < legendary_percent / 5; i++)
        {
            items_per_column.Add(legendary_item);
        }

        //math to get # of empty items//
        int total_percent = common_percent + rare_percent + legendary_percent;
        int percent_blank = 100 - total_percent;

        for (int i = 0; i < percent_blank / 5; i++)
        {
            items_per_column.Add(EMPTY_item);
        }

        //for now just pick random items to populate it
        for (int i = 0; i < full_grid.GetLength(1); i++)
        {
            Shuffle(items_per_column);
            for (int j = 0; j < full_grid.GetLength(0); j++)
            {
                full_grid[j, i] = items_per_column[j];
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

    //making a Fisher-Yates Shuffle
    public void Shuffle(List<GameObject> list)
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

    #region checks
    //function used to check for 3 in a line horizontal
    private GameObject CheckForSuccessHorizontal(int r, int c)
    {
        //if its an empty item dont even bother
        if (visual_grid[r, c].sprite == EMPTY_item.GetComponent<SpriteRenderer>().sprite)
        {
            return null;
        }

        //if it gets to the last column then return success//
        if(c == visual_grid.GetLength(1)-1)
        {
            return full_grid[gambling_grid[r,c],c];
        }
        else if (visual_grid[r, c+1].sprite == visual_grid[r,c].sprite)
        {
            //if the one to the right is the same keep going
            return CheckForSuccessHorizontal(r, c + 1);
        }
        else
        {
            //breaks the chain
            return null;
        }
    }

    //function used to check for 3 in a line vertical
    private GameObject CheckForSuccessVertical(int r, int c)
    {
        //if its an empty item dont even bother
        if (visual_grid[r, c].sprite == EMPTY_item.GetComponent<SpriteRenderer>().sprite)
        {
            return null;
        }
        //if it gets to the last row then return success//
        if (r == visual_grid.GetLength(0) - 1)
        {
            return full_grid[gambling_grid[r, c], c];
        }
        else if (visual_grid[r + 1, c].sprite == visual_grid[r, c].sprite)
        {
            //if the one to the bottom is the same keep going
            return CheckForSuccessVertical(r + 1, c);
        }
        else
        {
            //breaks the chain
            return null;
        }
    }

    //function used to check for 3 in a line diagonal down
    private GameObject CheckForSuccessDiagonalDown(int r, int c)
    {
        //if its an empty item dont even bother
        if (visual_grid[r, c].sprite == EMPTY_item.GetComponent<SpriteRenderer>().sprite)
        {
            return null;
        }
        //if it gets to the last row then return success//
        if (r == visual_grid.GetLength(0) - 1 && c == visual_grid.GetLength(1) - 1)
        {
            return full_grid[gambling_grid[r, c], c];
        }
        else if (visual_grid[r + 1, c + 1].sprite == visual_grid[r, c].sprite)
        {
            //if the one to the bottom is the same keep going
            return CheckForSuccessDiagonalDown(r + 1, c + 1);
        }
        else
        {
            //breaks the chain
            return null;
        }
    }

    //function used to check for 3 in a line diagonal up
    private GameObject CheckForSuccessDiagonalUp(int r, int c)
    {
        //if its an empty item dont even bother
        if (visual_grid[r, c].sprite == EMPTY_item.GetComponent<SpriteRenderer>().sprite)
        {
            return null;
        }
        //if it gets to the last row then return success//
        if (r == 1 && c == visual_grid.GetLength(1) - 1)
        {
            return full_grid[gambling_grid[r, c], c];
        }
        else if (visual_grid[r - 1, c + 1].sprite == visual_grid[r, c].sprite)
        {
            //if the one to the bottom is the same keep going
            return CheckForSuccessDiagonalUp(r - 1, c + 1);
        }
        else
        {
            //breaks the chain
            return null;
        }
    }
    #endregion checks

    private IEnumerator SpinReel(RectTransform r, int delay)
    {
        //wait based on delay
        yield return new WaitForSeconds(delay + Random.Range(0.1f, 0.3f));

        //starting velocity
        float velocity = 0.1f;
        float timer = 0.0f;

        while(timer < (5 + delay + Random.Range(1f, 2f)))
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
                        gambling_grid[j, delay] = full_grid.GetLength(0) - 1;
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
                velocity += 0.001f + Random.Range(0.001f, 0.005f); //slight randomness to keep it interesting
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
                        gambling_grid[j, delay] = gambling_grid[j, delay] = full_grid.GetLength(0) - 1;
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
                velocity -= 0.001f + Random.Range(0.001f, 0.005f);
            }

            yield return new WaitForEndOfFrame();
        }

        //if its the last wheel stopping then no longer spinning
        if(delay == reels.Count-1)
        {
            //show output//

            //prints out the current number values
            /*
            Debug.Log($"[{gambling_grid[0, 0]}, {gambling_grid[0, 1]}, {gambling_grid[0, 2]}] \n " +
                $"[{gambling_grid[1, 0]}, {gambling_grid[1, 1]}, {gambling_grid[1, 2]}] " +
                $"\n [{gambling_grid[2, 0]}, {gambling_grid[2, 1]}, {gambling_grid[2, 2]}] " +
                $"\n [{gambling_grid[3, 0]}, {gambling_grid[3, 1]}, {gambling_grid[3, 2]}]");

            Debug.Log($"[{visual_grid[0, 0].sprite}, {visual_grid[0, 1].sprite}, {visual_grid[0, 2].sprite}] \n " +
                $"[{visual_grid[1, 0].sprite}, {visual_grid[1, 1].sprite}, {visual_grid[1, 2].sprite}] " +
                $"\n [{visual_grid[2, 0].sprite}, {visual_grid[2, 1].sprite}, {visual_grid[2, 2].sprite}] " +
                $"\n [{visual_grid[3, 0].sprite}, {visual_grid[3, 1].sprite}, {visual_grid[3, 2].sprite}]");
            */
            //OUTPUTS//

            List<GameObject> check = new List<GameObject>();
            GameObject item = null;

            //horizontal
            for(int i = 1; i < gambling_grid.GetLength(0); i++)
            {
                //check for each spot
                item = CheckForSuccessHorizontal(i, 0);
                if (item != null)
                {
                    check.Add(item);
                }
            }

            if (check.Count > 0)
            {
                Debug.Log("Horizontal: " + string.Join(", ", check.Select(x => x.name.ToString())));
            }

            //vertical
            for (int i = 0; i < gambling_grid.GetLength(1); i++)
            {
                //check for each spot
                item = CheckForSuccessVertical(1, i);
                if(item != null)
                {
                    check.Add(item);
                }
            }

            if (check.Count > 0)
            {
                Debug.Log("Vertical: " + string.Join(", ", check.Select(x => x.name.ToString())));
            }

            //diagonal up
            item = CheckForSuccessDiagonalDown(1, 0);
            if (item != null)
            {
                check.Add(item);
            }

            if (check.Count > 0)
            {
                Debug.Log("DDown: " + string.Join(", ", check.Select(x => x.name.ToString())));
            }

            //diagonal down
            item = CheckForSuccessDiagonalUp(gambling_grid.GetLength(0) - 1, 0);
            if (item != null)
            {
                check.Add(item);
            }

            if (check.Count > 0)
            {
                Debug.Log("DUp: " + string.Join(", ", check.Select(x => x.name.ToString())));
            }

            yield return new WaitForSeconds(3); //small delay before spawning items

            //send to machine
            if(check.Count > 0)
            {
                Debug.Log(string.Join(", ", check.Select(x => x.name.ToString())));
                myMachine.PopulateItems(check);
                //end spinning if necessayr
                deActivateSlots();
            }

            //now finished so end spinning
            spinning = false;
        }

        yield return null;
    }

    #region token code

    //common functions
    public void RaiseCommonPercent()
    {
        //maxes out at 10
        if(common_percent < 100 && total_percent < 100 && !spinning)
        {
            common_percent += 5;
            common_percent_text.text = common_percent.ToString("D3");
            UpdateTotal(common_price);
        }
    }

    public void LowerCommonPercent()
    {
        //cant go negative
        if(common_percent > 0 && !spinning)
        {
            common_percent -= 5;
            common_percent_text.text = common_percent.ToString("D3");
            UpdateTotal(-common_price);
        }
    }

    //rare functions
    public void RaiseRarePercent()
    {
        //maxes out at 10
        if (rare_percent < 100 && total_percent < 100 && !spinning)
        {
            rare_percent += 5;
            rare_percent_text.text = rare_percent.ToString("D3");
            UpdateTotal(rare_price);
        }
    }

    public void LowerRarePercent()
    {
        //cant go negative
        if (rare_percent > 0 && !spinning)
        {
            rare_percent -= 5;
            rare_percent_text.text = rare_percent.ToString("D3");
            UpdateTotal(-rare_price);
        }
    }

    //legendary functions
    public void RaiseLegendaryPercent()
    {
        //maxes out at 10
        if (legendary_percent < 100 && total_percent < 100 && !spinning)
        {
            legendary_percent += 5;
            legendary_percent_text.text = legendary_percent.ToString("D3");
            UpdateTotal(legendary_price);
        }
    }

    public void LowerLegendaryPercent()
    {
        //cant go negative
        if (legendary_percent > 0 && !spinning)
        {
            legendary_percent -= 5;
            legendary_percent_text.text = legendary_percent.ToString("D3");
            UpdateTotal(-legendary_price);
        }
    }

    //update the total with amounts
    private void UpdateTotal(int amount)
    {
        total += amount;
        total_text.text = total.ToString("D3");
        FillBoard(); //reset the board
    }

    #endregion token code

    public void ResetItems()
    {
        //make sure you have the money for it
        if(reset_price <= PlayerStats.instance.moneyCount && !spinning)
        {
            //play sfx, subtract money, and increase reset price
            AudioManager.instance.PlaySingleSFX(AudioManager.instance.buttonpress);
            PlayerStats.instance.moneyCount -= reset_price;
            reset_price += 5;
            reset_price_text.text = reset_price.ToString("D2");
            RefreshItems();
        }
        else
        {
            //play error sound effect//
        }
    }

    //code to affect the UI itself//
    public void activateSlots(GamblingMachineScript machine)
    {
        this.gameObject.SetActive(true);

        //disable player movement
        PlayerScript.instance.canControl = false;

        myMachine = machine;
    }

    public void deActivateSlots()
    {
        myMachine.opened = false;
        PlayerScript.instance.canControl = true;
        this.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    private void Awake()
    {
        if(instance)
        {
            DestroyImmediate(this.gameObject);
        }
        instance = this;
    }

    /*
     * THIS SCRIPT IS FOR MANAGING EVERYTHING THAT STAYS BETWEEN SCENES!!!
     * 
     * This means that the map, health, items, and everything else are included here.
     */

    public static Grid g;

    public static int playerHealth = 3;
    public static int playerCharges = 3;
    public static int playerMoney = 0;
    public static int floorNumber = 0;
    public static List<ItemScript> inventory = new List<ItemScript>();
    public static bool playing = false;

    private static GameObject loadingScreen;
    private static Animator anim;

    public static List<GameObject> items;

    #region initialization
    // CHECKING TO MAKE SURE THERE ARE NOT DUPLICATES OF THIS OBJECT! //
    private static GameObject dataManager_instance;

    //check before anything is run to make sure there isnt one of these already
    private void Start()
    {
        //making sure this doesnt double
        if (dataManager_instance != null && dataManager_instance != this.gameObject)
        {
            Destroy(this.gameObject);
        }
        else
        {
            dataManager_instance = this.gameObject;
            //makes sure this stays
            DontDestroyOnLoad(this.gameObject);
            //load the item list
            items = GetComponent<ItemList>().getList();
        }
    }
    #endregion

    /***********************************************/

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void StartLoad()
    {
        playerHealth = (int)GameObject.Find("Tric").GetComponent<PlayerStats>().health;
        playerCharges = (int)GameObject.Find("Tric").GetComponent<PlayerStats>().charges;
        playerMoney = (int)GameObject.Find("Tric").GetComponent<PlayerStats>().moneyCount;
        playing = false;
        anim.SetBool("loading", true);
    }

    public static void StartRunFromSave()
    {
        //make sure the is a load to save
        if(SaveFileScript.LoadFloorNumber() != -1)
        {
            playerHealth = SaveFileScript.LoadHealth();
            playerCharges = SaveFileScript.LoadCharges();
            playerMoney = SaveFileScript.LoadCharges();
            SceneManager.LoadScene("MainFloor");
        }
        else
        {
            RunStart();
        }
    }

    public void EndLoad()
    {
        anim.SetBool("loading", false);

        //if there is no floor generator at the end of a load then generate a base grid
        if (!GameObject.Find("FloorGenerator"))
        {
            Debug.Log("running");
            g = new Grid(300, 300, 1);
            g.checkGrid();
            g.playerNode = g.getNode(PlayerStats.instance.transform);
        }

    }

    public static void LoadNextFloor()
    {
        floorNumber++;
        if(floorNumber == 3)
        {
            SceneManager.LoadScene("BossRoom");
        }
        else
        {
            SceneManager.LoadScene("MainFloor");
        }
    }

    public static void RunStart()
    {
        AudioManager.instance.PlaySingleSFX(AudioManager.instance.buttonpress);
        playerHealth = 3;
        playerCharges = 3;
        playerMoney = 0;
        floorNumber = 0;
        inventory = new List<ItemScript>();
        SceneManager.LoadScene("MainFloor");
    }

    public static void QuitGame()
    {
        AudioManager.instance.PlaySingleSFX(AudioManager.instance.buttonpress);
        //only make a save if its worth saving
        if (floorNumber > 0)
        {
            //SaveFileScript.SaveFile(floorNumber, playerHealth, playerCharges, playerMoney, inventory);
        }

        //save audio
        SaveFileScript.SaveAudio();

        Application.Quit();
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu")
        {
            loadingScreen = GameObject.Find("LoadingScreen");
            if (loadingScreen)
            {
                anim = loadingScreen.GetComponent<Animator>();
                anim.SetBool("loading", true);
            }
        }
    }
}

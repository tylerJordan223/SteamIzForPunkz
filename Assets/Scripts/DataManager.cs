using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    /*
     * THIS SCRIPT IS FOR MANAGING EVERYTHING THAT STAYS BETWEEN SCENES!!!
     * 
     * This means that the map, health, items, and everything else are included here.
     */

    public static int playerHealth = 3;
    public static int playerCharges = 3;
    public static int playerMoney = 0;
    public static int floorNumber = 0;
    public static List<ItemScript> inventory = new List<ItemScript>();
    public static bool playing = false;

    private static GameObject loadingScreen;
    private static Animator anim;

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
        }
    }
    #endregion

    /***********************************************/

    private void Awake()
    {
        loadingScreen = GameObject.Find("LoadingScreen");
        anim = loadingScreen.GetComponent<Animator>();
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            anim.SetBool("loading", false);
        }
        else
        {
            anim.SetBool("loading", true);
        }
    }

    public static void StartLoad()
    {
        playerHealth = (int)GameObject.Find("Tric").GetComponent<PlayerStats>().health;
        playerCharges = (int)GameObject.Find("Tric").GetComponent<PlayerStats>().charges;
        playerMoney = (int)GameObject.Find("Tric").GetComponent<PlayerStats>().moneyCount;
        playing = false;
        anim.SetBool("loading", true);
    }

    public static void EndLoad()
    {
        anim.SetBool("loading", false);
    }

    public static void LoadNextFloor()
    {
        floorNumber++;
        SceneManager.LoadScene("MainFloor");
    }

    public static void RunStart()
    {
        playerHealth = 3;
        playerCharges = 3;
        playerMoney = 0;
        floorNumber = 0;
        SceneManager.LoadScene("MainFloor");
    }

    public static void QuitGame()
    {
        Application.Quit();
    }
}

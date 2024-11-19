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
    public static int floorNumber = 0;
    public static List<ItemScript> inventory = new List<ItemScript>();

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
        anim.SetBool("loading", true);
    }

    public static void StartLoad()
    {
        playerHealth = (int)GameObject.Find("Tric").GetComponent<PlayerStats>().health;
        playerCharges = (int)GameObject.Find("Tric").GetComponent<PlayerStats>().charges;
        anim.SetBool("loading", true);
    }


    public static void EndLoad()
    {
        anim.SetBool("loading", false);
    }

    public static void LoadFloor()
    {
        floorNumber++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

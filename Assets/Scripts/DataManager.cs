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

    #region instanceManager
    // CHECKING TO MAKE SURE THERE ARE NOT DUPLICATES OF THIS OBJECT! //
    private static GameObject dataManager_instance;

    //check before anything is run to make sure there isnt one of these already
    private void Awake()
    {
        if(dataManager_instance != null && dataManager_instance != this.gameObject)
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

    private void Start()
    {

    }

    private void Update()
    {
    }


}

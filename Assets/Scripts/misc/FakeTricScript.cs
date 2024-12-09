using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeTricScript : MonoBehaviour
{
    //script to handle the fake player that is used for some animations

    public void NextLevel()
    {
        DataManager.StartLoad();
    }
}

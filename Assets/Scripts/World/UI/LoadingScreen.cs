using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public void Transition()
    {
        DataManager.LoadNextFloor();
    }
    public void Pausable()
    {
        DataManager.playing = true;
    }
    public void TriggerSpawnPoint()
    {
        GameObject.Find("SpawnPoint").GetComponent<SpawnScript>().StartSpawn();
    }
}

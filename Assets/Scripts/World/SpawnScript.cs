using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    [SerializeField] Animator anim;

    public void StartSpawn()
    {
        anim.SetBool("spawning", true);
    }

    public void EndSpawn()
    {
        GameObject.Find("Tric").GetComponent<PlayerScript>().canControl = true;
        GameObject.Find("Tric").transform.Find("Sprites").GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 10;
        GameObject.Find("Tric").transform.Find("Sprites").GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = 10;
    }

}

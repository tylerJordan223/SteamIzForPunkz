using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossItemScript : MonoBehaviour
{
    //script who's entire purpose is to lower the boss death items

    ItemScript item_s;

    public Vector3 goal_position;

    private void Start()
    {
        item_s = GetComponent<ItemScript>();
        item_s.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void Update()
    {
        if (transform.position.y > goal_position.y+0.5)
        {
            transform.position = Vector3.Lerp(transform.position, goal_position, 1f*Time.deltaTime);
        }
        else
        {
            item_s.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}

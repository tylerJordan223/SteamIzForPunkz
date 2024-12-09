using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private Animator d_anim;
    public Room my_room;

    public bool open;
    public bool wall;

    private void Start()
    {
        d_anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if(wall)
        {
            d_anim.SetBool("wall", true);
        }
        else
        {
            if(open)
            {
                d_anim.SetBool("open", true);
            }
            else
            {
                d_anim.SetBool("open", false);
            }
        }
    }

}

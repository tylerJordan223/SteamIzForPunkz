using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disappearScript: MonoBehaviour
{
    private Material my_mat;

    private bool dissolving;
    private float fade;

    private void Start()
    {
        //getting hte material attached to the item
        my_mat = GetComponent<SpriteRenderer>().material;

        fade = 1f;
        dissolving = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            dissolving = !dissolving;
        }

        if (dissolving && fade > 0f)
        {
            fade -= Time.deltaTime;

            if (fade <= 0f)
            {
                fade = 0f;
            }
        }
        else if(fade < 1f)
        {
            fade += Time.deltaTime;

            if(fade >= 1f)
            {
                fade = 1f;
            }
        }

        my_mat.SetFloat("_Fade", fade);
    }
}

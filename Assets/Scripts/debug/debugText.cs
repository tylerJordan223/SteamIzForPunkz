using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class debugText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    PlayerScript p;

    // Update is called once per frame
    void Update()
    {
        GameObject go = GameObject.Find("Tric");
        if (go)
        {
            p = go.GetComponent<PlayerScript>();
            text.text = p.currentRoom.name;
        }

    }
}

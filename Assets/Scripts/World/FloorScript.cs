using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorScript : MonoBehaviour
{
    GameObject virtualCamera;
    GameObject parentRoom;

    private void Start()
    {
        //gets the whole room object to be worked with
        parentRoom = this.transform.parent.parent.gameObject;
        //find the roomcam in the room
        virtualCamera = parentRoom.transform.Find("RoomCam").gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCamera.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCamera.SetActive(false);
        }
    }
}

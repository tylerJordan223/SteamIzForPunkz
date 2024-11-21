using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorScript : MonoBehaviour
{
    public GameObject virtualCamera;
    GameObject parentRoom;

    private void Start()
    {
        //gets the whole room object to be worked with
        parentRoom = this.transform.parent.parent.gameObject;
        //find the roomcam in the room
        virtualCamera = parentRoom.transform.Find("RoomCam").gameObject;
        virtualCamera.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCamera.SetActive(true);
            parentRoom.GetComponent<RoomScript>().active = true;
            if (parentRoom.GetComponent<RoomScript>().enemy_count > 0)
            {
                parentRoom.GetComponent<RoomScript>().locked = true;
                parentRoom.GetComponent<RoomScript>().my_room.CloseNeighbors();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            parentRoom.GetComponent<RoomScript>().active = false;
            if(parentRoom.GetComponent<RoomScript>().locked)
            {
                parentRoom.GetComponent<RoomScript>().locked = false;
                parentRoom.GetComponent<RoomScript>().my_room.OpenNeighbors();
            }
            virtualCamera.SetActive(false);
        }
    }
}

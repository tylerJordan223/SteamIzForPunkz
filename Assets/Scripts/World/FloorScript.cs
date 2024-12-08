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

    /*
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
    */

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            parentRoom.GetComponent<RoomScript>().active = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //only activates the room if it has enemies, and the player is not standing on a door
        if(collision.CompareTag("Player") && !collision.isTrigger)
        {
            if(!DataManager.g.playerNode.isDoor)
            {
                //quick check to set teh first camera
                if(collision.GetComponent<PlayerScript>().currentCamera == null)
                {
                    collision.GetComponent<PlayerScript>().currentCamera = virtualCamera;
                    collision.GetComponent<PlayerScript>().currentCamera.SetActive(true);
                }

                //cycle the cameras
                if(collision.GetComponent<PlayerScript>().currentCamera != virtualCamera)
                {
                    virtualCamera.SetActive(true);
                    collision.GetComponent<PlayerScript>().currentCamera.SetActive(false);
                    collision.GetComponent<PlayerScript>().currentCamera = virtualCamera;
                }
                
                if (parentRoom.GetComponent<RoomScript>().can_be_locked)
                {
                    parentRoom.GetComponent<RoomScript>().active = true;
                    if (parentRoom.GetComponent<RoomScript>().enemy_count > 0)
                    {
                        parentRoom.GetComponent<RoomScript>().locked = true;
                        parentRoom.GetComponent<RoomScript>().my_room.CloseDoors();
                        parentRoom.GetComponent<RoomScript>().my_room.CloseNeighbors();
                    }
                }
            }
        }
    }
}

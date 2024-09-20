using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    GameObject virtualCamera;

    private void Start()
    {
        virtualCamera = this.transform.Find("RoomCam").gameObject;
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

public class Room
{
    //actual room
    public int room_width;
    public int room_height;
    public GameObject room;
    private WorldNode centerNode;
    private Grid g;
    private List<GameObject> enemies;
    private List<WorldNode> nodes;
    private List<GameObject> doors;

    //position in floor
    public int floorposx;
    public int floorposy;
    
    //all that is needed is the room object itself and the grid it resides on
    public Room(GameObject r, Grid g_, int fpx, int fpy)
    {
        //set the position on the floor for generation
        floorposx = fpx;
        floorposy = fpy;

        //create the lists I need
        nodes = new List<WorldNode>();
        enemies = new List<GameObject>();
        doors = new List<GameObject>();

        //set room
        room = r;

        //set grid
        g = g_;

        //room height and width, only one size as of right now
        room_width = 19;
        room_height = 11;

        //get and add doors to list
        Transform allDoors = r.transform.GetChild(0);
        foreach(Transform d in allDoors)
        {
            doors.Add(d.gameObject);
        }

    }

    public void GenerateNodes()
    {
        //get the node at the center of the room
        centerNode = g.getNode(room.transform);


        //startx and starty for the calculations
        int nodex = centerNode.x - ((room_width - 1) / 2);
        int nodey = centerNode.y - ((room_height - 1) / 2);

        //get all the nodes
        for (int i = 0; i < room_width - 1; i++)
        {
            for (int j = 0; j < room_height - 1; j++)
            {
                nodes.Add(g.getNode(nodex + i, nodey + j));
            }
        }

    }

    //check for any neighbors on the room
    public int getNeighbors()
    {
        int neighbors = 0;

        //check all sides of each door to see if it finds another door
        foreach (GameObject door in doors)
        {
            //get the node for the current door
            WorldNode tempNode = g.getNode(door.transform);

            //check all 4 sides to see if theres a door
            if(g.getNode(tempNode.x + 1, tempNode.y).isDoor)
            {
                neighbors++;
            }
            if (g.getNode(tempNode.x - 1, tempNode.y).isDoor)
            {
                neighbors++;
            }
            if (g.getNode(tempNode.x, tempNode.y + 1).isDoor)
            {
                neighbors++;
            }
            if (g.getNode(tempNode.x, tempNode.y - 1).isDoor)
            {
                neighbors++;
            }

            return neighbors;
        }
        return 0;
    }
}


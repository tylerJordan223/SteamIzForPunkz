using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    public Room my_room;
    public int enemy_count;
    public bool locked;
    public bool active;

    private void Start()
    {
        enemy_count = 0;
        //get the amount of enemies in the room, and make sure they know that this is theri parent room
        for(int i = 0; i < transform.Find("Enemies").childCount; i++)
        {
            enemy_count++;
            transform.Find("Enemies").GetChild(i).gameObject.GetComponent<EnemyHealth>().my_room = this;
        }
        locked = false;
        active = false;
    }

    private void Update()
    {
        if(enemy_count == 0 && locked)
        {
            my_room.OpenDoors();
            locked = false;
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
    private Room parent;

    //special rooms
    private bool spawn;

    //position in floor
    public int floorposx;
    public int floorposy;
    
    //all that is needed is the room object itself and the grid it resides on
    public Room(GameObject r, Grid g_, Room p, int fpx, int fpy)
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

        //set parent, and if parent was null then that means this is the spawn room
        if(p == null)
        {
            spawn = true;
        }
        else
        {
            spawn = false;
            parent = p;
        }

        //set grid
        g = g_;

        //room height and width, only one size as of right now
        room_width = 19;
        room_height = 11;

        //get the doors on initialization
        Transform allDoors = room.transform.Find("Doors");
        foreach (Transform d in allDoors)
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
        }
        return neighbors;
    }

    public void UpdateDoors()
    {
        //get and add doors to list before checking them if necessary
        //this is for special rooms that are generated after the rest, and require the doors to be re-added
        if(doors.Count == 0)
        {
            Transform allDoors = room.transform.Find("Doors");
            foreach (Transform d in allDoors)
            {
                doors.Add(d.gameObject);
            }
        }

        foreach (GameObject door in doors)
        {
            //get the node for the current door
            WorldNode tempNode = g.getNode(door.transform);

            //check all 4 sides to see if theres a door, and if there is then change sprite to clear and disable collision
            if ((g.getNode(tempNode.x + 1, tempNode.y).isDoor) || (g.getNode(tempNode.x - 1, tempNode.y).isDoor) || (g.getNode(tempNode.x, tempNode.y + 1).isDoor) || (g.getNode(tempNode.x, tempNode.y - 1).isDoor))
            {
                door.GetComponent<SpriteRenderer>().color = Color.clear;
                door.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    //function to delete doors so that new ones can be added
    public void RemoveDoors()
    {
        doors = new List<GameObject>();
    }

    //opens doors
    public void OpenDoors()
    {
        foreach (GameObject door in doors)
        {
            door.GetComponent<SpriteRenderer>().color = Color.clear;
            door.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    //closes doors
    public void CloseDoors()
    {
        foreach (GameObject door in doors)
        {
            door.GetComponent<SpriteRenderer>().color = Color.white;
            door.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    //recursive function to get distance from spawn
    public int DistanceFromSpawn()
    {
        if(spawn)
        {
            return 1;
        }
        else
        {
            return parent.DistanceFromSpawn() + 1;
        }
    }
}


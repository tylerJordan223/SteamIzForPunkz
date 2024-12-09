using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomScript : MonoBehaviour
{
    public Room my_room;
    public int enemy_count;
    public bool can_be_locked;
    public bool locked;
    public bool active;

    private void Start()
    {
        enemy_count = 0;
        //get the amount of enemies in the room, and make sure they know that this is theri parent room
        for(int i = 0; i < transform.Find("Enemies").childCount; i++)
        {
            enemy_count++;
            if(transform.Find("Enemies").GetChild(i).CompareTag("enemy"))
            {
                transform.Find("Enemies").GetChild(i).gameObject.GetComponent<EnemyHealth>().my_room = this;
            }
        }
        locked = false;
        active = false;

        if(enemy_count >= 1)
        {
            can_be_locked = true;
        }
        else
        {
            can_be_locked = false;
        }

        //ONLY DO THIS FOR SPAWN BECAUSE OF BOSSES
        if(SceneManager.GetActiveScene().name == "BossRoom" && gameObject.name == "SpawnRoom")
        {
            my_room = new Room(gameObject, DataManager.g, null, 0, 0);
        }
    }

    private void Update()
    {
        if(enemy_count == 0 && locked)
        {
            my_room.OpenDoors();
            my_room.OpenNeighbors();
            locked = false;
            can_be_locked = false;
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
            d.GetComponent<DoorScript>().my_room = this;
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
    
    public List<Room> getNeighborRooms()
    {
        List<Room> r = new List<Room>();
        foreach (GameObject door in doors)
        {
            //get the node for the current door
            if(!g.getNode(door.transform).IsUnityNull())
            {
                WorldNode tempNode = g.getNode(door.transform);

                //check all 4 sides to see if theres a door, and if there is, add the room that the door is in to the script
                if (g.getNode(tempNode.x + 1, tempNode.y).isDoor)
                {
                    r.Add(g.getRoomGameObjectAtNode(g.getNode(tempNode.x + 1, tempNode.y)).GetComponent<RoomScript>().my_room);
                }
                if (g.getNode(tempNode.x - 1, tempNode.y).isDoor)
                {
                    r.Add(g.getRoomGameObjectAtNode(g.getNode(tempNode.x - 1, tempNode.y)).GetComponent<RoomScript>().my_room);
                }
                if (g.getNode(tempNode.x, tempNode.y + 1).isDoor)
                {
                    r.Add(g.getRoomGameObjectAtNode(g.getNode(tempNode.x, tempNode.y + 1)).GetComponent<RoomScript>().my_room);
                }
                if (g.getNode(tempNode.x, tempNode.y - 1).isDoor)
                {
                    r.Add(g.getRoomGameObjectAtNode(g.getNode(tempNode.x, tempNode.y - 1)).GetComponent<RoomScript>().my_room);
                }
            }
        }
        return r;
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
                d.GetComponent<DoorScript>().my_room = this;
            }
        }

        foreach (GameObject door in doors)
        {
            //get the node for the current door
            WorldNode tempNode = g.getNode(door.transform);

            //check all 4 sides to see if theres a door, and if there is then change sprite to clear and disable collision
            if ((g.getNode(tempNode.x + 1, tempNode.y).isDoor) || (g.getNode(tempNode.x - 1, tempNode.y).isDoor) || (g.getNode(tempNode.x, tempNode.y + 1).isDoor) || (g.getNode(tempNode.x, tempNode.y - 1).isDoor))
            {
                door.GetComponent<DoorScript>().open = true;
                door.GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                //if there is no room through a door it becomes a wall
                door.GetComponent<DoorScript>().wall = true;
                door.GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }

    public void UpdateNeighbors()
    {
        List<Room> neighbors = getNeighborRooms();
        foreach (Room r in neighbors)
        {
            r.UpdateDoors();
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
            door.GetComponent<BoxCollider2D>().enabled = false;
            door.GetComponent<DoorScript>().open = true;
        }
    }

    //closes doors
    public void CloseDoors()
    {
        foreach (GameObject door in doors)
        {
            door.GetComponent<BoxCollider2D>().enabled = true;
            door.GetComponent<DoorScript>().open = false;
        }
    }

    //closes the doors of all the rooms around the current room
    public void CloseNeighbors()
    {
        //get the list of neighboring rooms
        List<Room> temp_rooms = getNeighborRooms();
        //close the doors in all of the neighboring rooms
        foreach(Room r in temp_rooms)
        {
            r.CloseDoors();
        }
    }

    //opens the doors of all rooms around the current room
    public void OpenNeighbors()
    {
        //get the list of neighboring rooms
        List<Room> temp_rooms = getNeighborRooms();
        //opens the doors in all of the neighboring rooms
        foreach (Room r in temp_rooms)
        {
            r.OpenDoors();
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


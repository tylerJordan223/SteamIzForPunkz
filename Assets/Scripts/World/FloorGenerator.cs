using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorGenerator : MonoBehaviour
{
    //Script to randomly generator floors

    //collection of rooms, to be filled with some sort of list of possible rooms. If possible just grab the folder of room prefabs
    public List<GameObject> room_list = new List<GameObject>();
    [SerializeField] GameObject player;
    [SerializeField] GameObject floor_parent;

    //booleans
    private bool isStarted;

    //Number variables
    public int maxRooms;
    public int minRooms;
    private int roomCount;
    private int roomRandom;

    //Data Structures
    private List<Room> rooms;
    private int[,] floorplan;
    private Queue<Room> roomQueue;

    //GRID TO BE USED BY OTHER SCRIPTS//
    public Grid g;

    private void Start()
    {
        //decompose to generate grid
        g = new Grid(maxRooms * 19, maxRooms * 11, 1);

        //make the roomrandomvariable that will make rooms less likely overtime
        roomRandom = 0;

        //mark as started
        isStarted = true;

        //create the list of rooms
        rooms = new List<Room>();

        //creates the floor
        CreateFloor();
    }

    private void Update()
    {
        if (isStarted)
        {
            if(roomQueue.Count > 0)
            {
                Room room = roomQueue.Dequeue();
                if(room.floorposx > 1) { createRoom(room.floorposx-1, room.floorposy); }
                if (room.floorposx < maxRooms - 1) { createRoom(room.floorposx+1, room.floorposy); }
                if (room.floorposy > 1) { createRoom(room.floorposx, room.floorposy-1); }
                if (room.floorposy < maxRooms - 1) { createRoom(room.floorposx, room.floorposy+1); }
                if (roomRandom < 6)
                {
                    roomRandom++;
                }
                //re-enqueue the room if its not min rooms yet
                if (roomCount < minRooms || Random.Range(0,10) <= (9 - + roomRandom)) {
                    roomQueue.Enqueue(room);
                }
            }
            //all code that needs to be run at the end of a run
            if(roomQueue.Count == 0)
            {
                //stop the loop
                Debug.Log("GENERATION FINISHED!!!");
                Debug.Log("Floor Room Count: " + roomCount);
                isStarted = false;

                //update the grid to find all the doors
                g.checkGrid();

                //do the door check on all of the rooms
                foreach (Room r in rooms)
                {
                    r.UpdateDoors();
                }

                //spawn player and move it to the first room
                GameObject p = Instantiate(player);
                p.name = "Trice";
                p.transform.position = rooms[0].room.transform.position;
            }
        }
    }

    private void CreateFloor()
    {
        //create the floor plan and set every int to 0
        //the floorplan is a 2d array recreation of the actual floor
        floorplan = new int[maxRooms, maxRooms];
        for (int i = 0; i < maxRooms; i++)
        {
            for (int j = 0; j < maxRooms; j++)
            {
                floorplan[i, j] = 0;
            }
        }

        //create the room queue for picking where to generate the next room
        roomQueue = new Queue<Room>();

        createRoom(maxRooms / 2, maxRooms / 2);
    }

    private void createRoom(int i, int j)
    {
        //if the room already exists don't make another one
        if (floorplan[i, j] == 1)
        {
            return;
        }

        //check for would be neighboring rooms
        int neighbors = findPossibleNeighbors(i, j);

        if (neighbors > 1)
        {
             return;
        }

        if(roomCount >= maxRooms)
        {
            return;
        }

        int tempRand = Random.Range(0, 10);
        //50% chance for the room to spawn, automaticall passes if its the first room
        if( (tempRand <= (1+roomRandom)) && !(i == maxRooms / 2 && j == maxRooms / 2) )
        {
            return;
        }

        //set to an active room
        floorplan[i, j] = 1;

        // THE ORDER HERE IS IMPORTANT!!! //

        //make the actual object
        GameObject ro = Instantiate(room_list[0]);
        //make the room a child of the floor parent
        ro.transform.parent = floor_parent.transform;
        //increase the floor's room count
        roomCount++;
        //change the object's name to make sense
        ro.name = "Room " + roomCount;
        //make the room object
        Room r = new Room(ro, g, i, j);
        //put the room where it is going to stay
        ro.transform.position = new Vector3(g.getNode(i * r.room_width, j * r.room_height).x + 0.5f, g.getNode(i * r.room_width, j * r.room_height).y + 0.5f, 0f);
        //generate the nodes in the room object for its new position
        r.GenerateNodes();
        //enqueue the room to be checked later
        roomQueue.Enqueue(r);
        //add the room to the list of rooms on the floor
        rooms.Add(r);
    }

    private int findPossibleNeighbors(int x, int y)
    {
        //check all four sides for would be neighbors according to the floorplan
        return floorplan[x + 1, y] + floorplan[x - 1, y] + floorplan[x, y + 1] + floorplan[x, y - 1];
    }

}

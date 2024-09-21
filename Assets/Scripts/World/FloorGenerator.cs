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
    [SerializeField] GameObject room;

    //booleans
    private bool isStarted;

    //Number variables
    public int floor_width;
    public int floor_height;
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
        g = new Grid(floor_width*20, floor_height*20, 1);

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
                if (room.floorposx < floor_width-1) { createRoom(room.floorposx+1, room.floorposy); }
                if (room.floorposy > 1) { createRoom(room.floorposx, room.floorposy-1); }
                if (room.floorposy < floor_height-1) { createRoom(room.floorposx, room.floorposy+1); }
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
            }
        }
    }

    private void CreateFloor()
    {
        //create the floor plan and set every int to 0
        //the floorplan is a 2d array recreation of the actual floor
        floorplan = new int[floor_width, floor_height];
        for (int i = 0; i < floor_width; i++)
        {
            for (int j = 0; j < floor_height; j++)
            {
                floorplan[i, j] = 0;
            }
        }

        //create the room queue for picking where to generate the next room
        roomQueue = new Queue<Room>();

        createRoom(floor_width / 2, floor_height / 2);
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
        if( (tempRand <= (1+roomRandom)) && !(i == floor_width / 2 && j == floor_height / 2) )
        {
            return;
        }

        //set to an active room
        floorplan[i, j] = 1;

        // THE ORDER HERE IS IMPORTANT!!! //

        //make the actual object
        GameObject ro = Instantiate(room);
        //increase the floor's room count
        roomCount++;
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

    /*
     * used solely for testing purposes
     * 
    private void GenerateFloor()
    {
        //decompose to generate grid
        decomposer.Decompose();

        //pick a random room

        //pick a random node
        WorldNode tempNode = decomposer.g.randomNode();
        Debug.Log("Node is: " + tempNode.x + " " + tempNode.y);

        //create the room 
        GameObject randRoom = Instantiate(room);
        //place the room at the random node
        randRoom.transform.position = new Vector3(tempNode.x, tempNode.y, 0f);

        //check to make sure all doors are within the nodes, otherwise delete//

        //find doors
        //DOORS MUST BE FIRST THING ON OBJECT FOR THIS TO WORK!!!!//
        GameObject allDoors = randRoom.transform.GetChild(0).gameObject;
        foreach(Transform door in allDoors.transform)
        {
            if(!decomposer.g.isInGrid(door))
            {
                Debug.Log("room destroyed");
                Destroy(randRoom);
            }    
        }
    }
    */

}

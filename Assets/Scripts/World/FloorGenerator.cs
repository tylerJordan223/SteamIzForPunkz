using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorGenerator : MonoBehaviour
{
    //Script to randomly generator floors

    //collection of rooms, to be filled with some sort of list of possible rooms. If possible just grab the folder of room prefabs
    [SerializeField] GameObject room;
    [SerializeField] WorldDecomp decomposer;

    //booleans
    private bool isStarted;

    //Number variables
    public int floor_width;
    public int floor_height;
    private int maxRooms;
    private int roomCount;

    //Data Structures
    private List<Room> rooms;
    private int[,] floorplan;
    private Queue<Room> roomQueue;

    private void Start()
    {
        //decompose to generate grid
        decomposer.Decompose();

        //mark as started
        isStarted = true;
        maxRooms = 20;
        roomCount = 0;

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
            }
            if(roomQueue.Count == 0)
            {
                Debug.Log("GENERATION FINISHED!!!");
                isStarted = false;
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
            Debug.Log("Room already exists!");
            return;
        }

        //check for would be neighboring rooms
        int neighbors = findPossibleNeighbors(i, j);

        if (neighbors > 1)
        {
            Debug.Log("Too many neighbors!");
             return;
        }

        if(roomCount >= maxRooms)
        {
            Debug.Log("Hit Max Rooms!");
            return;
        }

        //50% chance for the room to spawn, automaticall passes if its the first room
        if( (Random.Range(0, 10) <= 5) && !(i == floor_width / 2 && j == floor_height / 2) )
        {
            Debug.Log("Random Chance!");
            return;
        }

        //set to an active room
        floorplan[i, j] = 1;

        //make the room
        GameObject ro = Instantiate(room);
        Room r = new Room(ro, decomposer.g, i, j);
        roomCount++;
        ro.transform.position = new Vector3(decomposer.g.getNode(i*r.room_width, j*r.room_height).x, decomposer.g.getNode(i*r.room_width, j*r.room_height).y, 0f);
        roomQueue.Enqueue(r);
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

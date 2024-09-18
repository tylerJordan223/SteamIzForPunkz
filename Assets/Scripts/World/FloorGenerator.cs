using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    //Script to randomly generator floors

    //collection of rooms, to be filled with some sort of list of possible rooms. If possible just grab the folder of room prefabs
    [SerializeField] GameObject room;
    [SerializeField] WorldDecomp decomposer;

    private void Start()
    {
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            GenerateFloor();
        }
    }

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

}

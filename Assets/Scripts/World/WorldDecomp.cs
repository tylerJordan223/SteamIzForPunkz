using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static UnityEngine.Rendering.DebugUI.Table;

public class WorldDecomp : MonoBehaviour
{
    /*
    this whole script is about the WorldNode and Grid objects
    WorldNode is each individual node on the map
    Grid is the entire map of nodes
     */
}

//Node class to be used
public class WorldNode
{
    public int x, y;
    public float startToNode, startToEnd;
    public bool isObstacle;
    public bool isPlayer;
    public bool isDoor;
    public bool isEnemy;
    public WorldNode parent;

    public WorldNode(int _x, int _y, bool _isObstacle, bool _isPlayer, bool _isDoor, bool _isEnemy)
    {
        x = _x;
        y = _y;
        isObstacle = _isObstacle;
        isPlayer = _isPlayer;
        isDoor = _isDoor;
        isEnemy = _isEnemy;
    }

    public float get_f()
    {
        return startToNode + startToEnd;
    }

    public Vector2 getPosition2()
    {
        return new Vector2(x, y);
    }

    public Vector3 getPosition3()
    {
        return new Vector3(x, y, 0f);
    }
}

public class Grid
{
    public WorldNode[,] nodes;
    public WorldNode playerNode;
    public int gridLength;
    public int gridHeight;
    public int node_size;
    private int r;
    private int c;

    public Grid(int l, int h, int _node_size)
    {
        gridLength = l;
        gridHeight = h;
        node_size = _node_size;

        //amount of rows/columns needed for the space
        r = gridLength / node_size;
        c = gridHeight / node_size;

        nodes = new WorldNode[r, c];

        Decompose();
    }

    //create a list of nodes in the world
    public void Decompose()
    {
        //go through each row and column
        for (int row = 0; row < r; row++)
        {
            for (int col = 0; col < c; col++)
            {
                nodes[row, col] = new WorldNode(row, col, false, false, false, false);
            }
        }

        //check the collisions on the grid
        checkGrid();
    }

    //check the grid for any collisions
    public void checkGrid()
    {
        //start at 0
        float startX = 0;
        float startY = 0;

        //get the offset to find the center
        float nco = node_size / 2;

        for (int row = 0; row < r; row++)
        {
            for (int col = 0; col < c; col++)
            {
                float x = startX + nco + (node_size * col);
                float y = startY + nco + (node_size * row);

                Vector3 starting_position = new Vector3(y, x, 10f);

                //check for the collision
                Collider2D hit = Physics2D.OverlapPoint(new Vector2(y, x), ~LayerMask.GetMask("Room"));
                if (hit != null)
                {
                    //decide what it hit
                    if (hit.gameObject.CompareTag("Player"))
                    {
                        nodes[row, col].isPlayer = true;
                        Debug.DrawRay(starting_position, Vector3.back * 20, Color.cyan, 50000);
                    }
                    else if (hit.gameObject.CompareTag("Door"))
                    {
                        nodes[row, col].isDoor = true;
                        Debug.DrawRay(starting_position, Vector3.back * 20, Color.gray, 50000);
                    }
                    else if (hit.gameObject.CompareTag("enemy"))
                    {
                        nodes[row, col].isEnemy = true;
                        Debug.DrawRay(starting_position, Vector3.back * 20, Color.magenta, 50000);
                    }
                    else
                    {
                        //hit something unspecified, considered obstacle
                        nodes[row, col].isObstacle = true;
                        Debug.DrawRay(starting_position, Vector3.back * 20, Color.red, 50000);
                    }
                }
                else
                {
                    //if it did not hit anything show green
                    nodes[row, col].isPlayer = false;
                    nodes[row, col].isEnemy = false;
                    nodes[row, col].isObstacle = false;
                    //IMPORTANT TO NOTE!!! DOORS WILL ALWAYS STAY TRUE AFTER GENERATION!!!
                    Debug.DrawRay(starting_position, Vector3.back * 20f, Color.green, 50000);
                }
            }
        }
    }

    //returns the game object at the given node
    public GameObject getObjectAtNode(WorldNode n)
    {
        //check for the collision
        Collider2D hit = Physics2D.OverlapPoint(new Vector2(n.x, n.y), ~LayerMask.GetMask("Room"));
        if (hit != null)
        {
            return hit.gameObject;
        }
        else
        {
            Debug.Log("Hit Nothing");
            return null;
        }
    }

    //returns the room at the given node
    public GameObject getRoomGameObjectAtNode(WorldNode n)
    {
        //check for the collision
        Collider2D hit = Physics2D.OverlapPoint(new Vector2(n.x, n.y), LayerMask.GetMask("Room"));
        if (hit != null)
        {
            //the game object is the floor, so that needs to check the parent until room
            return hit.transform.parent.parent.gameObject;
        }
        else
        {
            Debug.Log("Hit Nothing");
            return null;
        }
    }

    //returns the node the player is on
    public WorldNode getPlayerNode()
    {
        //be sure to upgrade the map to know where the player is
        checkGrid();
        for(int r = 0; r < gridLength; r++)
        {
            for(int c = 0; c < gridHeight; c++)
            {
                if (nodes[r,c].isPlayer)
                {
                    return nodes[r, c];
                }
            }
        }
        Debug.Log("Could not Find Player");
        return null;
    }

    //sets the player node variable in the class to playerNode
    public void setPlayerNode()
    {
        for (int r = 0; r < nodes.Length; r++)
        {
            for (int c = 0; c < nodes.Length; c++)
            {
                if (nodes[r, c].isPlayer)
                {
                    playerNode = nodes[r, c];
                }
            }
        }
    }

    //function that sets the player node using a node
    public void setPlayerNodeAtNode(WorldNode n)
    {
        //if the node is already the playernode
        if(playerNode != null)
        {
            if (playerNode == n)
            {
                //just to be safe set the node
                playerNode.isPlayer = true;
            }
            else
            {
                //cycle the player node to the new one
                playerNode.isPlayer = false;
                playerNode = n;
                n.isPlayer = true;
            }
        }
        else
        {
            playerNode = n;
            playerNode.isPlayer = true;
        }
    }

    //checks if an object is in the
    public bool isInGrid(Transform t)
    {
        //return if within the confines of the grid
        if ((t.position.x > 0 && t.position.x < gridLength) && (t.position.y > 0 && t.position.y < gridHeight))
        {
            return true;
        }
        //no other options
        return false;
    }

    //get a random node in the grid
    public WorldNode randomNode()
    {
        return nodes[UnityEngine.Random.Range(0, gridLength), UnityEngine.Random.Range(0, gridHeight)];
    }

    //returns the node at the position of the transform
    public WorldNode getNode(Transform t)
    {
        WorldNode temp = nodes[(int)Mathf.Round(t.position.x), (int)Mathf.Round(t.position.y)];
        if (temp != null)
        {
            return temp;
        }
        else
        {
            Debug.Log("Could not find node at position");
            return null;
        }
    }
    //returns the node at the position given
    public WorldNode getNode(int x, int y)
    {
        WorldNode temp = nodes[x, y];
        if (temp != null)
        {
            return temp;
        }
        else
        {
            Debug.Log("Could not find node at position");
            return null;
        }
    }

}

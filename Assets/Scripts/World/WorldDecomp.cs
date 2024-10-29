using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

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
    public WorldNode parent;

    public WorldNode(int _x, int _y, bool _isObstacle, bool _isPlayer, bool _isDoor)
    {
        x = _x;
        y = _y;
        isObstacle = _isObstacle;
        isPlayer = _isPlayer;
        isDoor = _isDoor;
    }

    public float get_f()
    {
        return startToNode + startToEnd;
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
                nodes[row, col] = new WorldNode(row, col, false, false, false);
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
                Collider2D hit = Physics2D.OverlapPoint(new Vector2(row, col), ~LayerMask.GetMask("Room"));
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
                    Debug.DrawRay(starting_position, Vector3.back * 20f, Color.green, 50000);
                }
            }
        }
    }

    //returns the node the player is on
    public WorldNode getPlayerNode()
    {
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

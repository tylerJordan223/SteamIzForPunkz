using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class WorldDecomp : MonoBehaviour
{
    //Necessary variables for the storage of nodes

    //debug settings to change through testing
    [SerializeField] int t_length;
    [SerializeField] int t_height;

    public WorldNode[,] nodes;
    public Grid g;
    private int node_size;

    //rows and columns of nodes
    private int r;
    private int c;

    private void Start()
    {
        node_size = 1;

        //amount of rows/columns needed for the space
        r = t_length / node_size;
        c = t_height / node_size;

        nodes = new WorldNode[r, c];

        Decompose();
    }

    public void Decompose()
    {
        float startX = 0;
        float startY = 0;

        //get the offset to find the center
        float nco = node_size / 2;

        //go through each row and column
        for(int row = 0; row < r; row++)
        {
            for(int col = 0; col < c; col++)
            {
                float x = startX + nco + (node_size * col);
                float y = startY + nco + (node_size * row);

                Vector3 starting_position = new Vector3(y, x, 10f);

                //check for the collision
                Collider2D hit = Physics2D.OverlapPoint(new Vector2(row, col));
                if (hit != null)
                {
                    //obstacles
                    nodes[row, col] = new WorldNode(row, col, false, true, true);

                    //set target/player diff
                    if(hit.gameObject.CompareTag("Target"))
                    {
                        nodes[row, col].isObstacle = false;
                        nodes[row, col].isTarget = true;
                        Debug.DrawRay(starting_position, Vector3.back * 20, Color.magenta, 50000);
                    }
                    else if(hit.gameObject.CompareTag("Player"))
                    {
                        nodes[row, col].isObstacle = false;
                        nodes[row, col].isPlayer = true;
                        Debug.DrawRay(starting_position, Vector3.back * 20, Color.cyan, 50000);
                    }
                    else
                    {
                        //hit something unspecified
                        Debug.DrawRay(starting_position, Vector3.back * 20, Color.red, 50000);
                    }
                }
                else
                {
                    //if it did not hit anything show green
                    Debug.DrawRay(starting_position, Vector3.back * 20f, Color.green, 50000);
                    nodes[row, col] = new WorldNode(row, col, false, false, false);
                }
            }
        }

        g = new Grid(nodes, t_length, t_height);
        g.getPlayerNode();
    }

}

//Node class to be used
public class WorldNode
{
    public int x, y;
    public float startToNode, startToEnd;
    public bool isTarget;
    public bool isObstacle;
    public bool isPlayer;
    public WorldNode parent;

    public WorldNode(int _x, int _y, bool _isTarget, bool _isObstacle, bool _isPlayer)
    {
        x = _x;
        y = _y;
        isTarget = _isTarget;
        isObstacle = _isObstacle;
        isPlayer = _isPlayer;
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

    public Grid(WorldNode[,] nodes_, int l, int h)
    {
        nodes = nodes_;
        gridLength = l;
        gridHeight = h;
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
        return nodes[Random.Range(0, gridLength),Random.Range(0,gridHeight)];
    }

    //returns the node at the position
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
}

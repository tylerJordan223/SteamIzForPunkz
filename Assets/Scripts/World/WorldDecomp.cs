using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WorldDecomp : MonoBehaviour
{
    //Necessary variables for the storage of nodes

    //debug settings to change through testing
    [SerializeField] int t_width;
    [SerializeField] int t_length;

    public WorldNode[,] nodes;
    private int node_size;

    //rows and columns of nodes
    private int r;
    private int c;

    private void Start()
    {
        node_size = 1;

        //amount of rows/columns needed for the space
        r = t_width / node_size;
        c = t_length / node_size;

        nodes = new WorldNode[r, c];
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Decompose();
        }
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
                    Debug.Log("HIT!");

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

                    Debug.Log(row + " " + col);
                }
                else
                {
                    //if it did not hit anything show green
                    Debug.DrawRay(starting_position, Vector3.back * 20f, Color.green, 50000);
                    nodes[row, col] = new WorldNode(row, col, false, false, false);
                }
            }
        }

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
    
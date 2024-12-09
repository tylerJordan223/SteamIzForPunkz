using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    public Vector2 getPosition2()
    {
        return new Vector2(x, y);
    }

    public Vector3 getPosition3()
    {
        return new Vector3(x, y, 0f);
    }
}

//a node specifically to do aStar
public class AStarNode
{
    public WorldNode node;
    public AStarNode parent;
    public float g, h, f;

    public AStarNode(WorldNode _n, AStarNode _p, float _g, float _h, float _f)
    {
        node = _n;
        parent = _p;
        g = _g;
        h = _h;
        f = _f;
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

    //constructor
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
                        Debug.DrawRay(starting_position, Vector3.back * 20, Color.cyan, 10);
                    }
                    else if (hit.gameObject.CompareTag("Door"))
                    {
                        nodes[row, col].isDoor = true;
                        Debug.DrawRay(starting_position, Vector3.back * 20, Color.gray, 10);
                    }
                    else if (hit.gameObject.CompareTag("enemy"))
                    {
                        nodes[row, col].isEnemy = true;
                        Debug.DrawRay(starting_position, Vector3.back * 20, Color.magenta, 10);
                    }
                    else
                    {
                        //hit something unspecified, considered obstacle
                        nodes[row, col].isObstacle = true;
                        Debug.DrawRay(starting_position, Vector3.back * 20, Color.red, 10);
                    }
                }
                else
                {
                    //if it did not hit anything show green
                    nodes[row, col].isPlayer = false;
                    nodes[row, col].isEnemy = false;
                    nodes[row, col].isObstacle = false;
                    //IMPORTANT TO NOTE!!! DOORS WILL ALWAYS STAY TRUE AFTER GENERATION!!!
                }
            }
        }
    }

    #region getting nodes

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

    //returns the 4 neighbors of the nodes
    public List<WorldNode> getNodeNeighbors(WorldNode n)
    {
        List<WorldNode> neighbors = new List<WorldNode>();

        //test in all 4 directions for neighbors
        if(getNode(n.x+1, n.y) != null)
        {
            neighbors.Add(getNode(n.x + 1, n.y));
        }
        if (getNode(n.x + 1, n.y) != null)
        {
            neighbors.Add(getNode(n.x - 1, n.y));
        }
        if (getNode(n.x + 1, n.y) != null)
        {
            neighbors.Add(getNode(n.x, n.y + 1));
        }
        if (getNode(n.x + 1, n.y) != null)
        {
            neighbors.Add(getNode(n.x, n.y - 1));
        }

        return neighbors;
    }

    #endregion getting nodes

    #region setting nodes

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

    #endregion setting nodes

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

    #region Astar

    //getting the manhattan distance between nodes for ASTar
    public float ManhattanDistance(WorldNode a, WorldNode b)
    {
        return Mathf.Abs(b.getPosition2().x - a.getPosition2().x) + Mathf.Abs(b.getPosition2().y - a.getPosition2().y);
    }

    //getting the diagonal distance between nodes for AStar
    public float DiagonalDistance(WorldNode a, WorldNode b)
    {
        float dx = Mathf.Abs(a.x - b.x);
        float dy = Mathf.Abs(a.y - b.y);

        return 1 * (dx + dy) + (Mathf.Sqrt(2) - 2 * 1) * Mathf.Min(dx, dy);
    }

    public List<WorldNode> AStarSearchToPlayer(WorldNode n)
    {
        List<AStarNode> open = new List<AStarNode>();
        List<AStarNode> closed = new List<AStarNode>();

        //all 0 since this is the starting node (also signified by having no parent
        open.Add(new AStarNode(n, null, 0f, 0f, 0f));

        while(open.Count > 0)
        {
            //pop the first node
            AStarNode q = open[0];

            //find the node with the smalled f (distance)
            foreach(AStarNode node in open)
            {
                if(node.f < q.f)
                {
                    q = node;
                }
            }

            //remove whatever node ended up being q to pop it
            open.Remove(q);

            //generate the neighbors
            List<WorldNode> neighbors = getNodeNeighbors(q.node);
            List<AStarNode> aNeighbors = new List<AStarNode>();
            //translate to AStar Nodes
            foreach(WorldNode node in neighbors)
            {
                if(!node.isObstacle)
                {
                    //initialize all the values to max since the smaller one is what we are looking for
                    aNeighbors.Add(new AStarNode(node, q, float.MaxValue, float.MaxValue, float.MaxValue));
                }
            }

            //check all the nodes
            foreach(AStarNode node in aNeighbors)
            {
                if(node.node.isPlayer)
                {
                    return AStarPath(closed, node);
                }

                //compute the distance and heuristic values for the node
                node.g = q.g + 1f; //always 1 cost to move between nodes
                node.h = DiagonalDistance(node.node, playerNode);
                node.f = node.g + node.h;

                //will not add if this flag is false
                bool add = true;

                //check the open list
                foreach(AStarNode oNode in open)
                {
                    //if they are at the same position
                    if(oNode.node == node.node)
                    {
                        //if the one in open list already has a lower f, thne this node will not be added
                        if(oNode.f < node.f)
                        {
                            add = false;
                        }
                    }
                }

                //check the closed list
                foreach(AStarNode cNode in closed)
                {
                    //if they are the same node
                    if(cNode.node == node.node)
                    {
                        //if the one in closed list had a lower f, skip it
                        if(cNode.f < node.f)
                        {
                            add = false;
                        }
                    }
                }

                //if all checks out, add the node to the open list
                if(add)
                {
                    open.Add(node);
                }
            }

            //add the node to the closed list
            closed.Add(q);
        }

        //if it gets here then it never found a path
        Debug.Log("There was no path to the player");
        return null;
    }

    //used to return the whole path when finding the last node in AStar
    public List<WorldNode> AStarPath(List<AStarNode> a_nodes, AStarNode end)
    {
        List<WorldNode> path = new List<WorldNode>();

        //find each parent until finding the current position
        AStarNode temp_node = end;
        while(temp_node.parent != null)
        {
            path.Add(temp_node.node);
            temp_node = temp_node.parent;
        }

        //reverse the path so the last node in it is the goal
        path.Reverse();

        return path;
    }

    #endregion Astar

}

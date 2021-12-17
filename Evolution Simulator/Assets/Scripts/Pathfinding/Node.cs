using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node(float x, float y, float z)
    {
        position.x = x;
        position.y = y;
        position.z = z;
        indices = Pathfinding.positionToIndices(position);
    }
    public Node(float _g, float _h, Vector3 _pos, Node _parent){
        g = _g;
        h = _h;
        position = _pos;
        indices = Pathfinding.positionToIndices(position);
        parent = _parent;
    }
    public Node(Node node) {
        g = node.g;
        h = node.h;
        position = node.position;
        indices = Pathfinding.positionToIndices(position);
        parent = node.parent;
    }

    public float GetScore() { return g + h; }
    public float CalculateF(Vector3 destination)
    {
        if (parent.isNull == false)
            g = parent.g + 1;
        else 
            g = 0;
        h = Vector3.Distance(position, destination);

        return g + h;
    }
    public int FindWalkableAdjacents(ref PathList adjacentNodes)
    {
        Node cell = new Node(position.x + 1.0f, position.y, position.z);
        if (Pathfinding.IsWalkable(cell))
            adjacentNodes.list.Add(new Node(-1, -1, cell.position, this));

        cell = new Node(position.x - 1.0f, position.y, position.z);
        if (Pathfinding.IsWalkable(cell))
            adjacentNodes.list.Add(new Node(-1, -1, cell.position, this));

        cell = new Node(position.x, position.y + 1.0f, position.z);
        if (Pathfinding.IsWalkable(cell))
            adjacentNodes.list.Add(new Node(-1, -1, cell.position, this));

        cell = new Node(position.x, position.y - 1.0f, position.z);
        if (Pathfinding.IsWalkable(cell))
            adjacentNodes.list.Add(new Node(-1, -1, cell.position, this));

        cell = new Node(position.x, position.y, position.z + 1.0f);
        if (Pathfinding.IsWalkable(cell))
            adjacentNodes.list.Add(new Node(-1, -1, cell.position, this));

        cell = new Node(position.x, position.y, position.z - 1.0f);
        if (Pathfinding.IsWalkable(cell))
            adjacentNodes.list.Add(new Node(-1, -1, cell.position, this));

        int ret = adjacentNodes.list.Count;
        return ret;
    }

    public float g; // Distance from starting node
    private float h; // Distance from end node
    public Vector3Int indices; // Indices in the pathfinding node list
    public Vector3 position;
    public Node parent; // Used to reconstruct the path in the end
    public bool isNull = false;
}

public struct PathList
{
    public List<Node> list;

    // Looks for a node in this list and returns it's list node or null
    public Node Find(Node node) {
        for(int i = 0; i < list.Count; ++i)
        {
            if (list[i] == node)
                return list[i];
        }
        return null;
    }

    // Returns the Node with lowest score in this list or null if empty
    public Node GetNodeLowestScore()
    {
        Node ret = null;
        float min = float.MaxValue;

        for(int i = 0; i < list.Count; ++i)
        {
            if (list[i].GetScore() < min)
            {
                min = list[i].GetScore();
                ret = list[i];
            }
        }        

        return ret;
    }
}
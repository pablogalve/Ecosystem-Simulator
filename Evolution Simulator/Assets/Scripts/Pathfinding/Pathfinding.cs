using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    enum NODE_TYPES{
        WALKABLE = 0,
        NON_WALKABLE
    }

    public static int[] map;
    // size of the map
    public static int map_width;
    public static int map_height;
    public static int map_depth;

    public static void CreateWalkabilityMap(int width, int height, int depth, ref int[] buffer) {
        int[] map = new int[width * height * depth];
        for(int i = 0; i < map.Length; ++i)
        {
            map[i] = (int)NODE_TYPES.WALKABLE;
        }
        buffer = map;
    }
    public static void SetMap(int width, int height, int depth, ref int[] data)
    {
        map_width = width;
        map_height = height;
        map_depth = depth;

        map = new int[width * height * depth];
        for (int i = 0; i < map.Length; ++i)
        {
            map[i] = data[i];
        }
    }

    public static int CreatePath(Vector3 origin, Vector3 destination, Vector3Int originIndex, Vector3Int destinationIndex, ref List<Vector3Int> last_path)
    {
        //Return -1 if the path origin or destination are unwalkable
        if(!IsWalkable(originIndex) || !IsWalkable(destinationIndex))
            return -1;

        // Create 2 Path lists
        last_path.Clear();
        PathList open;
        open.list = new List<Node>();
        PathList closed;
        closed.list = new List<Node>();

        // Add the origin tile to open
        Node nodeToAdd = new Node(0, GetDistanceTo(originIndex, destinationIndex), origin, null);
        open.list.Add(nodeToAdd);

        // Iterate while we have tile in the open list
        while (open.GetNodeLowestScore().isNull == false)
        {
            Debug.Log("Open: " + open);
            Debug.Log("Closed: " + closed);
            Debug.Log("Last_path: " + last_path);
            Node current_node = new Node(open.GetNodeLowestScore());
            //Move the lowest score cell from open list to the closed list
            closed.list.Add(current_node);
            open.list.Remove(current_node);

            if(current_node.indices == destinationIndex)
            {
                for (Node iterator = current_node; iterator.position != origin; iterator = iterator.parent)
                {                       
                    last_path.Add(iterator.indices);
                }

                last_path.Reverse();

                return 0;    
            }

            //We get a list of adjacent nodes
            PathList adjacentNodes = new PathList();
            adjacentNodes.list = new List<Node>();
            int limit = current_node.FindWalkableAdjacents(ref adjacentNodes);
            //We iterate adjancent nodes
            for (int i = 0; i < limit; ++i)
            {
                // ignore nodes in the closed list <======> do things only if we didnt find them
                if(closed.Exists(adjacentNodes.list[i]) == false)
                {
                    //if adjacent node is null, we calculate the open one
                    if(open.Exists(adjacentNodes.list[i]) == false)
                    {
                        adjacentNodes.list[i].CalculateF(destinationIndex);
                        open.list.Add(adjacentNodes.list[i]);
                    }
                    //if we are already into open list, we check a better path
                    else if (adjacentNodes.list[i].g < open.Find(adjacentNodes.list[i]).g)
                    {
                        // if we find a better path, we search a new parent
                        adjacentNodes.list[i].CalculateF(destinationIndex);
                        open.list.Remove(open.Find(adjacentNodes.list[i]));
                        open.list.Add(adjacentNodes.list[i]);                    
                    }
                }
            }
        }
        return -1;
    }
    public static bool IsWalkable(Vector3Int indices){
        if (indices.x < 0) return false;
        if (indices.y < 0) return false;
        if (indices.z < 0) return false;
        return true;
    } 

    public static bool IsWalkable(Node node) {
        // TODO: This is temporal
        if (node.indices.x < 0) return false;
        if (node.indices.y < 0) return false;
        if (node.indices.z < 0) return false;
        return true;
    } 

    public static Vector3 indicesToPosition(Vector3Int indices) {
        return indices;
    }

    public static Vector3Int positionToIndices(Vector3 position) {
        return new Vector3Int((int)position.x, (int)position.y, (int)position.z);
    }

    //Return the distance in nodes
    public static int GetDistanceTo(Vector3Int origin, Vector3Int destination)
    {
        int result = origin.x - destination.x + origin.y - destination.y + origin.z - destination.z;
        return result >= 0 ? result : (-1 * result);
    }
}

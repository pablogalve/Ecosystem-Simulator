using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeighmapData : MonoBehaviour
{
    /*public static Vector2Int resolution = new Vector2Int(256, 256);
    public static int[,] heightmap = new int[resolution[0], resolution[1]];
    public static GameObject islandMap = null;
    public static Vector3[] islandMeshVerts;

    // Start is called before the first frame update
    void Start()
    {
        islandMap = GameObject.Find("IslandMap");
        if (islandMap != null) islandMeshVerts = islandMap.GetComponent<MeshFilter>().mesh.vertices;
        else Debug.LogError("islandMap is null on HeighmapData.cs");

        //Initialize array
        for (int i = 0; i < resolution[0]; ++i)
        {
            for (int j = 0; j < resolution[1]; ++j)
            {
                heightmap[i,j] = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    public static Vector3 GetTerrainHeight(float x, float z)
    {
        //Create object to store raycast data
        RaycastHit hit;

        //Create origin for raycast that is above the terrain. I chose 100.
        Vector3 origin = new Vector3(x, 100, z);

        //Send the raycast.
        Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity);

        return hit.point;
    }
}

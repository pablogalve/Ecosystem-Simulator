using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeighmapData : MonoBehaviour
{
    public static Vector3 GetTerrainHeight(float x, float z)
    {
        //Create object to store raycast data
        RaycastHit hit;

        //Create origin for raycast that is above the terrain. I chose 100.
        Vector3 origin = new Vector3(x, 500, z);

        //Send the raycast.
        Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity);

        return hit.point;
    }

    public static bool isValidSpawnPoint(Vector3 position)
    {
        if (position == null) return false;

        // TODO: Numbers shouldn't be hardcoded
        if (position.y < 25.0f) return false; // Position too low, out of grass bioma 
        if (position.y > 115.0f) return false; // Position too high, out of grass bioma 

        return true;
    }

    public static float GetRandomVariation(float min, float max)
    {
        return Random.Range(min, max);
    }
}
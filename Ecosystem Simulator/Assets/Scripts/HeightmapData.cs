using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightmapData : MonoBehaviour
{
    public float GetTerrainHeight(float x, float z)
    {
        //Create object to store raycast data
        RaycastHit hit;

        //Create origin for raycast that is above the terrain. I chose 100.
        Vector3 origin = new Vector3(x, 500, z);

        //Send the raycast.
        Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity);

        return hit.point.y;
    }

    public bool IsValidPosition(EntityManager.EntityType type, Vector3 position)
    {
        if (position == null) return false;

        switch (type)
        {
            case EntityManager.EntityType.TREE:
                {
                    // TODO: Numbers shouldn't be hardcoded
                    if (position.y < 25.0f) return false; // Position too low, out of grass bioma 
                    if (position.y > 115.0f) return false; // Position too high, out of grass bioma 

                    if (AmountOfTreesPerArea.Instance.hasReachedMaxAmountOfTrees(position.x, position.z)) return false;
                }
                break;
            case EntityManager.EntityType.FOOD:
                return true;
            case EntityManager.EntityType.ANIMAL:
                return true;
                default: return false;
        }        

        return true;
    }

    public float GetRandomVariation(float min, float max)
    {
        return Random.Range(min, max);
    }

    public Vector3 LevyWalk(Vector3 currPos, float shortDistance, float largeDistance, float mu)
    {
        Vector3 target = currPos;
        do
        {
            float u = GetRandomVariation(0.0f, 1.0f);
            float range = Mathf.Pow(largeDistance, 1 - mu) - Mathf.Pow(shortDistance, 1 - mu);
            float baseVal = range * u + Mathf.Pow(shortDistance, 1 - mu);
            float distanceToMove = Mathf.Pow(baseVal, 1 / (1 - mu));

            float angle = GetRandomVariation(0f, 359f);

            target.x += distanceToMove * Mathf.Cos(angle * Mathf.Deg2Rad);            
            target.z += distanceToMove * Mathf.Sin(angle * Mathf.Deg2Rad);
            target.y = GetTerrainHeight(target.x, target.z);
        }
        while (!IsValidPosition(EntityManager.EntityType.ANIMAL, target));

        return target;
    }

    #region SINGLETON PATTERN
    public static HeightmapData _instance;
    public static HeightmapData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HeightmapData>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("HeighmapData");
                    _instance = container.AddComponent<HeightmapData>();
                }
            }

            return _instance;
        }
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeighmapData : MonoBehaviour
{
    public Vector3 GetTerrainHeight(float x, float z)
    {
        //Create object to store raycast data
        RaycastHit hit;

        //Create origin for raycast that is above the terrain. I chose 100.
        Vector3 origin = new Vector3(x, 500, z);

        //Send the raycast.
        Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity);

        return hit.point;
    }

    public bool isValidSpawnPoint(EntityManager.EntityType type, Vector3 position)
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

    #region SINGLETON PATTERN
    public static HeighmapData _instance;
    public static HeighmapData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<HeighmapData>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("AmountOfTreesPerArea");
                    _instance = container.AddComponent<HeighmapData>();
                }
            }

            return _instance;
        }
    }
    #endregion
}
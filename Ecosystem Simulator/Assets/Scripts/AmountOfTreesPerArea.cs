using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmountOfTreesPerArea : MonoBehaviour
{    
    public int[,] map;
    public Vector2Int mapAreas = new Vector2Int(10, 10); //Axis x and z
    public Vector2Int worldSize = new Vector2Int(1000, 1000);
    public int maxTreesPerArea = 100;

    private float tileXSize;
    private float tileZSize;

    private void Awake()
    {
        map = new int[mapAreas[0], mapAreas[1]];

        tileXSize = worldSize[0] / mapAreas[0];
        tileZSize = worldSize[1] / mapAreas[1];
    }

    public bool hasReachedMaxAmountOfTrees(float positionX, float positionY)
    {
        Vector2Int index = WorldToMap(new Vector2(positionX, positionY));

        if (map[index[0], index[1]] >= maxTreesPerArea) return true;
        else return false;
    }

    private Vector2Int WorldToMap(Vector2 position)
    {
        int myX = Mathf.FloorToInt((position[0]) / tileXSize);
        int myZ = Mathf.FloorToInt((position[1]) / tileZSize);

        return new Vector2Int(myX, myZ);
    }

    private Vector2 MapToWorld(Vector2Int indices)
    {
        return new Vector2(indices[0] * tileXSize, indices[1] * tileZSize);
    }

    #region SINGLETON PATTERN
    public static AmountOfTreesPerArea _instance;
    public static AmountOfTreesPerArea Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AmountOfTreesPerArea>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("AmountOfTreesPerArea");
                    _instance = container.AddComponent<AmountOfTreesPerArea>();
                }
            }

            return _instance;
        }
    }
    #endregion
}

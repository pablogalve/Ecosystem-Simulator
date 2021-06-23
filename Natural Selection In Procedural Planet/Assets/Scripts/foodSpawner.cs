using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public int foodToSpawn = 50;
    void Start()
    {
        for (int i = 0; i < foodToSpawn; i++)
        {
            GameObject newFood = Instantiate(foodPrefab, GetRandomVector3(), Quaternion.identity);
            AddToList(newFood);
        }
    }

    void AddToList(GameObject newFood)
    {
        if (DataHolder.food == null)
            DataHolder.food = new List<GameObject>();

        DataHolder.food.Add(newFood);
    }

    Vector3 GetRandomVector3()
    {
        Vector3 min = new Vector3(-60, -60, -60);
        Vector3 max = new Vector3(70, 70, 70);
        return new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
    }
}

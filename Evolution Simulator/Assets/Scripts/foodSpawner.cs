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
            GameObject newFood = Instantiate(foodPrefab, randomNumbers.GetRandomVector3(), Quaternion.identity);
            EntityManager.CreateEntity("food", newFood);
        }
    }
}

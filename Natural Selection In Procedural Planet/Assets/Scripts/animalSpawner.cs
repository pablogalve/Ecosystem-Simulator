using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animalSpawner : MonoBehaviour
{
    public GameObject animalPrefab;
    public int animalsToSpawn = 50;
    void Start()
    {
        for (int i = 0; i < animalsToSpawn; i++)
        {
            GameObject newAnimal = Instantiate(animalPrefab, GetRandomVector3(), Quaternion.identity);
            AddToList(newAnimal);
        }
    }

    void AddToList(GameObject newAnimal)
    {
        if (DataHolder.animals == null)
            DataHolder.animals = new List<GameObject>();

        DataHolder.animals.Add(newAnimal);
    }

    Vector3 GetRandomVector3()
    {
        Vector3 min = new Vector3(-60, -60, -60);
        Vector3 max = new Vector3(70, 70, 70);
        return new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
    }
}

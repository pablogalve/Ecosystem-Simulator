using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animalSpawner : MonoBehaviour
{
    public GameObject herbivoreMale;
    public GameObject herbivoreFemale;
    public int herbivoresToSpawn = 50;

    public GameObject omnivorePrefab;
    public int omnivoresToSpawn = 50;

    public GameObject carnivoreMale;
    public GameObject carnivoreFemale;
    public int carnivoresToSpawn = 50;

    void Start()
    {
        for (int i = 0; i < herbivoresToSpawn; i++)
        {
            GameObject newAnimal = Instantiate(herbivoreMale, GetRandomVector3(), Quaternion.identity);
            EntityManager.CreateEntity("herbivores", newAnimal);
        }

        for (int i = 0; i < omnivoresToSpawn; i++)
        {
            GameObject newAnimal = Instantiate(omnivorePrefab, GetRandomVector3(), Quaternion.identity);
            EntityManager.CreateEntity("omnivores", newAnimal);
        }

        for (int i = 0; i < carnivoresToSpawn; i++)
        {
            GameObject newAnimal = Instantiate(carnivoreMale, GetRandomVector3(), Quaternion.identity);
            EntityManager.CreateEntity("carnivores", newAnimal);
        }
    }

    Vector3 GetRandomVector3()
    {
        Vector3 min = new Vector3(-60, -60, -60);
        Vector3 max = new Vector3(70, 70, 70);
        return new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
    }
}

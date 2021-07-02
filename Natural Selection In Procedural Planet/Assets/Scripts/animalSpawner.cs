using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animalSpawner : MonoBehaviour
{
    public GameObject[] prefabs;
    public string[] specieName;
    public int[] numberToSpawn;

    void Start()
    {
        if (prefabs.Length == numberToSpawn.Length && prefabs.Length == specieName.Length) //Check that 3 lists are of the same length
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                if (prefabs[i] != null && specieName[i] != null && numberToSpawn[i] != null)
                {
                    for (int j = 0; j < numberToSpawn[i]; j++)
                    {
                        GameObject newAnimal = Instantiate(prefabs[i], GetRandomVector3(), Quaternion.identity);
                        EntityManager.CreateEntity(specieName[i], newAnimal);
                    }
                }
            }
        }
    }

    Vector3 GetRandomVector3()
    {
        Vector3 min = new Vector3(-60, -60, -60);
        Vector3 max = new Vector3(70, 70, 70);
        return new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
    }
}

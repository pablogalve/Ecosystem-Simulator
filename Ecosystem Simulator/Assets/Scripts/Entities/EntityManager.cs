using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public enum EntityType
    {
        TREE,
        FOOD,
        ANIMAL
    }

    public List<List<GameObject>> entities = new List<List<GameObject>>(); // This list contains one list for every type of entity
    public int maxTrees = 2000;
    public int maxFood = 5000;
    public int maxAnimals = 100;

    void Start()
    {
        // Initialize list. The size of the loop is equivalent to the amount of different EntityTypes
        for(int i = 0; i < 3; ++i)
        {
            entities.Add(new List<GameObject>());
        }

        SetInitialScene();

        StartCoroutine(GrowOrDie());
        StartCoroutine(Reproduce());
    }

    private void SetInitialScene()
    {
        TreeManager treeManager = GetComponent<TreeManager>();
        TryToSpawn(EntityType.TREE, treeManager.treePrefab, 0f, 0f, 0.0f);

        AnimalManager animalManager = GetComponent<AnimalManager>();
        TryToSpawn(EntityType.ANIMAL, animalManager.prefab, 50f, 50f, 0.0f);
    }

    public void TryToSpawn(EntityType type, GameObject prefab, float x, float z, float randomVariation)
    {
        if (prefab == null) return;

        // Generate a spawn position
        Vector3 spawnPos = new Vector3(x, 0f, z);
        spawnPos.x += HeighmapData.GetRandomVariation(-randomVariation, randomVariation);
        spawnPos.z += HeighmapData.GetRandomVariation(-randomVariation, randomVariation);
        spawnPos = HeighmapData.GetTerrainHeight(spawnPos.x, spawnPos.z);

        if (!HeighmapData.isValidSpawnPoint(spawnPos)) return;

        // Prefab and position are valid, so it can be instantiated
        GameObject gameObject = Instantiate(prefab, spawnPos, Quaternion.identity);

        switch (type)
        {
            case EntityType.TREE:
                entities[(int)EntityType.TREE].Add(gameObject);
                break;
            case EntityType.FOOD:
                entities[(int)EntityType.FOOD].Add(gameObject);
                break;
            case EntityType.ANIMAL:
                entities[(int)EntityType.ANIMAL].Add(gameObject);
                break;
            default:
                Debug.LogError("Entered default on TryToSpawn on EntityManager.cs");
                break;
        }
    }

    public bool TryToKill(EntityType type, string UUIDtoKill)
    {
        // Looks for the UUIDtoKill in the entities list specified as argument
        // Returns true if it was successfully killed, and false otherwise
        switch (type)
        {
            case EntityType.TREE:
                {
                    for (int i = 0; i < entities[(int)EntityType.TREE].Count; i++)
                    {
                        Entity entityScript = entities[(int)EntityType.TREE][i].GetComponent<Entity>();
                        if (entityScript == null)
                        {
                            Debug.LogError("entityScript was null on EntityManager.cs on TryToKill()");
                            return false;
                        }

                        if (entityScript.GetUUID() == UUIDtoKill)
                        {
                            Destroy(entities[(int)EntityType.TREE][i]);
                            entities[(int)EntityType.TREE].RemoveAt(i);
                            return true;
                        }
                    }
                }
                break;
            case EntityType.FOOD:
                {
                    for (int i = 0; i < entities[(int)EntityType.FOOD].Count; i++)
                    {
                        Entity entityScript = entities[(int)EntityType.FOOD][i].GetComponent<Entity>();
                        if (entityScript == null)
                        {
                            Debug.LogError("entityScript was null on EntityManager.cs on TryToKill()");
                            return false;
                        }

                        if (entityScript.GetUUID() == UUIDtoKill)
                        {
                            Destroy(entities[(int)EntityType.FOOD][i]);
                            entities[(int)EntityType.FOOD].RemoveAt(i);
                            return true;
                        }
                    }
                }
                break;
            case EntityType.ANIMAL:
                {
                    for (int i = 0; i < entities[(int)EntityType.ANIMAL].Count; i++)
                    {
                        Entity entityScript = entities[(int)EntityType.ANIMAL][i].GetComponent<Entity>();
                        if (entityScript == null)
                        {
                            Debug.LogError("entityScript was null on EntityManager.cs on TryToKill()");
                            return false;
                        }

                        if (entityScript.GetUUID() == UUIDtoKill)
                        {
                            Destroy(entities[(int)EntityType.ANIMAL][i]);
                            entities[(int)EntityType.ANIMAL].RemoveAt(i);
                            return true;
                        }
                    }
                }
                break;
            default:
                Debug.LogError("Entered default on TryToSpawn on EntityManager.cs");
                break;
        }

        // Couldn't kill it
        return false;
    }

    protected IEnumerator Reproduce() {
        // Iterate different types of entities
        for (int i = 0; i < entities.Count; i++)
        {
            if ((EntityType)i == EntityType.FOOD) continue; // Food can't reproduce
            if (isMaxCapReached((EntityType)i, entities[i].Count) == true) continue;

            // Iterate all the entities of the same type
            for (int j = 0; j < entities[i].Count; j++)
            {
                if (isMaxCapReached((EntityType)i, entities[i].Count) == true) break;

                AgeController ageController = entities[i][j].GetComponent<AgeController>();
                if (ageController == null)
                {
                    Debug.LogError("AgeController is null on EntityManager.cs: Reproduce()");
                    continue;
                }

                if (ageController.canReproduce())
                {
                    // Short variation by default, except for trees
                    float randomVariation = 2f;
                    if (i == (int)EntityType.TREE) randomVariation = 50f;

                    TryToSpawn((EntityType)i, ageController.prefab, entities[i][j].transform.position.x, entities[i][j].transform.position.z, randomVariation);
                }
            }
        }
        yield return new WaitForSeconds(30.0f); // Wait before repeating the cycle
        StartCoroutine(Reproduce());
    }

    protected IEnumerator GrowOrDie()
    {
        // Iterate different types of entities
        for (int i = 0; i < entities.Count; i++)
        {
            // Iterate all the entities of the same type
            for (int j = 0; j < entities[i].Count; j++)
            {
                AgeController ageController = entities[i][j].GetComponent<AgeController>();
                if(ageController == null) Debug.LogError("AgeController script is null on EntityManager.cs on GrowOrDie");                

                // Kill tree if it has reached it's maximum age
                if (ageController.age + 1 > ageController.maxAge)
                {                    
                    entities[i].RemoveAt(j);
                    Destroy(ageController.gameObject);
                }
                else ageController.age++;

                // Scale the tree in the world according to its age
                if (ageController.growsInSize) 
                {
                    float agePercentage = (float)(ageController.age) / (float)(ageController.maxAge);
                    entities[i][j].transform.localScale = Vector3.one * agePercentage;
                }                
            }
            yield return new WaitForSeconds(0.1f); // Wait between different types of entities
        }

        yield return new WaitForSeconds(10.0f); // Wait before repeating the cycle
        StartCoroutine(GrowOrDie());
    }

    public bool isMaxCapReached(EntityType type, int currentAmount)
    {
        // If max cap of an entity type is reached, it must be skipped to save performance
        if (type == EntityType.FOOD)
        {
            if (currentAmount < maxFood) return false;
            else return true;
        }

        if (type == EntityType.ANIMAL)
        {
            if (currentAmount < maxAnimals) return false;
            else return true;
        }

        if (type == EntityType.TREE)
        {
            if (currentAmount < maxTrees) return false;
            else return true;
        }

        Debug.LogError("An error was detected in EntityManager.cs on isMaxCapReached()");
        return true;
    }
}
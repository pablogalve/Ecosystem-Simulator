﻿using System.Collections;
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
    public List<List<string>> entitiesToDelete = new List<List<string>>();  // List of UUIDs of GO that have to be removed at the end of the frame
    public int maxTrees = 2000;
    public int maxFood = 5000;
    public int maxAnimals = 100;

    // Acess to other scripts
    AnimalManager animalManager = null;
    TreeManager treeManager = null;

    void Start()
    {
        animalManager = GetComponent<AnimalManager>();
        treeManager = GetComponent<TreeManager>();

        // Initialize list. The size of the loop is equivalent to the amount of different EntityTypes
        for (int i = 0; i < 3; ++i)
        {
            entities.Add(new List<GameObject>());
            entitiesToDelete.Add(new List<string>());
        }

        SetInitialScene();

        StartCoroutine(GrowOrDie());
        StartCoroutine(Reproduce());
    }

    // LateUpdate is called after all Update functions have been called. It should be used to delete gameObjects
    private void LateUpdate()
    {
        DeleteEntities();
    }

    private void DeleteEntities()
    {
        // Delete all gameObjects waiting to be removed at the end of the frame

        #region Delete Trees
        for (int i = 0; i < entitiesToDelete[(int)EntityType.TREE].Count; ++i)
        {
            // Check for the UUIDs in the actual entities list
            for (int j = entities[(int)EntityType.TREE].Count - 1; j >= 0; --j)
            {
                // When there's a match, entity is removed
                Entity entityScript = entities[(int)EntityType.TREE][j].GetComponent<Entity>();
                if (entitiesToDelete[(int)EntityType.TREE][i] == entityScript.GetUUID())
                {
                    Destroy(entities[(int)EntityType.TREE][j]);
                    entities[(int)EntityType.TREE].RemoveAt(j);
                    continue;
                }
            }
        }
        #endregion

        #region Delete Animals
        for (int i = 0; i < entitiesToDelete[(int)EntityType.ANIMAL].Count; ++i)
        {
            // Check for the UUIDs in the actual entities list
            for (int j = entities[(int)EntityType.ANIMAL].Count - 1; j >= 0; j--)
            {
                // When there's a match, entity is removed
                Entity entityScript = entities[(int)EntityType.ANIMAL][j].GetComponent<Entity>();
                if (entitiesToDelete[(int)EntityType.ANIMAL][i] == entityScript.GetUUID())
                {
                    Destroy(entities[(int)EntityType.ANIMAL][j]);
                    entities[(int)EntityType.ANIMAL].RemoveAt(j);
                    continue;
                }
            }
        }
        #endregion

        #region Delete Food
        for (int i = 0; i < entitiesToDelete[(int)EntityType.FOOD].Count; ++i)
        {
            // Check for the UUIDs in the actual entities list
            for (int j = entities[(int)EntityType.FOOD].Count - 1; j >= 0; j--)
            {
                // When there's a match, entity is removed
                Entity entityScript = entities[(int)EntityType.FOOD][j].GetComponent<Entity>();
                if (entitiesToDelete[(int)EntityType.FOOD][i] == entityScript.GetUUID())
                {
                    Destroy(entities[(int)EntityType.FOOD][j]);
                    entities[(int)EntityType.FOOD].RemoveAt(j);
                    continue;
                }
            }
        }
        #endregion
    }

    private void SetInitialScene()
    {
        TryToSpawn(EntityType.TREE, treeManager.treePrefab, 0f, 0f, 0.0f);
        TryToSpawn(EntityType.TREE, treeManager.treePrefab, 100f, 100f, 0.0f);
        TryToSpawn(EntityType.TREE, treeManager.treePrefab, 200f, 200f, 0.0f);

        AnimalManager animalManager = GetComponent<AnimalManager>();
        TryToSpawn(EntityType.ANIMAL, animalManager.animalPrefab, 50f, 50f, 0.0f);
        TryToSpawn(EntityType.ANIMAL, animalManager.animalPrefab, 300f, 300f, 0.0f);
        TryToSpawn(EntityType.ANIMAL, animalManager.animalPrefab, 200f, 300f, 0.0f);
        TryToSpawn(EntityType.ANIMAL, animalManager.animalPrefab, 400f, 300f, 0.0f);
        TryToSpawn(EntityType.ANIMAL, animalManager.animalPrefab, 50f, 300f, 0.0f);
        TryToSpawn(EntityType.ANIMAL, animalManager.animalPrefab, 300f, 250f, 0.0f);
        TryToSpawn(EntityType.ANIMAL, animalManager.animalPrefab, 30f, 30f, 0.0f);
    }

    public void TryToSpawn(EntityType type, GameObject prefab, float x, float z, float randomVariation)
    {
        if (prefab == null) return;

        // Generate a spawn position
        Vector3 spawnPos = new Vector3(x, 0f, z);
        spawnPos.x += HeighmapData.GetRandomVariation(-randomVariation, randomVariation);
        spawnPos.z += HeighmapData.GetRandomVariation(-randomVariation, randomVariation);
        spawnPos = HeighmapData.GetTerrainHeight(spawnPos.x, spawnPos.z);

        if (!HeighmapData.isValidSpawnPoint(type, spawnPos)) return;

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

    public void TryToKill(EntityType type, string UUIDtoKill)
    {
        entitiesToDelete[(int)type].Add(UUIDtoKill);
    }

    private IEnumerator Reproduce() {
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
                    if (i == (int)EntityType.TREE)
                    {
                        TryToSpawn((EntityType)i, treeManager.treePrefab, entities[i][j].transform.position.x, entities[i][j].transform.position.z, 50f);
                    }
                    else if (i == (int)EntityType.ANIMAL)
                    {
                        TryToSpawn((EntityType)i, animalManager.animalPrefab, entities[i][j].transform.position.x, entities[i][j].transform.position.z, 2f);
                    }
                    else Debug.LogError("Reproduction of EntityType " + i + " is not being handled on EntityManager.cs on Reproduce()");
                }
            }
            yield return null;
        }
        Debug.Log("Trees: " + entities[(int)EntityType.TREE].Count);
        Debug.Log("Animals: " + entities[(int)EntityType.ANIMAL].Count);
        Debug.Log("Food: " + entities[(int)EntityType.FOOD].Count);
        yield return new WaitForSeconds(15f);
        StartCoroutine(Reproduce());
    }

    private IEnumerator GrowOrDie()
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
                    Entity entityScript = entities[i][j].GetComponent<Entity>();
                    TryToKill((EntityType)i, entityScript.GetUUID());
                }
                else ageController.age++;

                // Scale the tree in the world according to its age
                if (ageController.growsInSize) 
                {
                    float agePercentage = (float)(ageController.age) / (float)(ageController.maxAge);
                    entities[i][j].transform.localScale = Vector3.one * agePercentage;
                }
            }
            //yield return null;
        }
        yield return new WaitForSeconds(5f);
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
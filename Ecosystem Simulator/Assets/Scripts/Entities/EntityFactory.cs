using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityFactory : MonoBehaviour
{
    [SerializeField] private List<GameObject> animals = new List<GameObject>();
    [SerializeField] private List<GameObject> trees = new List<GameObject>();
    [SerializeField] private GameObject food = null;

    private Dictionary<int, GameObject> _idToAnimal;

    private EntityManager entityManager = null;

    private void Awake()
    {
        _idToAnimal = new Dictionary<int, GameObject>();

        foreach(var animal in animals)
        {
            Animal animalScript = animal.GetComponent<Animal>();
            _idToAnimal.Add((int)animalScript.species, animal);
        }

        entityManager = gameObject.GetComponent<EntityManager>();
    }

    private Vector3 GenerateSpawnPosition(float x, float z, float randomVariation) 
    {
        // Generate a spawn position
        Vector3 spawnPos = new Vector3(x, 0f, z);
        spawnPos.x += HeighmapData.Instance.GetRandomVariation(-randomVariation, randomVariation);
        spawnPos.z += HeighmapData.Instance.GetRandomVariation(-randomVariation, randomVariation);
        spawnPos = HeighmapData.Instance.GetTerrainHeight(spawnPos.x, spawnPos.z);

        //if (!HeighmapData.Instance.isValidSpawnPoint(type, spawnPos)) return; //TODO
        return spawnPos;
    }

    private void AddToEntitiesList(EntityManager.EntityType type, GameObject entityToAdd)
    {
        switch (type)
        {
            case EntityManager.EntityType.TREE:
                entityManager.entities[(int)EntityManager.EntityType.TREE].Add(entityToAdd);
                break;
            case EntityManager.EntityType.FOOD:
                entityManager.entities[(int)EntityManager.EntityType.FOOD].Add(entityToAdd);
                break;
            case EntityManager.EntityType.ANIMAL:
                entityManager.entities[(int)EntityManager.EntityType.ANIMAL].Add(entityToAdd);
                break;
            default:
                Debug.LogError("Entered default on AddToEntitiesList on EntityFactory.cs");
                break;
        }
    }

    public GameObject SpawnAnimalOfRandomGender(int speciesID, float x, float z, float randomVariation = 0)
    {
        if(!_idToAnimal.TryGetValue(speciesID, out GameObject animal))
        {
            throw new Exception($"Entity with speciesID {speciesID} does not exist");
        }

        Vector3 spawnPos = GenerateSpawnPosition(x, z, randomVariation);
        if (!HeighmapData.Instance.isValidSpawnPoint(EntityManager.EntityType.ANIMAL, spawnPos)) return null;

        GameObject newAnimal = Instantiate(animal, spawnPos, Quaternion.identity);
        AddToEntitiesList(EntityManager.EntityType.ANIMAL, newAnimal);

        return newAnimal;
    }

    public GameObject SpawnRandomTree(float x, float z, float randomVariation = 0, byte initialAge = 1)
    {
        Vector3 spawnPos = GenerateSpawnPosition(x, z, randomVariation);
        if (!HeighmapData.Instance.isValidSpawnPoint(EntityManager.EntityType.TREE, spawnPos)) return null;

        GameObject newTree = Instantiate(trees[UnityEngine.Random.Range(0, trees.Count - 1)], spawnPos, Quaternion.Euler(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f));
        AddToEntitiesList(EntityManager.EntityType.TREE, newTree);

        //Set age
        AgeController ageController = newTree.GetComponent<AgeController>();
        ageController.age = initialAge;

        return newTree;
    }

    public GameObject SpawnFood(float x, float z, float randomVariation = 0)
    {
        Vector3 spawnPos = GenerateSpawnPosition(x, z, randomVariation);
        if (!HeighmapData.Instance.isValidSpawnPoint(EntityManager.EntityType.TREE, spawnPos)) return null;

        GameObject newFood = Instantiate(food, spawnPos, Quaternion.identity);
        AddToEntitiesList(EntityManager.EntityType.FOOD, newFood);

        return newFood;
    }
}
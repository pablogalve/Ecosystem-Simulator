using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityFactory : MonoBehaviour
{
    [SerializeField] private List<GameObject> animals = new List<GameObject>();
    [SerializeField] private List<GameObject> trees = new List<GameObject>();
    [SerializeField] private GameObject food = null;

    private Dictionary<int, GameObject> _idToEntity;

    private EntityManager entityManager = null;
    [SerializeField] private Dictionary<int, Stack<GameObject>> _idToObjectPool = new Dictionary<int, Stack<GameObject>>();

    private void Awake()
    {
        _idToEntity = new Dictionary<int, GameObject>();

        foreach(var animal in animals)
        {
            Animal animalScript = animal.GetComponent<Animal>();
            _idToEntity.Add((int)animalScript.species, animal);

            Stack<GameObject> newPool = new Stack<GameObject>();
            _idToObjectPool.Add((int)animalScript.species, newPool);
        }

        int i = 100; // Random int that is not equal to any animal species. // TODO: This is a bad practice
        foreach(var tree in trees)
        {
            _idToEntity.Add(i, tree);
            
            Stack<GameObject> newPool = new Stack<GameObject>();
            _idToObjectPool.Add(i, newPool);

            if (i > 100) throw new Exception("At the moment this is hardcoded for 1 tree type max. Please refactor this");

            i++;
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
        EntityManager.Entity entity = new EntityManager.Entity();
        entity.UUID = System.Guid.NewGuid().ToString();
        entity.type = type;

        entityManager.UUIDs.Add(entity);
        entityManager.entitiesByType[(int)type].Add(entity.UUID);
        entityManager.entities[entity.UUID] = entityToAdd;
    }

    public GameObject SpawnAnimalOfRandomGender(int speciesID, float x, float z, float randomVariation = 0)
    {
        if(!_idToEntity.TryGetValue(speciesID, out GameObject animal))
        {
            throw new Exception($"Entity with speciesID {speciesID} does not exist");
        }

        Vector3 spawnPos = GenerateSpawnPosition(x, z, randomVariation);
        if (!HeighmapData.Instance.isValidSpawnPoint(EntityManager.EntityType.ANIMAL, spawnPos)) return null;

        GameObject newAnimal;
        if (!_idToObjectPool.TryGetValue(speciesID, out Stack<GameObject> objectsInPool)) 
        {
            throw new Exception($"Pool with speciesID {speciesID} does not exist");
        }

        if(objectsInPool.Count > 0) // Reuse a GO from pool
        {
            newAnimal = objectsInPool.Peek();
            newAnimal.SetActive(true);
            objectsInPool.Pop();

            Animal animalScript = newAnimal.GetComponent<Animal>();
            animalScript.OnSpawn();

            newAnimal.transform.position = spawnPos;
        }
        else // Instantiate a new GO
        {
            newAnimal = Instantiate(animal, spawnPos, Quaternion.identity);
        }
        
        AddToEntitiesList(EntityManager.EntityType.ANIMAL, newAnimal);

        return newAnimal;
    }

    public GameObject SpawnRandomTree(float x, float z, float randomVariation = 0, byte initialAge = 1)
    {
        Vector3 spawnPos = GenerateSpawnPosition(x, z, randomVariation);
        if (!HeighmapData.Instance.isValidSpawnPoint(EntityManager.EntityType.TREE, spawnPos)) return null;
           
        GameObject newTree;
        if (!_idToObjectPool.TryGetValue(100, out Stack<GameObject> objectsInPool))
        {
            throw new Exception($"Pool with speciesID {100} does not exist");
        }

        if (objectsInPool.Count > 0) // Reuse a GO from pool
        {
            newTree = objectsInPool.Peek();
            newTree.SetActive(true);
            objectsInPool.Pop();

            newTree.transform.position = spawnPos;
        }
        else // Instantiate a new GO
        {
            newTree = Instantiate(trees[UnityEngine.Random.Range(0, trees.Count - 1)], spawnPos, Quaternion.Euler(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f));
        }

        //Set age
        AgeController ageController = newTree.GetComponent<AgeController>();
        ageController.age = initialAge;
        ageController.OnSpawn();

        AddToEntitiesList(EntityManager.EntityType.TREE, newTree);

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

    public int GetAmountOfUniqueSpecies(EntityManager.EntityType type)
    {
        switch (type)
        {
            case EntityManager.EntityType.TREE:
                return trees.Count;
            case EntityManager.EntityType.FOOD:
                return 1;
            case EntityManager.EntityType.ANIMAL:
                return animals.Count;
            default:
                throw new Exception($"Entity of type '{type}' is not being managed on the switch statement");
        }
    }
}
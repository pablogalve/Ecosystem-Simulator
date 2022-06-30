using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityFactory : MonoBehaviour
{
    [SerializeField] private List<GameObject> maleAnimals = new List<GameObject>();
    [SerializeField] private List<GameObject> femaleAnimals = new List<GameObject>();
    [SerializeField] private List<GameObject> trees = new List<GameObject>();
    [SerializeField] private GameObject food = null;    

    // Key used to encode the key in the dictionaries
    private int maleKey = 1000;
    private int femaleKey = 0;

    private Dictionary<int, GameObject> _idToEntity;
    private Dictionary<int, Stack<GameObject>> _idToObjectPool = new Dictionary<int, Stack<GameObject>>();

    public Dictionary<int, int> animalSpeciesAmount = new Dictionary<int, int>();

    private EntityManager entityManager = null;

    private void Awake()
    {
        _idToEntity = new Dictionary<int, GameObject>();

        /*foreach (var animal in maleAnimals)
        {
            Animal animalScript = animal.GetComponent<Animal>();
            Stack<GameObject> newPool = new Stack<GameObject>();

            int key = (int)animalScript.species + maleKey;

            _idToEntity.Add(key, animal);
            _idToObjectPool.Add(key, newPool);
        }

        foreach (var animal in femaleAnimals)
        {
            Animal animalScript = animal.GetComponent<Animal>();
            Stack<GameObject> newPool = new Stack<GameObject>();

            int key = (int)animalScript.species + femaleKey;

            _idToEntity.Add(key, animal);
            _idToObjectPool.Add(key, newPool);
        }*/

        int i = 100; // Random int that is not equal to any animal species. // TODO: This is a bad practice
        foreach(var tree in trees)
        {
            _idToEntity.Add(i, tree);
            
            Stack<GameObject> newPool = new Stack<GameObject>();
            _idToObjectPool.Add(i, newPool);

            if (i > 100) throw new Exception("At the moment this is hardcoded for 1 tree type max. Please refactor this");

            i++;
        }

        Stack<GameObject> newFoodPool = new Stack<GameObject>();
        _idToObjectPool.Add(200, newFoodPool); // TODO: Don't hardcode the food key if there are more than one type of food

        entityManager = gameObject.GetComponent<EntityManager>();        

        for(int j = 0; j < GetAmountOfUniqueSpecies(EntityManager.EntityType.ANIMAL); j++)
        {
            animalSpeciesAmount[j] = 0;
        }
    }

    private Vector3 GenerateSpawnPosition(float x, float z, float randomVariation) 
    {
        // Generate a spawn position
        Vector3 spawnPos = new Vector3(x, 0f, z);
        spawnPos.x += HeightmapData.Instance.GetRandomVariation(-randomVariation, randomVariation);
        spawnPos.z += HeightmapData.Instance.GetRandomVariation(-randomVariation, randomVariation);
        spawnPos.y = HeightmapData.Instance.GetTerrainHeight(spawnPos.x, spawnPos.z);

        //if (!HeighmapData.Instance.isValidSpawnPoint(type, spawnPos)) return; //TODO
        return spawnPos;
    }

    private void AddToEntitiesList(EntityManager.EntityType type, GameObject entityToAdd)
    {
        EntityManager.Entity entity = new EntityManager.Entity();
        entity.UUID = System.Guid.NewGuid().ToString();
        entity.type = type;

        entityManager.UUIDs.AddFirst(entity); // O(1)
        entityManager.entitiesByType[(int)type].Add(entity.UUID);
        entityManager.entities[entity.UUID] = entityToAdd;

        UUID uuid = entityToAdd.GetComponent<UUID>();
        uuid.SetMyUUIDInfo(entityManager.UUIDs.First);
    }

    public void AddToPool(int species, GameObject gameObject)
    {
        _idToObjectPool[species].Push(gameObject);
    }

    public GameObject SpawnAnimalOfRandomGender(int speciesID, float x, float z, float randomVariation = 0, bool randomAge = false)
    {
        if (entityManager.isMaxCapReached(EntityManager.EntityType.ANIMAL)) return null;

        Vector3 spawnPos = GenerateSpawnPosition(x, z, randomVariation);
        if (!HeightmapData.Instance.IsValidPosition(EntityManager.EntityType.ANIMAL, spawnPos)) return null;

        animalSpeciesAmount[speciesID - 1]++;

        byte gender = (byte)UnityEngine.Random.Range(0, 2);
        if (gender == 0) speciesID += femaleKey;
        else if (gender == 1) speciesID += maleKey;

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

            newAnimal.transform.position = spawnPos;            
        }
        else // Instantiate a new GO
        {
            if (!_idToEntity.TryGetValue(speciesID, out GameObject animal))
            {
                throw new Exception($"Entity with speciesID {speciesID} does not exist");
            }

            newAnimal = Instantiate(animal, spawnPos, Quaternion.identity);
        }

        Animal animalScript = newAnimal.GetComponent<Animal>();
        animalScript.OnSpawn();
        Reproduction reproduction = newAnimal.GetComponent<Reproduction>();
        reproduction.gender = gender;

        if(randomAge)
        {
            AgeController ageController = newAnimal.GetComponent<AgeController>();
            int newAge = UnityEngine.Random.Range(0, ageController.maxAge - 1);
            ageController.age = (byte)newAge;
        }
        else
        {
            AgeController ageController = newAnimal.GetComponent<AgeController>();
            ageController.age = 1;
        }

        AddToEntitiesList(EntityManager.EntityType.ANIMAL, newAnimal);              

        return newAnimal;
    }

    public GameObject SpawnAnimalOfRandomGenderAndAge(int speciesID, float x, float z, float randomVariation = 0)
    {
        GameObject newAnimal = SpawnAnimalOfRandomGender(speciesID, x, z, randomVariation, true);

        return newAnimal;
    }

    public GameObject SpawnRandomTree(float x, float z, float randomVariation = 0, byte initialAge = 1)
    {
        if (entityManager.isMaxCapReached(EntityManager.EntityType.TREE)) return null;

        Vector3 spawnPos = GenerateSpawnPosition(x, z, randomVariation);
        //if (!HeightmapData.Instance.IsValidPosition(EntityManager.EntityType.TREE, spawnPos)) return null;
           
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
        if (entityManager.isMaxCapReached(EntityManager.EntityType.FOOD)) return null;

        Vector3 spawnPos = GenerateSpawnPosition(x, z, randomVariation);

        {
            // Food shouldn't be on the tree
            if (spawnPos.x - x < 1.0f && spawnPos.x - x > 1.0f) spawnPos.x = x + 2.0f;
            if (spawnPos.z - z < 1.0f && spawnPos.z - z > 1.0f) spawnPos.z = z + 2.0f;
        }

        if (!HeightmapData.Instance.IsValidPosition(EntityManager.EntityType.FOOD, spawnPos)) return null;

        GameObject newFood;
        if (!_idToObjectPool.TryGetValue(200, out Stack<GameObject> objectsInPool))
        {
            throw new Exception($"Pool with speciesID {200} does not exist");
        }

        if (objectsInPool.Count > 0) // Reuse a GO from pool
        {
            newFood = objectsInPool.Peek();
            newFood.SetActive(true);
            objectsInPool.Pop();

            newFood.transform.position = spawnPos;
        }
        else // Instantiate a new GO
        {
            newFood = Instantiate(food, spawnPos, Quaternion.identity);
        }

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
                return maleAnimals.Count;
            default:
                throw new Exception($"Entity of type '{type}' is not being managed on the switch statement");
        }
    }

    public int GetCurrentAmountOfAnimal(AnimalManager.Species species)
    {
        return animalSpeciesAmount[(int)species - 1];
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public struct Entity
    {
        public EntityType type;
        public string UUID;
    }
    public enum EntityType
    {
        TREE,
        FOOD,
        ANIMAL
    }

    public List<Entity> UUIDs = new List<Entity>(); // UUIDs of all existing entities
    public Dictionary<string, GameObject> entities = new Dictionary<string, GameObject>();
    public Dictionary<int, List<string>> entitiesByType = new Dictionary<int, List<string>>(); // int = species or type, List<string> All the entities of the given species
    private Queue<Entity> entitiesToDelete = new Queue<Entity>();

    public int maxTrees = 2000;
    public int maxFood = 5000;
    public int maxAnimals = 100; 

    // Acess to other scripts
    AnimalManager animalManager = null;
    TreeManager treeManager = null;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        for (int i = 0; i < 3; ++i)
        {
            var list = new List<string>();
            entitiesByType.Add(i, list);
        }
    }

    void Start()
    {
        animalManager = GetComponent<AnimalManager>();
        treeManager = GetComponent<TreeManager>();      

        SetInitialScene();

        StartCoroutine(GrowOrDie());        
    }

    // LateUpdate is called after all Update functions have been called. It should be used to delete gameObjects
    private void LateUpdate()
    {
        DeleteEntities(1);
    }

    private void DeleteEntity()
    {
        Entity entity = entitiesToDelete.Peek();

        if (entities.ContainsKey(entity.UUID)) // O(1)
        {
            GameObject go = entities[entity.UUID];
            Destroy(go);

            UUIDs.Remove(entity);
            entities.Remove(entity.UUID);
            entitiesByType[(int)entity.type].Remove(entity.UUID);
        }
        else
        {
            Debug.LogWarning($"Couldnt find entity of type '{entitiesToDelete.Peek().type}' with UUID '{entitiesToDelete.Peek().UUID}' on DeleteEntity");
        }
        entitiesToDelete.Dequeue();
    }

    private void DeleteEntities(int maxOfEachTypeToDeleteInThisFrame)
    {
        // Delete all gameObjects waiting to be removed at the end of the frame
        for(int i = 0; i < maxOfEachTypeToDeleteInThisFrame; ++i)
        {
            if (entitiesToDelete.Count < 1) return;
            DeleteEntity();
        }
    }

    private void SetInitialScene()
    {
        EntityFactory entityFactory = gameObject.GetComponent<EntityFactory>();
        for (int i = 0; i < 50; i++) {
            entityFactory.SpawnRandomTree(1000f, 1000f, 500f, 12);
        }

        for (int i = 0; i < 150; i++)
        {            
            entityFactory.SpawnAnimalOfRandomGender((int)AnimalManager.Species.K, 1000f, 1000f, 500f);
            entityFactory.SpawnAnimalOfRandomGender((int)AnimalManager.Species.R, 1000f, 1000f, 500f);
        }
    }

    public void TryToKill(EntityType type, string UUIDtoKill)
    {
        Entity entity = new Entity();
        entity.type = type;
        entity.UUID = UUIDtoKill;
        entitiesToDelete.Enqueue(entity);
    }
      
    // Updates entity's age, grows it in size, and kills if it it's too old
    private IEnumerator GrowOrDie()
    {
        for(int i = 0; i < UUIDs.Count; ++i)
        {
            AgeController ageController = entities[UUIDs[i].UUID].GetComponent<AgeController>();
            if (ageController == null) throw new Exception("AgeController script is null on EntityManager.cs on GrowOrDie");

            // Kill entity if it has reached it's maximum age
            if (ageController.age + 1 > ageController.maxAge)
            {
                TryToKill((EntityType)i, UUIDs[i].UUID);
            }
            else ageController.age++;

            // Scale the entity in the world according to its age
            if (ageController.growsInSize)
            {
                float agePercentage = (float)(ageController.age) / (float)(ageController.maxAge);
                entities[UUIDs[i].UUID].transform.localScale = Vector3.one * agePercentage;
            }
        }
        yield return new WaitForSeconds(30f);
        StartCoroutine(GrowOrDie());
    }

    public bool isMaxCapReached(EntityType type)
    {
        // If max cap of an entity type is reached, it must be skipped to save performance
        int maxAmount;
        switch (type)
        {
            case EntityType.TREE: maxAmount = maxTrees; break;
            case EntityType.FOOD: maxAmount = maxFood; break;
            case EntityType.ANIMAL: maxAmount = maxAnimals; break;
            default: throw new Exception("type is not being managed on isMaxCapReached()");
        }

        if (entitiesByType[(int)type].Count < maxAmount) return false;
        else return true;
    }
}
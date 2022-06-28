using System;
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

    public struct Entity
    {
        public EntityType type;
        public string UUID;
    }

    public LinkedList<Entity> UUIDs = new LinkedList<Entity>(); // UUIDs of all existing entities
    public Dictionary<string, GameObject> entities = new Dictionary<string, GameObject>();
    public Dictionary<int, List<string>> entitiesByType = new Dictionary<int, List<string>>(); // int = species or type, List<string> All the entities of the given species
    private Queue<LinkedListNode<Entity>> entitiesToDelete = new Queue<LinkedListNode<Entity>>();

    public int maxTrees = 2000;
    public int maxFood = 5000;
    public int maxAnimals = 100;

    public int maxEntitiesToDeletePerFrame = 10;

    // Acess to other scripts
    private EntityFactory entityFactory = null;
    private GPUInstancedRendering treeInstancedRenderer;

    private void Awake()
    {
        for (int i = 0; i < 3; ++i)
        {
            var list = new List<string>();
            entitiesByType.Add(i, list);
        }

        treeInstancedRenderer = GameObject.Find("TreeRenderer").GetComponent<GPUInstancedRendering>();
    }

    void Start()
    {
        entityFactory = GetComponent<EntityFactory>();

        SetInitialScene();

        StartCoroutine(GrowOrDie());        
    }

    // LateUpdate is called after all Update functions have been called. It should be used to delete gameObjects
    private void LateUpdate()
    {
        DeleteEntities(maxEntitiesToDeletePerFrame);
    }

    private void DeleteEntity()
    {
        LinkedListNode<Entity> entityNode = entitiesToDelete.Peek();

        if (entities.ContainsKey(entityNode.Value.UUID)) // O(n) // TODO: Should I avoid this to optimize?
        {
            GameObject go = entities[entityNode.Value.UUID];

            UUIDs.Remove(entityNode); // O(1) when removing a 'LinkedListNode' instead of 'T' https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.linkedlist-1.remove?view=netframework-4.7.2
            entities.Remove(entityNode.Value.UUID); // O(1)
            entitiesByType[(int)entityNode.Value.type].Remove(entityNode.Value.UUID); // O(1)            

            // Add to object pooling
            if (entityNode.Value.type == EntityType.ANIMAL)
            {
                entityFactory.animalSpeciesAmount[(int)go.GetComponent<Animal>().species - 1]--;
                entityFactory.AddToPool((int)go.GetComponent<Animal>().species, go);
            }
                
            else if (entityNode.Value.type == EntityType.TREE)
                entityFactory.AddToPool(100, go);
            else if (entityNode.Value.type == EntityType.FOOD)
                entityFactory.AddToPool(200, go);

            go.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"Couldnt find entity of type '{entityNode.Value.type}' with UUID '{entityNode.Value.UUID}' on DeleteEntity");
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

        for (int i = 0; i < 1000; i++) 
        {
            entityFactory.SpawnRandomTree(1000f, 1000f, 500f, 12);
        }
        
        for (int i = 0; i < 0; i++)
        {
            entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.LONGHORN, 1000f, 1000f, 300f);
        }
        for (int i = 0; i < 1000; i++)
        {
            entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.SHEEP, 1000f, 1000f, 100f);
        }
        for (int i = 0; i < 0; i++)
        {
            entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.WOLF, 500f, 1000f, 300f);
        }        
    }

    public void TryToKill(LinkedListNode<Entity> entityNode)
    {
        entitiesToDelete.Enqueue(entityNode);
    }
      
    // Updates entity's age, grows it in size, and kills if it it's too old
    private IEnumerator GrowOrDie()
    {
        // Declare transformation data to send to the instanced renderer
        List<Matrix4x4> matrices = new List<Matrix4x4>();

        for(LinkedListNode<Entity> entity = UUIDs.First; entity != null; entity = entity.Next)
        {
            AgeController ageController = entities[entity.Value.UUID].GetComponent<AgeController>();
            if (ageController == null) throw new Exception("AgeController script is null on EntityManager.cs on GrowOrDie");

            // Kill entity if it has reached it's maximum age
            if (ageController.age + 1 > ageController.maxAge)
            {
                TryToKill(entity);
                continue;
            }
            else ageController.age++;

            // Scale the entity in the world according to its age
            if (ageController.growsInSize && ageController.IsBaby())
            {
                float agePercentage = (float)(ageController.age) / (float)(ageController.maxAge);
                entities[entity.Value.UUID].transform.localScale = Vector3.one * agePercentage * ageController.scaleFactor;
            }
            else
            {
                entities[entity.Value.UUID].transform.localScale = Vector3.one * ageController.scaleFactor;
            }

            // Save data for the instanced renderer
            if(entity.Value.type != EntityType.ANIMAL)
            {
                GameObject go = entities[entity.Value.UUID].gameObject;
                Matrix4x4 matrix = Matrix4x4.TRS(
                    pos: go.transform.position,
                    q: go.transform.rotation,
                    s: go.transform.localScale
                    );
                matrices.Add(matrix);
            }
        }

        // Send data to the tree instanced renderer
        treeInstancedRenderer.RecalculateMatrices(matrices);

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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        #region Sandbox SetInitialScene
        if(SceneManager.GetActiveScene().name == "Sandbox")
        {
            for (int i = 0; i < 10000; i++)
            {
                entityFactory.SpawnRandomTree(1000f, 1000f, 1000f, 1);
                entityFactory.SpawnRandomTree(1000f, 1000f, 1000f, 5);
                entityFactory.SpawnRandomTree(1000f, 1000f, 1000f, 10);
                entityFactory.SpawnRandomTree(1000f, 1000f, 1000f, 40);
            }

            for (int i = 0; i < 350; i++)
            {
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.LONGHORN, 500f, 500f, 50f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.LONGHORN, 600f, 600f, 50f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.LONGHORN, 1200f, 600f, 50f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.LONGHORN, 800f, 800f, 50f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.LONGHORN, 1200f, 1200f, 50f);
            }
            for (int i = 0; i < 350; i++)
            {
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.SHEEP, 500f, 1200f, 100f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.SHEEP, 700f, 700f, 100f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.SHEEP, 1200f, 500f, 100f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.SHEEP, 500f, 1200f, 100f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.SHEEP, 700f, 600f, 100f);
            }
            for (int i = 0; i < 350; i++)
            {
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.WOLF, 600f, 600f, 100f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.WOLF, 700f, 700f, 100f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.WOLF, 500f, 850f, 100f);
            }
        }
        #endregion
        #region Simulator SetInitialScene
        else if (SceneManager.GetActiveScene().name == "Simulator")
        {
            for (int i = 0; i < 10000; i++)
            {
                entityFactory.SpawnRandomTree(1000f, 1000f, 1000f, 1);
                entityFactory.SpawnRandomTree(1000f, 1000f, 1000f, 5);
                entityFactory.SpawnRandomTree(1000f, 1000f, 1000f, 10);
            }

            for (int i = 0; i < 200; i++)
            {
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.LONGHORN, 1100f, 1500f, 50f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.LONGHORN, 600f, 600f, 50f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.LONGHORN, 1200f, 600f, 50f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.LONGHORN, 800f, 800f, 50f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.LONGHORN, 1200f, 1200f, 50f);
            }
            for (int i = 0; i < 200; i++)
            {
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.SHEEP, 800f, 1200f, 50f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.SHEEP, 700f, 700f, 50f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.SHEEP, 1200f, 500f, 50f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.SHEEP, 1300f, 1200f, 50f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.SHEEP, 700f, 1000f, 50f);
            }
            for (int i = 0; i < 200; i++)
            {
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.WOLF, 1000f, 1000f, 50f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.WOLF, 700f, 700f, 50f);
                entityFactory.SpawnAnimalOfRandomGenderAndAge((int)AnimalManager.Species.WOLF, 600f, 850f, 50f);
            }
        }
        #endregion
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
                entities[entity.Value.UUID].transform.localScale = Vector3.one * (0.5f + agePercentage) * ageController.scaleFactor;
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
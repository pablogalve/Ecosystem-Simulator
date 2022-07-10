using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    private EntityManager entityManager = null;
    private EntityFactory entityFactory = null;
    private GPUInstancedRendering foodInstancedRenderer = null;

    public int maxUpdatesPerFrame = 50;

    void Start()
    {
        entityManager = GetComponent<EntityManager>();
        entityFactory = GetComponent<EntityFactory>();
        foodInstancedRenderer = GameObject.Find("FoodRenderer").GetComponent<GPUInstancedRendering>();

        StartCoroutine(GenerateFood());
        StartCoroutine(AsexualReproduction());
        StartCoroutine(SendFoodMatricesToInstancedRenderer());
    }

    private IEnumerator GenerateFood()
    {
        if (entityManager == null) throw new System.Exception("entityManager was null on AnimalManager.cs on GenerateFood()");

        for (int i = 0; i < entityManager.entitiesByType[(int)EntityManager.EntityType.TREE].Count - 1; i++)
        {  
            GameObject tree = entityManager.entities[entityManager.entitiesByType[(int)EntityManager.EntityType.TREE][i]];

            if (entityManager.isMaxCapReached(EntityManager.EntityType.FOOD)) break;

            AgeController ageScript = tree.GetComponent<AgeController>();
            if (ageScript == null) throw new System.Exception("tree " + i + " is null on TreeManager.cs: FoodGeneration");               

            if (!ageScript.IsBaby())
            {
                entityFactory.SpawnFood(tree.transform.position.x, tree.transform.position.z, 20f);
            }

            if (i != 0 && i % maxUpdatesPerFrame == 0)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(15.0f); // Wait before repeating the cycle
        StartCoroutine(GenerateFood());
    }

    private IEnumerator AsexualReproduction()
    {
        if (entityManager == null) throw new System.Exception("entityManager was null on AnimalManager.cs on GenerateFood()");

        for (int i = 0; i < entityManager.entitiesByType[(int)EntityManager.EntityType.TREE].Count - 1; i++)
        {
            if (i != 0 && i % maxUpdatesPerFrame == 0)
            {
                yield return null;
            }
                        
            GameObject tree = entityManager.entities[entityManager.entitiesByType[(int)EntityManager.EntityType.TREE][i]];
            
            AgeController ageController = tree.GetComponent<AgeController>();
            if (ageController == null) throw new System.Exception("AgeController is null on EntityManager.cs: Reproduce()");
            
            if (ageController.IsBaby() == false)
            {
                entityFactory.SpawnRandomTree(
                    tree.transform.position.x,
                    tree.transform.position.z,
                    50f);
            }
        }

        yield return new WaitForSeconds(15f);
        StartCoroutine(AsexualReproduction());
    }

    private IEnumerator SendFoodMatricesToInstancedRenderer()
    {
        if (foodInstancedRenderer == null) throw new System.Exception("entityManager was null on AnimalManager.cs on GenerateFood()");

        // Declare transformation data to send to the instanced renderer
        List<Matrix4x4> matrices = new List<Matrix4x4>();

        for (int i = 0; i < entityManager.entitiesByType[(int)EntityManager.EntityType.FOOD].Count - 1; i++)
        {
            GameObject go = entityManager.entities[entityManager.entitiesByType[(int)EntityManager.EntityType.FOOD][i]];
            Matrix4x4 matrix = Matrix4x4.TRS(
                pos: go.transform.position,
                q: go.transform.rotation,
                s: go.transform.localScale
                );
            matrices.Add(matrix);
        }

        foodInstancedRenderer.RecalculateMatrices(matrices);

        yield return new WaitForSeconds(1f);
        StartCoroutine(SendFoodMatricesToInstancedRenderer());
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public GameObject treePrefab = null;
    EntityManager entityManager = null;

    // FOOD
    public GameObject foodPrefab = null;

    void Start()
    {
        entityManager = GetComponent<EntityManager>();
        StartCoroutine(GenerateFood());
        StartCoroutine(AsexualReproduction());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator GenerateFood()
    {
        if (entityManager == null) Debug.LogError("entityManager was null on TreeManager.cs on GenerateFood()");
        List<GameObject> trees = null;
        if (entityManager.entities.Count > 0)
        {
            trees = entityManager.entities[(int)EntityManager.EntityType.TREE];
        }
        if (trees != null) 
        {
            for (int i = 0; i < trees.Count; i++)
            {
                if (entityManager.isMaxCapReached(EntityManager.EntityType.FOOD)) break;

                AgeController ageScript = trees[i].GetComponent<AgeController>();
                if (ageScript == null)
                {
                    Debug.LogError("tree " + i + " is null on TreeManager.cs: FoodGeneration");
                    continue;
                }

                // Check that tree is old enough to produce food
                if (ageScript.age >= ageScript.reproductionAge * ageScript.maxAge)
                {
                    entityManager.TryToSpawn(EntityManager.EntityType.FOOD, foodPrefab, trees[i].transform.position.x, trees[i].transform.position.z, 2f);
                }
            }
        }

        yield return new WaitForSeconds(35.0f); // Wait before repeating the cycle
        StartCoroutine(GenerateFood());
    }

    private IEnumerator AsexualReproduction()
    {        
        if (entityManager.isMaxCapReached(EntityManager.EntityType.TREE)) yield return null; // We can't have infinite entities for performance issues

        // Iterate all the entities of the same type
        for (int j = 0; j < entityManager.entities[(int)EntityManager.EntityType.TREE].Count; j++)
        {
            if (entityManager.isMaxCapReached(EntityManager.EntityType.TREE)) break;

            AgeController ageController = entityManager.entities[(int)EntityManager.EntityType.TREE][j].GetComponent<AgeController>();
            if (ageController == null)
            {
                Debug.LogError("AgeController is null on EntityManager.cs: Reproduce()");
                continue;
            }

            if (ageController.IsBaby() == false)
            {
                entityManager.TryToSpawn(EntityManager.EntityType.TREE, 
                                        treePrefab, 
                                        entityManager.entities[(int)EntityManager.EntityType.TREE][j].transform.position.x, 
                                        entityManager.entities[(int)EntityManager.EntityType.TREE][j].transform.position.z, 
                                        50f);
            }
        }

        yield return new WaitForSeconds(15f);
        StartCoroutine(AsexualReproduction());
    }
}
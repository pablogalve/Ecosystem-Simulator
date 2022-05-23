using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public GameObject treePrefab = null;
    private EntityManager entityManager = null;
    private EntityFactory entityFactory = null;

    public int maxUpdatesPerFrame = 50;

    void Start()
    {
        entityManager = GetComponent<EntityManager>();
        entityFactory = GetComponent<EntityFactory>();

        StartCoroutine(GenerateFood());
        StartCoroutine(AsexualReproduction());
    }

    private IEnumerator GenerateFood()
    {
        if (entityManager == null) throw new System.Exception("entityManager was null on AnimalManager.cs on GenerateFood()");

        for (int i = 0; i < entityManager.entitiesByType[(int)EntityManager.EntityType.TREE].Count; i++)
        {
            if (i != 0 && i % maxUpdatesPerFrame == 0)
                yield return null;

            GameObject tree = entityManager.entities[entityManager.entitiesByType[(int)EntityManager.EntityType.TREE][i]];

            if (entityManager.isMaxCapReached(EntityManager.EntityType.FOOD)) break;

            AgeController ageScript = tree.GetComponent<AgeController>();
            if (ageScript == null) throw new System.Exception("tree " + i + " is null on TreeManager.cs: FoodGeneration");               

            // Check that tree is old enough to produce food
            if (ageScript.age >= ageScript.reproductionAge * ageScript.maxAge)
            {
                entityFactory.SpawnFood(tree.transform.position.x, tree.transform.position.z, 2f);
            }
        }

        yield return new WaitForSeconds(35.0f); // Wait before repeating the cycle
        StartCoroutine(GenerateFood());
    }

    private IEnumerator AsexualReproduction()
    {
        if (entityManager == null) throw new System.Exception("entityManager was null on AnimalManager.cs on GenerateFood()");

        for (int i = 0; i < entityManager.entitiesByType[(int)EntityManager.EntityType.TREE].Count; i++)
        {
            if (i != 0 && i % maxUpdatesPerFrame == 0)
                yield return null;

            GameObject tree = entityManager.entities[entityManager.entitiesByType[(int)EntityManager.EntityType.TREE][i]];

            // Possible bug? Only the trees at the top of the list are being reproduced when they are close to the maximum population
            if (entityManager.isMaxCapReached(EntityManager.EntityType.TREE)) break;

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
}
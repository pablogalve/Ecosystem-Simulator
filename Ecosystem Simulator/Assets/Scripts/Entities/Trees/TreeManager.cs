using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public GameObject treePrefab = null;
    EntityManager entityManager = null;
    EntityFactory entityFactory = null;

    // FOOD
    public GameObject foodPrefab = null;

    void Start()
    {
        entityManager = GetComponent<EntityManager>();
        entityFactory = GetComponent<EntityFactory>();

        StartCoroutine(GenerateFood());
        StartCoroutine(AsexualReproduction());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator GenerateFood()
    {
        if (entityManager == null) throw new System.Exception("entityManager was null on AnimalManager.cs on GenerateFood()");

        for (int i = 0; i < entityManager.UUIDs.Count; i++)
        {
            if (entityManager.UUIDs[i].type != EntityManager.EntityType.TREE) continue;
            GameObject tree = entityManager.entities[entityManager.UUIDs[i].UUID];

            if (entityManager.isMaxCapReached(EntityManager.EntityType.FOOD)) break;

            AgeController ageScript = tree.GetComponent<AgeController>();
            if (ageScript == null)
            {
                Debug.LogError("tree " + i + " is null on TreeManager.cs: FoodGeneration");
                continue;
            }

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

        for (int i = 0; i < entityManager.UUIDs.Count; i++)
        {
            if (entityManager.UUIDs[i].type != EntityManager.EntityType.TREE) continue;
            GameObject tree = entityManager.entities[entityManager.UUIDs[i].UUID];

            // Possible bug? Only the trees at the top of the list are being reproduced when they are close to the maximum population
            if (entityManager.isMaxCapReached(EntityManager.EntityType.TREE)) break;

            AgeController ageController = tree.GetComponent<AgeController>();
            if (ageController == null)
            {
                Debug.LogError("AgeController is null on EntityManager.cs: Reproduce()");
                continue;
            }

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
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
                if (entityManager.isMaxCapReached(EntityManager.EntityType.FOOD, entityManager.entities[(int)EntityManager.EntityType.FOOD].Count) == true) break;

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

        yield return new WaitForSeconds(10.0f); // Wait before repeating the cycle
        StartCoroutine(GenerateFood());
    }
}
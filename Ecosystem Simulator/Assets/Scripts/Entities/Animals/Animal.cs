using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : Entity
{
    // FOOD and HUNGER
    EntityManager entityManager = null;
    public float hunger = 180f;
    public float minDistanceToEat = 1f;

    NavMeshAgent myNavMeshAgent;    

    // Start is called before the first frame update
    void Start()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        entityManager = gameManager.GetComponent<EntityManager>();
    }

    // Update is called once per frame
    void Update()
    {
        hunger -= Time.deltaTime;

        if(hunger <= 0)
        {
            Destroy(gameObject);
        }

        MoveToFood();
    }

    void MoveToFood()
    {
        Transform foodPos = FindClosestFood();

        if(foodPos != null)
        {
            myNavMeshAgent.SetDestination(foodPos.position);
        }
    }

    void Eat(GameObject food)
    {
        if(food == null)
        {
            Debug.LogWarning("food is null on Animal.cs. Eat()");
            return;
        }

        Entity entityScript = food.GetComponent<Entity>();
        if (entityScript == null)
        {
            Debug.LogWarning("entityScript is null on Animal.cs. Eat()");
            return;
        }

        if (entityManager.TryToKill(EntityManager.EntityType.FOOD, entityScript.GetUUID())) {
            hunger += 30f;
        }
    }

    Transform FindClosestFood()
    {
        // Create local list because it is not guaranteed that "foodManager.foodList" will not change while it is being iterated
        List<GameObject> foodList = entityManager.entities[(int)EntityManager.EntityType.FOOD];

        int indexOfClosestFood = -1;
        float smallestDistance = float.MaxValue;

        for(int i = 0; i < foodList.Count; i++)
        {
            // Food died before we could arrive
            if (foodList[i] == null) continue;

            float distance = Vector3.Distance(foodList[i].transform.position, gameObject.transform.position);

            // If food is found at an eatable distance, then animal stops looking for other food
            if(distance <= minDistanceToEat)
            {
                Eat(foodList[i]);
                break;
            }

            if (distance < smallestDistance)
            {
                indexOfClosestFood = i;
                smallestDistance = distance;
            }
        }

        if (indexOfClosestFood == -1) return null;
        else return foodList[indexOfClosestFood].transform;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    // FOOD and HUNGER
    public float hunger = 50;
    FoodManager foodManager;

    NavMeshAgent myNavMeshAgent;    

    // Start is called before the first frame update
    void Start()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        foodManager = gameManager.GetComponent<FoodManager>();
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

    Transform FindClosestFood()
    {
        // Create local list because it is not guaranteed that "foodManager.foodList" will not change while it is being iterated
        List<GameObject> foodList = foodManager.foodList;

        int indexOfClosestFood = -1;
        float smallestDistance = float.MaxValue;

        for(int i = 0; i < foodList.Count; i++)
        {
            float distance = Vector3.Distance(foodList[i].transform.position, gameObject.transform.position);
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

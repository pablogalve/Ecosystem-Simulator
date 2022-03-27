using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalManager : MonoBehaviour
{
    public enum States { 
        IDLE,
        LOOKING_FOR_FOOD,
        LOOKING_FOR_MATE,
        FOLLOWING_MUM,
    }

    public GameObject animalPrefab;
    EntityManager entityManager = null;

    // Start is called before the first frame update
    void Start()
    {
        entityManager = GetComponent<EntityManager>();

        StartCoroutine(UpdateAnimalsStateMachine());
        StartCoroutine(UpdateStats());
    }

    private IEnumerator UpdateStats()
    {
        if (entityManager == null) Debug.LogError("entityManager was null on AnimalManager.cs on UpdateStats()");
        List<GameObject> animals = null;
        if (entityManager.entities.Count > 0)
        {
            animals = entityManager.entities[(int)EntityManager.EntityType.ANIMAL];
        }
        if (animals == null)Debug.LogError("animals list was null on AnimalManager.cs");

        for (int i = 0; i < animals.Count; i++)
        {
            Animal animalScript = animals[i].GetComponent<Animal>();
            if (animalScript == null) Debug.LogError("animalScript list was null on UpdateStats.cs");

            animalScript.UpdateAllStats();
        }
        
        yield return new WaitForSeconds(15.0f); // Wait before repeating the cycle
        StartCoroutine(UpdateStats());
    }

    private IEnumerator UpdateAnimalsStateMachine()
    {
        if (entityManager == null) Debug.LogError("entityManager was null on AnimalManager.cs on UpdateAnimalsStateMachine()");
        List<GameObject> animals = null;
        if (entityManager.entities.Count > 0)
        {
            animals = entityManager.entities[(int)EntityManager.EntityType.ANIMAL];
        }
        if (animals == null)
        {
            Debug.LogError("animals list was null on AnimalManager.cs");
            yield return null;
        }

        for (int i = 0; i < animals.Count; i++) 
        {
            Animal animalScript = animals[i].GetComponent<Animal>();
            if(animalScript == null) Debug.LogError("animalScript list was null on AnimalManager.cs");

            switch (animalScript.state) 
            {
                case States.IDLE: // Default state when there are no other needs
                    if (animalScript.isHungry())
                        animalScript.state = States.LOOKING_FOR_FOOD;
                    else if (animalScript.wantsToReproduce())
                        animalScript.state = States.LOOKING_FOR_MATE;
                    break;

                case States.LOOKING_FOR_FOOD: // Primary need
                    if (animalScript.isHungry() == false)
                    {
                        if (animalScript.wantsToReproduce())
                            animalScript.state = States.LOOKING_FOR_MATE;
                        else animalScript.state = States.IDLE;
                    }
                    break;

                case States.LOOKING_FOR_MATE: // Secondary need
                    if (animalScript.isHungry())
                        animalScript.state = States.LOOKING_FOR_FOOD;
                    else if (animalScript.wantsToReproduce() == false)
                        animalScript.state = States.IDLE;
                    break;

                case States.FOLLOWING_MUM: // Default state for babies when there are no other needs

                    break;
            }
        }        

        yield return new WaitForSeconds(1.0f); // Wait before repeating the cycle
        StartCoroutine(ActionOfTheAnimalsStateMachine());
    }

    private IEnumerator ActionOfTheAnimalsStateMachine()
    {
        if (entityManager == null) Debug.LogError("entityManager was null on AnimalManager.cs on ActionOfTheAnimalsStateMachine()");
        List<GameObject> animals = null;
        if (entityManager.entities.Count > 0)
        {
            animals = entityManager.entities[(int)EntityManager.EntityType.ANIMAL];
        }
        if (animals == null)
        {
            Debug.LogError("animals list was null on AnimalManager.cs");
            yield return null;
        }

        for (int i = 0; i < animals.Count; i++)
        {
            Animal animalScript = animals[i].GetComponent<Animal>();
            if (animalScript == null) Debug.LogError("animalScript list was null on AnimalManager.cs");

            switch (animalScript.state)
            {
                case States.IDLE: // Default state when there are no other needs
                    StopMoving(animalScript);

                    break;

                case States.LOOKING_FOR_FOOD: // Primary need
                    MoveToFood(animalScript);

                    break;

                case States.LOOKING_FOR_MATE: // Secondary need
                    

                    break;

                case States.FOLLOWING_MUM: // Default state for babies when there are no other needs

                    break;
            }
        }

        yield return new WaitForSeconds(1.0f); // Wait before repeating the cycle
        StartCoroutine(UpdateAnimalsStateMachine());
    }

    private void MoveToFood(Animal animalScript)
    {
        Transform foodPos = FindClosestFood(animalScript);

        if (foodPos == null) return; // There are no food GOs in the scene

        NavMeshAgent myNavMeshAgent = animalScript.gameObject.GetComponent<NavMeshAgent>();
        if (myNavMeshAgent == null) Debug.LogError("myNavMeshAgent was null on AnimalManager.cs on MoveToFood()");
        myNavMeshAgent.SetDestination(foodPos.position);        
    }

    private Transform FindClosestFood(Animal animalScript)
    {
        // Create local list because it is not guaranteed that "foodManager.foodList" will not change while it is being iterated
        List<GameObject> foodList = entityManager.entities[(int)EntityManager.EntityType.FOOD];

        int indexOfClosestFood = -1;
        float smallestDistance = float.MaxValue;

        for (int i = 0; i < foodList.Count; i++)
        {
            // Food died before the animal could arrive
            if (foodList[i] == null) continue;

            float distance = Vector3.Distance(foodList[i].transform.position, animalScript.gameObject.transform.position);

            // If food is found at an eatable distance, then animal stops looking for other food
            if (animalScript.canEat(distance))
            {
                Entity entityScript = foodList[i].GetComponent<Entity>();
                entityManager.TryToKill(EntityManager.EntityType.FOOD, entityScript.GetUUID());
                animalScript.SetMaxHunger();
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

    private void StopMoving(Animal animalScript)
    {
        NavMeshAgent myNavMeshAgent = animalScript.gameObject.GetComponent<NavMeshAgent>();
        if (myNavMeshAgent == null) Debug.LogError("myNavMeshAgent was null on AnimalManager.cs on StopMoving()");

        myNavMeshAgent.SetDestination(animalScript.gameObject.transform.position);
    }
}

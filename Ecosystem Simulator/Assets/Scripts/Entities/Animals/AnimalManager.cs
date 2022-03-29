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

    public GameObject animalPrefab = null;
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
            Reproduction myGender = animals[i].GetComponent<Reproduction>();
            if (animalScript == null) Debug.LogError("animalScript list was null on UpdateStats.cs");

            animalScript.UpdateAllStats();
            if (myGender.gender == 0 && myGender.IsPregnant()) myGender.UpdatePregnancy(animalPrefab);
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
                    animalScript.StopMoving();

                    break;

                case States.LOOKING_FOR_FOOD: // Primary need
                    MoveToFood(animalScript);

                    break;

                case States.LOOKING_FOR_MATE: // Secondary need
                    MoveToClosestPotentialMate(animalScript);

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

        animalScript.MoveTo(foodPos);      
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
                animalScript.Eat();
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

    private void MoveToClosestPotentialMate(Animal animalScript)
    {
        Reproduction myGender = animalScript.gameObject.GetComponent<Reproduction>();

        // Create local list because it is not guaranteed that the list will not change while it is being iterated
        List<GameObject> potentialMates = entityManager.entities[(int)EntityManager.EntityType.ANIMAL];

        int indexOfClosestMate = -1;
        float smallestDistance = float.MaxValue;

        for (int i = 0; i < potentialMates.Count; i++)
        {
            // Mate died before the animal could arrive
            if (potentialMates[i] == null) continue;

            // Mates can only be of the opposite gender
            Reproduction mateGender = potentialMates[i].GetComponent<Reproduction>();
            if (myGender.gender == mateGender.gender) continue;

            // A mate must also be in "LOOKING_FOR_MATE" state
            Animal otherAnimalScript = potentialMates[i].GetComponent<Animal>();
            if (otherAnimalScript.state != States.LOOKING_FOR_MATE) continue; // Since babies can't be on this state, this also ensures that babies are not mates

            float distance = Vector3.Distance(potentialMates[i].transform.position, animalScript.gameObject.transform.position);

            if (distance < 1f) // Mate found, let's reproduce!
            {
                if (myGender.gender == 0) 
                {
                    if(myGender.RequestMate(mateGender) == true) // Mate accepted
                    {
                        myGender.GetPregnant();
                        animalScript.SetMaxReproductionUrge();
                    }
                }
                    
                break;
            }else if (myGender.gender == 0 && distance < 10f) // Females wait at 10f distance for the male. Removing this causes a weird reproduction pattern
            {
                animalScript.StopMoving();
            }

            if (distance < smallestDistance)
            {
                indexOfClosestMate = i;
                smallestDistance = distance;
            }
        }

        // Move to closest potential mate
        if (indexOfClosestMate != -1)
        {
            animalScript.MoveTo(potentialMates[indexOfClosestMate].transform);
        }
    }
}
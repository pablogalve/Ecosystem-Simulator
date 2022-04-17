using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalManager : MonoBehaviour
{
    public enum Species
    {
        UNDEFINED,
        R,
        K,
    }

    public enum States { 
        IDLE,
        LOOKING_FOR_FOOD,
        LOOKING_FOR_MATE,
        FOLLOWING_MUM,
    }

    public GameObject animalPrefab = null;
    public List<GameObject> animalPrefabs = new List<GameObject>();

    private EntityManager entityManager = null;

    // Start is called before the first frame update
    void Start()
    {
        entityManager = GetComponent<EntityManager>();

        StartCoroutine(UpdateAnimalsStateMachine());
        StartCoroutine(UpdateStats());
    }

    private IEnumerator UpdateStats()
    {
        if (entityManager == null) throw new System.Exception("entityManager was null on AnimalManager.cs on UpdateStats()");

        for (int i = 0; i < entityManager.UUIDs.Count; i++) // O(n)
        {
            if (entityManager.UUIDs[i].type != EntityManager.EntityType.ANIMAL) continue;
            GameObject animal = entityManager.entities[entityManager.UUIDs[i].UUID];

            Animal animalScript = animal.GetComponent<Animal>();
            Reproduction myGender = animal.GetComponent<Reproduction>();
            if (animalScript == null) Debug.LogError("animalScript list was null on UpdateStats.cs");

            animalScript.UpdateAllStats(entityManager.UUIDs[i].UUID);
            if (myGender.gender == 0 && myGender.IsPregnant()) myGender.UpdatePregnancy(animalPrefab);
        }
        
        yield return new WaitForSeconds(15.0f); // Wait before repeating the cycle
        StartCoroutine(UpdateStats());
    }

    private IEnumerator UpdateAnimalsStateMachine()
    {
        if (entityManager == null) throw new System.Exception("entityManager was null on AnimalManager.cs on UpdateAnimalsStateMachine()");

        for (int i = 0; i < entityManager.UUIDs.Count; i++)
        {
            if (entityManager.UUIDs[i].type != EntityManager.EntityType.ANIMAL) continue;
            GameObject animal = entityManager.entities[entityManager.UUIDs[i].UUID];

            Animal animalScript = animal.GetComponent<Animal>();
            if (animalScript == null) throw new System.Exception("animalScript list was null on AnimalManager.cs");

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
        if (entityManager == null) throw new System.Exception("entityManager was null on AnimalManager.cs on ActionOfTheAnimalsStateMachine()");

        for (int i = 0; i < entityManager.UUIDs.Count; i++)
        {
            if (entityManager.UUIDs[i].type != EntityManager.EntityType.ANIMAL) continue;
            GameObject animal = entityManager.entities[entityManager.UUIDs[i].UUID];

            Animal animalScript = animal.GetComponent<Animal>();
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
        int indexOfClosestFood = -1;
        float smallestDistance = float.MaxValue;

        for (int i = 0; i < entityManager.UUIDs.Count; i++)
        {
            if (entityManager.UUIDs[i].type != EntityManager.EntityType.FOOD) continue;
            GameObject food = entityManager.entities[entityManager.UUIDs[i].UUID];

            // Food died before the animal could arrive
            if (food == null) continue;

            float distance = Vector3.Distance(food.transform.position, animalScript.gameObject.transform.position);

            // If food is found at an eatable distance, then animal stops looking for other food
            if (animalScript.canEat(distance))
            {
                entityManager.TryToKill(EntityManager.EntityType.FOOD, entityManager.UUIDs[i].UUID);
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
        else return entityManager.entities[entityManager.UUIDs[indexOfClosestFood].UUID].transform;
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

        int indexOfClosestMate = -1;
        float smallestDistance = float.MaxValue;

        if (entityManager == null) throw new System.Exception("entityManager was null on AnimalManager.cs on MoveToClosestPotentialMate()");

        for (int i = 0; i < entityManager.UUIDs.Count; i++)
        {
            if (entityManager.UUIDs[i].type != EntityManager.EntityType.ANIMAL) continue;
            GameObject potentialMate = entityManager.entities[entityManager.UUIDs[i].UUID];

            // Mate died before the animal could arrive
            if (potentialMate == null) continue;

            // A mate must also be in "LOOKING_FOR_MATE" state
            Animal otherAnimalScript = potentialMate.GetComponent<Animal>();
            if (otherAnimalScript.state != States.LOOKING_FOR_MATE) continue; // Since babies can't be on this state, this also ensures that babies are not mates
            if (animalScript.species != otherAnimalScript.species) continue;

            // Mates can only be of the opposite gender
            Reproduction mateGender = potentialMate.GetComponent<Reproduction>();
            if (myGender.gender == mateGender.gender) continue;

            float distance = Vector3.Distance(potentialMate.transform.position, animalScript.gameObject.transform.position);

            if (distance < 1f) // Mate found, let's reproduce!
            {
                if (myGender.gender == 0)
                {
                    if (myGender.RequestMate(mateGender) == true) // Mate accepted
                    {
                        myGender.GetPregnant();
                        animalScript.SetMaxReproductionUrge();
                    }
                }

                break;
            }
            else if (myGender.gender == 0 && distance < 10f) // Females wait at 10f distance for the male. Removing this causes a weird reproduction pattern
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
            animalScript.MoveTo(entityManager.entities[entityManager.UUIDs[indexOfClosestMate].UUID].transform);
        }
    }
}
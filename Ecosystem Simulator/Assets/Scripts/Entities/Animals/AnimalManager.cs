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
        CARNIVORE
    }

    public enum States { 
        IDLE,
        LOOKING_FOR_FOOD,
        LOOKING_FOR_MATE,
        FOLLOWING_MUM,
    }

    public int maxAnimalsToUpdatePerFrame = 50;

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

        for (LinkedListNode<EntityManager.Entity> entity = entityManager.UUIDs.First; entity != null; entity = entity.Next)
        {
            if (entity.Value.type != EntityManager.EntityType.ANIMAL) continue;
            GameObject animal = entityManager.entities[entity.Value.UUID];

            Animal animalScript = animal.GetComponent<Animal>();
            Reproduction myGender = animal.GetComponent<Reproduction>();
            if (animalScript == null) Debug.LogError("animalScript list was null on UpdateStats.cs");

            animalScript.UpdateAllStats(entity);
            if (myGender.gender == 0 && myGender.IsPregnant()) myGender.UpdatePregnancy();
        }
        
        yield return new WaitForSeconds(15.0f); // Wait before repeating the cycle
        StartCoroutine(UpdateStats());
    }

    private IEnumerator UpdateAnimalsStateMachine()
    {
        if (entityManager == null) throw new System.Exception("entityManager was null on AnimalManager.cs on UpdateAnimalsStateMachine()");

        for (int i = 0; i < entityManager.entitiesByType[(int)EntityManager.EntityType.ANIMAL].Count; i++)
        {
            GameObject animal = entityManager.entities[entityManager.entitiesByType[(int)EntityManager.EntityType.ANIMAL][i]];

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

            // Divide the workload in multiple frames
            if (i != 0 && i % maxAnimalsToUpdatePerFrame == 0)
                yield return null;
        }

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(ActionOfTheAnimalsStateMachine());
    }

    private IEnumerator ActionOfTheAnimalsStateMachine()
    {
        if (entityManager == null) throw new System.Exception("entityManager was null on AnimalManager.cs on ActionOfTheAnimalsStateMachine()");

        for (LinkedListNode<EntityManager.Entity> entity = entityManager.UUIDs.First; entity != null; entity = entity.Next)
        {
            if (entity.Value.type != EntityManager.EntityType.ANIMAL) continue;
            GameObject animal = entityManager.entities[entity.Value.UUID];

            Animal animalScript = animal.GetComponent<Animal>();
            if (animalScript == null) Debug.LogError("animalScript list was null on AnimalManager.cs");

            switch (animalScript.state)
            {
                case States.IDLE: // Default state when there are no other needs
                    animalScript.StopMoving();

                    break;

                case States.LOOKING_FOR_FOOD: // Primary need
                    MoveToFood(animalScript, entity.Value.UUID);

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

    private void MoveToFood(Animal animalScript, string myUUID)
    {
        // Food is different for herbivores and carnivores
        if (animalScript.isHerbivore)
        {
            var ret = FindClosestFood(animalScript);
            if(ret.Item1 == true) // If there's food nearby, move there
            {
                animalScript.MoveTo(ret.Item2);
            }
            else
            {
                // If animal is not already moving, set new direction using Levy walk
                NavMeshAgent myNavMeshAgent = animalScript.gameObject.GetComponent<NavMeshAgent>();
                if (myNavMeshAgent.hasPath) return;

                // Move to random position looking for food
                //Vector3 newTarget = animalScript.gameObject.transform.position;
                Vector3 newTarget = HeighmapData.Instance.LevyWalk(animalScript.gameObject.transform.position, 10f, 1000f, 2.0f);
                               
                animalScript.MoveTo(newTarget);
            }
        }
        //else foodPos = FindClosestAnimalToEat(animalScript, myUUID); // TODO: Add functionality for carnivores
    }

    private (bool, Vector3) FindClosestFood(Animal animalScript)
    {
        Vector3 closestFood = new Vector3(0f, 0f, 0f);
        float smallestDistance = float.MaxValue;
        bool foundAtLeastOne = false;

        Collider[] hitColliders = Physics.OverlapSphere(animalScript.transform.position, animalScript.fieldOfView);
        for(int i = 0; i < hitColliders.Length; i++)
        {
            GameObject food = hitColliders[i].gameObject;
            if (!food.CompareTag("Food")) continue;

            foundAtLeastOne = true;

            // Food died before the animal could arrive
            if (food == null) continue;

            Vector3 directionToTarget = food.transform.position - animalScript.gameObject.transform.position;
            float distanceSqr = directionToTarget.sqrMagnitude;

            // If food is found at an eatable distance, then animal stops looking for other food
            if (animalScript.canEat(distanceSqr))
            {
                AgeController foodAge = food.GetComponent<AgeController>(); // TODO: Ugly way to kill the food in a retarded way, since I can't access the UUID from here
                foodAge.age = foodAge.maxAge;
                animalScript.Eat();
                break;
            }

            if (distanceSqr < smallestDistance)
            {
                closestFood = food.transform.position;
                smallestDistance = distanceSqr;
            }
        }

        if (foundAtLeastOne) return (true, closestFood);
        else                 return (false, closestFood);
    }

    private Transform FindClosestAnimalToEat(Animal animalScript, string myUUID)
    {
        string UUIDOfClosestVictim = "";
        float smallestDistance = float.MaxValue;

        for (LinkedListNode<EntityManager.Entity> entity = entityManager.UUIDs.First; entity != null; entity = entity.Next)
        {
            if (entity.Value.type != EntityManager.EntityType.ANIMAL) continue;
            if (myUUID == entity.Value.UUID) continue;
            if (entityManager.entities[entity.Value.UUID].GetComponent<Animal>().isHerbivore == false) continue; // Can't eat other carnivores

            GameObject victim = entityManager.entities[entity.Value.UUID];

            // Food died before the animal could arrive
            if (victim == null) continue;

            Vector3 directionToTarget = victim.transform.position - animalScript.gameObject.transform.position;
            float distanceSqr = directionToTarget.sqrMagnitude;

            // If food is found at an eatable distance, then animal stops looking for other food
            if (animalScript.canEat(distanceSqr))
            {
                entityManager.TryToKill(entity);
                animalScript.Eat();
                break;
            }

            if (distanceSqr < smallestDistance)
            {
                UUIDOfClosestVictim = entity.Value.UUID;
                smallestDistance = distanceSqr;
            }
        }

        if (UUIDOfClosestVictim.Length == 0) return null;
        else return entityManager.entities[UUIDOfClosestVictim].transform;
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

        string UUIDOfClosestMate = "";
        float smallestDistance = float.MaxValue;

        if (entityManager == null) throw new System.Exception("entityManager was null on AnimalManager.cs on MoveToClosestPotentialMate()");

        for (LinkedListNode<EntityManager.Entity> entity = entityManager.UUIDs.First; entity != null; entity = entity.Next)
        {
            if (entity.Value.type != EntityManager.EntityType.ANIMAL) continue;
            GameObject potentialMate = entityManager.entities[entity.Value.UUID];

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
                UUIDOfClosestMate = entity.Value.UUID;
                smallestDistance = distance;
            }
        }

        // Move to closest potential mate
        if (UUIDOfClosestMate.Length != 0)
        {
            animalScript.MoveTo(entityManager.entities[UUIDOfClosestMate].transform.position);
        }
    }
}
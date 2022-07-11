using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalManager : MonoBehaviour
{
    public enum Species
    {
        UNDEFINED,
        LONGHORN,
        SHEEP,
        WOLF,
    }

    public enum States 
    { 
        IDLE,
        LOOKING_FOR_FOOD,
        LOOKING_FOR_MATE,
        EATING,
        DYING,        
    }

    public enum Animation
    {
        WALK,        
        EAT,
        IDLE,
        RUN
    }

    public int maxStatesUpdatePerFrame = 50;
    public int maxActionsUpdatePerFrame = 50;

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

        int i = 0;

        for (LinkedListNode<EntityManager.Entity> entity = entityManager.UUIDs.First; entity != null; entity = entity.Next)
        {
            i++;

            if (entity.Value.type != EntityManager.EntityType.ANIMAL) continue;
            GameObject animal = entityManager.entities[entity.Value.UUID];

            Animal animalScript = animal.GetComponent<Animal>();
            Reproduction myGender = animal.GetComponent<Reproduction>();
            if (animalScript == null) throw new System.Exception("animalScript list was null on UpdateStats.cs");

            animalScript.UpdateAllStats(entity);
            if (myGender.gender == 0 && myGender.IsPregnant()) myGender.UpdatePregnancy();

            if (i % maxStatesUpdatePerFrame == 0) yield return null;
        }
        
        yield return new WaitForSeconds(5.0f); // Wait before repeating the cycle
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
            
            switch (animalScript.GetState())
            {
                case States.IDLE: // Default state when there are no other needs
                    if (animalScript.isHungry())
                    {
                        animalScript.SetState(States.LOOKING_FOR_FOOD);
                    }
                    else if (animalScript.wantsToReproduce())
                    {
                        animalScript.SetState(States.LOOKING_FOR_MATE);
                    }
                    break;                

                case States.LOOKING_FOR_FOOD: // Primary need
                    if (animalScript.isHungry() == false)
                    {
                        if (animalScript.wantsToReproduce())
                        {
                            animalScript.SetState(States.LOOKING_FOR_MATE);
                        }                            
                        else
                        {
                            animalScript.SetState(States.IDLE);
                        }                                                    
                    }
                    break;

                case States.LOOKING_FOR_MATE: // Secondary need
                    if (animalScript.isHungry())
                    {
                        animalScript.SetState(States.LOOKING_FOR_FOOD);
                    }                        
                    else if (animalScript.wantsToReproduce() == false)
                    {
                        animalScript.SetState(States.IDLE);
                    }
                        
                    break;

                case States.EATING: 

                    break;

                case States.DYING:

                    break;
            }

            // Divide the workload in multiple frames
            if (i != 0 && i % maxStatesUpdatePerFrame == 0)
                yield return null;
        }

        //yield return new WaitForSeconds(1.0f);
        StartCoroutine(ActionOfTheAnimalsStateMachine());
    }

    private IEnumerator ActionOfTheAnimalsStateMachine()
    {
        if (entityManager == null) throw new System.Exception("entityManager was null on AnimalManager.cs on ActionOfTheAnimalsStateMachine()");

        int i = 0;

        for (LinkedListNode<EntityManager.Entity> entity = entityManager.UUIDs.First; entity != null; entity = entity.Next)
        {
            i++;

            if (entity.Value.type != EntityManager.EntityType.ANIMAL) continue;
            GameObject animal = entityManager.entities[entity.Value.UUID];

            Animal animalScript = animal.GetComponent<Animal>();
            if (animalScript == null) throw new System.Exception("animalScript list was null on AnimalManager.cs");

            switch (animalScript.GetState())
            {
                case States.IDLE: // Default state when there are no other needs
                    animalScript.StopMoving();
                    animalScript.PlayAnimation(GetAnimationIndex(animalScript.species, animalScript.CalculateCurrentAnimation()));
                    break;

                case States.LOOKING_FOR_FOOD: // Primary need
                    if(MoveToFood(animalScript, entity.Value.UUID))
                    {
                        animalScript.PlayAnimation(GetAnimationIndex(animalScript.species, animalScript.CalculateCurrentAnimation()));
                    }

                    break;

                case States.LOOKING_FOR_MATE: // Secondary need
                    if(MoveToClosestPotentialMate(animalScript))
                    {
                        animalScript.PlayAnimation(GetAnimationIndex(animalScript.species, animalScript.CalculateCurrentAnimation()));
                    }

                    break;

                case States.EATING:
                    animalScript.PlayAnimation(GetAnimationIndex(animalScript.species, animalScript.CalculateCurrentAnimation()));
                    break;

                case States.DYING:

                    break;
            }            

            // Divide the workload in multiple frames
            if (i % maxActionsUpdatePerFrame == 0)
                yield return null;
        }        

        //yield return new WaitForSeconds(1.0f); // Wait before repeating the cycle
        StartCoroutine(UpdateAnimalsStateMachine());
    }

    private bool MoveToFood(Animal animalScript, string myUUID)
    {
        // Food is different for herbivores and carnivores

        // Step 1: Get coordinates of closest food, or false if there's no food in field of view
        (bool, Vector3) ret; // bool = food found? | Vector3 = coordinates of food
        if (animalScript.isHerbivore)
        {
            ret = FindClosestFood(animalScript);
        }
        else
        {
            ret = FindClosestHerbivoreToEat(animalScript, myUUID);
        }
        
        // Step 2: If food was found, move there. Otherwise, find in random location using LevyWalk
        if(ret.Item1 == true) // If there's food nearby, move there
        {
            return animalScript.MoveTo(ret.Item2);
        }
        else
        {
            // If animal is not already moving, set new direction using Levy walk
            NavMeshAgent myNavMeshAgent = animalScript.gameObject.GetComponent<NavMeshAgent>();
            if (myNavMeshAgent.hasPath) return true;

            // Move to random position looking for food
            Vector3 newTarget = HeightmapData.Instance.LevyWalk(animalScript.gameObject.transform.position, 10f, 1000f, 2.0f);
                               
            return animalScript.MoveTo(newTarget);
        }        
    }

    private (bool, Vector3) FindClosestFood(Animal animalScript)
    {
        // Herbivores can eat both food and baby trees
        Vector3 closestFood = new Vector3(0f, 0f, 0f);
        float smallestDistance = float.MaxValue;
        bool foundAtLeastOne = false;

        Collider[] hitColliders = Physics.OverlapSphere(animalScript.transform.position, animalScript.fieldOfView);
        for(int i = 0; i < hitColliders.Length; i++)
        {
            GameObject food = hitColliders[i].gameObject;            

            if(food.CompareTag("Tree"))
            {
                AgeController treeAge = food.GetComponent<AgeController>();
                if (!treeAge.IsBaby())
                    continue;
            }else if (!food.CompareTag("Food")) continue;

            foundAtLeastOne = true;

            // Food died before the animal could arrive
            if (food == null) continue;

            Vector3 directionToTarget = food.transform.position - animalScript.gameObject.transform.position;
            float distanceSqr = directionToTarget.sqrMagnitude;

            // If food is found at an eatable distance, then animal stops looking for other food
            if (animalScript.canEat(distanceSqr))
            {
                animalScript.SetState(States.EATING);

                animalScript.Eat();
                animalScript.PlayAnimation(GetAnimationIndex(animalScript.species, animalScript.CalculateCurrentAnimation()));

                entityManager.TryToKill(food.GetComponent<UUID>().GetMyUUIDInfo());

                return (false, closestFood);
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

    private (bool, Vector3) FindClosestHerbivoreToEat(Animal animalScript, string myUUID)
    {
        Vector3 closestHerbivore = new Vector3(0f, 0f, 0f);
        float smallestDistance = float.MaxValue;
        bool foundAtLeastOne = false;

        Collider[] hitColliders = Physics.OverlapSphere(animalScript.transform.position, animalScript.fieldOfView);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            GameObject herbivore = hitColliders[i].gameObject;
            if (!herbivore.CompareTag("Herbivore")) continue;
            
            // Food died before the animal could arrive
            if (herbivore == null) continue;

            foundAtLeastOne = true;

            Vector3 directionToTarget = herbivore.transform.position - animalScript.gameObject.transform.position;
            float distanceSqr = directionToTarget.sqrMagnitude;

            // If food is found at an eatable distance, then animal stops looking for other food
            if (animalScript.canEat(distanceSqr))
            {
                animalScript.SetState(States.EATING);
                animalScript.Eat();
                animalScript.PlayAnimation(GetAnimationIndex(animalScript.species, animalScript.CalculateCurrentAnimation()));

                entityManager.TryToKill(herbivore.GetComponent<UUID>().GetMyUUIDInfo());

                return (false, closestHerbivore);
            }

            if (distanceSqr < smallestDistance)
            {
                closestHerbivore = herbivore.transform.position;
                smallestDistance = distanceSqr;
            }
        }

        if (foundAtLeastOne) return (true, closestHerbivore);
        else return (false, closestHerbivore);
    }

    private bool MoveToClosestPotentialMate(Animal animalScript)
    {
        Vector3 closestMate = new Vector3(0f, 0f, 0f);
        float smallestDistance = float.MaxValue;
        bool foundAtLeastOne = false;
        Reproduction myGender = animalScript.gameObject.GetComponent<Reproduction>();

        Collider[] hitColliders = Physics.OverlapSphere(animalScript.transform.position, animalScript.fieldOfView);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            GameObject mate = hitColliders[i].gameObject;
            Animal otherAnimal = mate.GetComponent<Animal>();
            if (otherAnimal == null) continue; // Only animals can reproduce
            if (animalScript.species != otherAnimal.species) continue; // Must be of the same species to reproduce

            // A mate must also be in "LOOKING_FOR_MATE" state
            if (otherAnimal.GetState() != States.LOOKING_FOR_MATE) continue; // Since babies can't be on this state, this also ensures that babies are not mates
            if (animalScript.species != otherAnimal.species) continue;

            // Mates can only be of the opposite gender
            Reproduction mateGender = mate.GetComponent<Reproduction>();
            if (myGender.gender == mateGender.gender) continue;

            foundAtLeastOne = true;

            Vector3 directionToTarget = mate.transform.position - animalScript.gameObject.transform.position;
            float distanceSqr = directionToTarget.sqrMagnitude;

            if (myGender.gender == 0)
            {
                if (distanceSqr < 3f)
                {
                    if (myGender.RequestMate(mateGender) == true) // Mate accepted
                    {
                        myGender.GetPregnant();
                        animalScript.SetMaxReproductionUrge();
                        otherAnimal.SetMaxReproductionUrge();
                        break;
                    }
                    else continue; // Mate denied. Let's look for another one
                }
                else if (distanceSqr < 100f) // Females wait at 10f distance (distanceSqr is squared) for the male. Removing this causes a weird reproduction pattern
                {
                    animalScript.StopMoving();
                    break;
                }
            }

            if (distanceSqr < smallestDistance)
            {
                closestMate = mate.transform.position;
                smallestDistance = distanceSqr;
            }
        }

        // --- All potential mates have been checked, now let's act

        if (foundAtLeastOne && myGender.gender == 1)
        {
            return animalScript.MoveTo(closestMate);
        }
        else if (!foundAtLeastOne)
        {
            // If animal is not already moving, set new direction using Levy walk
            NavMeshAgent myNavMeshAgent = animalScript.gameObject.GetComponent<NavMeshAgent>();
            if (myNavMeshAgent.hasPath) return true;

            // Move to random position looking for mate
            Vector3 newTarget = HeightmapData.Instance.LevyWalk(animalScript.gameObject.transform.position, 10f, 1000f, 2.0f);

            return animalScript.MoveTo(newTarget);
        }
        else return false;
    }

    public int GetAnimationIndex(Species species, Animation animation)
    {
        switch(animation)
        {
            case Animation.IDLE:
                if (species == Species.SHEEP) return 2;
                if (species == Species.LONGHORN) return 0;
                if (species == Species.WOLF) return 0;
                break;

            case Animation.WALK:
                if (species == Species.SHEEP) return 0;
                if (species == Species.LONGHORN) return 1;
                if (species == Species.WOLF) return 2;
                break;

            case Animation.RUN:
                if (species == Species.SHEEP) return 3;
                if (species == Species.LONGHORN) return 3;
                if (species == Species.WOLF) return 1;
                break;

            case Animation.EAT:
                if (species == Species.SHEEP) return 1;
                if (species == Species.LONGHORN) return 2;
                if (species == Species.WOLF) return 0;
                break;

            default:
                throw new System.Exception("Animation not being handled on GetAnimationIndex()");
        }

        throw new System.Exception("Animal species not being handled on GetAnimationIndex()");
    }
}
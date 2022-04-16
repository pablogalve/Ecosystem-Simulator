using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    public AnimalManager.Species species = AnimalManager.Species.UNDEFINED;
    public AnimalManager.States state;

    // Basic needs for animals
    public byte maxNeed = 10; // Needs go from 
    public byte reproductionUrge; // Goes from 0 (no reproduction urge) to max
    public byte hunger; // Goes from 0 (death from starvation) to max
    private float minDistanceToEat = 1f;

    public float speed = 4.0f;
    public float speedForBabiesAndPregnants = 2.0f;

    // Start is called before the first frame update
    void Awake()
    {
        state = AnimalManager.States.IDLE;
        reproductionUrge = maxNeed;
        hunger = maxNeed;       
    }

    public bool isHungry()
    {
        return hunger <= maxNeed >> 1; // Return true is hunger <= maxNeed/2
    }

    public bool wantsToReproduce()
    {
        float reproductionUrgePercentage = (float)(reproductionUrge) / (float)(maxNeed);
        return reproductionUrgePercentage <= 0.4f;
    }

    public bool canEat(float distance)
    {
        return distance <= minDistanceToEat;
    }

    public void Eat()
    {
        // Eating restores a maximum of 50% of the animal's total hunger. Example: If current hunger is 30% and it eats, hunger becomes 80% (30+50)        
        hunger += (byte)(maxNeed >> 1); // Bit-shifting. ">> 1" is like dividing by 2
    }

    public void SetMaxReproductionUrge()
    {
        reproductionUrge = maxNeed;
    }

    public void UpdateAllStats()
    {
        if (hunger == 0) Die();
        else hunger--;

        AgeController ageController = gameObject.GetComponent<AgeController>();
        if (ageController.IsBaby() == true) return; // Babies don't have reproduction urge

        // ReproductionUrge can't go below 0. It will stay on 0 until the animal dies
        if (reproductionUrge != 0) reproductionUrge--;
    }

    private void Die()
    {
        EntityManager entityManager = GameObject.Find("GameManager").GetComponent<EntityManager>();
        string myUUID = entityManager.entities.FirstOrDefault(x => x.Value == this).Key;
        entityManager.TryToKill(EntityManager.EntityType.ANIMAL, myUUID);
    }

    public void MoveTo(Transform targetPosition)
    {
        NavMeshAgent myNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        AgeController ageController = gameObject.GetComponent <AgeController>();
        Reproduction reproduction = gameObject.GetComponent<Reproduction>();

        if (myNavMeshAgent == null) { Debug.LogError("myNavMeshAgent was null on Animal.cs on MoveTo()"); return; }
        if (ageController == null) { Debug.LogError("ageController was null on Animal.cs on MoveTo()"); return; }
        if (reproduction == null) { Debug.LogError("reproduction was null on Animal.cs on MoveTo()"); return; }

        myNavMeshAgent.SetDestination(targetPosition.position);

        // Reduce speed to babies and pregnant females
        if (ageController.IsBaby() || reproduction.IsPregnant()) myNavMeshAgent.speed = speedForBabiesAndPregnants;        
        else myNavMeshAgent.speed = speed;
    }

    public void StopMoving()
    {
        NavMeshAgent myNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        if (myNavMeshAgent == null) Debug.LogError("myNavMeshAgent was null on AnimalManager.cs on StopMoving()");

        myNavMeshAgent.SetDestination(gameObject.transform.position);
    }
}
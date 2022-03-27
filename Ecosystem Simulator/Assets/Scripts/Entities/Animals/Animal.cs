using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    public AnimalManager.States state;

    // Basic needs for animals
    private byte maxNeed = 10; // Needs go from 
    private byte reproductionUrge; // Goes from 0 (no reproduction urge) to max
    private byte hunger; // Goes from 0 (death from starvation) to max
    private float minDistanceToEat = 1f;    

    // Start is called before the first frame update
    void Start()
    {
        state = AnimalManager.States.IDLE;
        reproductionUrge = maxNeed;
        hunger = maxNeed;
       
    }

    public bool isHungry()
    {
        float hungerPercentage = (float)(hunger) / (float)(maxNeed);
        return hungerPercentage <= 0.4f;
    }

    public bool wantsToReproduce()
    {
        float reproductionUrgePercentage = (float)(reproductionUrge) / (float)(reproductionUrge);
        return reproductionUrgePercentage <= 0.4f;
    }

    public bool canEat(float distance)
    {
        return distance <= minDistanceToEat;
    }

    public void SetMaxHunger()
    {
        hunger = maxNeed;
    }

    public void UpdateAllStats()
    {
        if (hunger == 0) Die();
        else hunger--;

        // ReproductionUrge can't go below 0. It will stay on 0 until the animal dies
        if (reproductionUrge != 0) reproductionUrge--;
    }

    private void Die()
    {
        EntityManager entityManager = GameObject.Find("GameManager").GetComponent<EntityManager>();
        Entity entityScript = gameObject.GetComponent<Entity>();
        entityManager.TryToKill(EntityManager.EntityType.ANIMAL, entityScript.GetUUID());
    }
}
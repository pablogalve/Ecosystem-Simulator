using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    public AnimalManager.Species species = AnimalManager.Species.UNDEFINED;
    [SerializeField] private AnimalManager.States state;

    private Animator animator;

    // Basic needs for animals
    public byte maxNeed = 10; // Needs go from 
    public byte reproductionUrge; // Goes from 0 (no reproduction urge) to max
    public byte hunger; // Goes from 0 (death from starvation) to max
    public float minDistanceToEat = 1f;
    public bool isHerbivore = true;

    public float speed = 4.0f;
    public float speedForBabiesAndPregnants = 2.0f;

    public float fieldOfView = 50.0f;

    public float eatingTime = 10.0f;
    public float eatingTimer = 0.0f;

    public List<GameObject> UIimageToDisplayState = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();

        OnSpawn();
    }

    private void Update()
    {
        if(state == AnimalManager.States.EATING)
        {
            eatingTimer -= Time.deltaTime;

            if(eatingTimer <= 0.0f)
            {
                SetState(AnimalManager.States.IDLE);             
            }
        }
    }

    public void OnSpawn()
    {
        state = AnimalManager.States.IDLE;
        reproductionUrge = maxNeed;
        hunger = maxNeed;
        minDistanceToEat = Mathf.Pow(minDistanceToEat, 2);

        Reproduction reproduction = gameObject.GetComponent<Reproduction>();
        reproduction.OnSpawn();

        AgeController age = gameObject.GetComponent<AgeController>();
        age.OnSpawn();
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

    public bool canEat(float distanceSquared)
    {
        return distanceSquared <= minDistanceToEat;
    }

    public void Eat()
    {
        SetState(AnimalManager.States.EATING);
        eatingTimer = eatingTime;
        StopMoving();

        // Eating restores a maximum of 50% of the animal's total hunger. Example: If current hunger is 30% and it eats, hunger becomes 80% (30+50)        
        hunger += (byte)(maxNeed >> 1); // Bit-shifting. ">> 1" is like dividing by 2
        if(hunger > maxNeed) hunger = maxNeed;
    }

    public void SetMaxReproductionUrge()
    {
        reproductionUrge = maxNeed;
    }

    public void UpdateAllStats(LinkedListNode<EntityManager.Entity> entityNode)
    {
        if (hunger == 0) Die(entityNode);
        else hunger--;

        AgeController ageController = gameObject.GetComponent<AgeController>();
        if (ageController.IsBaby() == true) return; // Babies don't have reproduction urge

        // ReproductionUrge can't go below 0. It will stay on 0 until the animal dies
        if (reproductionUrge != 0) reproductionUrge--;
    }

    private void Die(LinkedListNode<EntityManager.Entity> entityNode)
    {
        EntityManager entityManager = GameObject.Find("GameManager").GetComponent<EntityManager>();
        entityManager.TryToKill(entityNode);
    }

    public void MoveTo(Vector3 targetPosition)
    {
        NavMeshAgent myNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        AgeController ageController = gameObject.GetComponent <AgeController>();
        Reproduction reproduction = gameObject.GetComponent<Reproduction>();

        if (myNavMeshAgent == null) { throw new System.Exception("myNavMeshAgent was null on Animal.cs on MoveTo()"); }
        if (ageController == null) { throw new System.Exception("ageController was null on Animal.cs on MoveTo()"); }
        if (reproduction == null) { throw new System.Exception("reproduction was null on Animal.cs on MoveTo()"); }

        myNavMeshAgent.SetDestination(targetPosition);

        // Reduce speed to babies and pregnant females
        if (ageController.IsBaby() || reproduction.IsPregnant()) myNavMeshAgent.speed = speedForBabiesAndPregnants;        
        else myNavMeshAgent.speed = speed;
    }

    public void StopMoving()
    {
        NavMeshAgent myNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        if (myNavMeshAgent == null) throw new System.Exception("myNavMeshAgent was null on AnimalManager.cs on StopMoving()");

        myNavMeshAgent.SetDestination(gameObject.transform.position);
    }

    public GameObject GetCurrentUIimage()
    {
        // The UI Image is the same for states EATING and LOOKING_FOR_FOOD, so it is stored only once to save memory
        if(state == AnimalManager.States.EATING) return UIimageToDisplayState[(int)AnimalManager.States.LOOKING_FOR_FOOD];

        if ((int)state >= UIimageToDisplayState.Count)
            throw new System.Exception("The UI image was not set in an animal for the state: " + state.ToString());

        return UIimageToDisplayState[(int)state];
    }

    public AnimalManager.States GetState()
    {
        return state;
    }

    public void SetState(AnimalManager.States newState)
    {
        GetCurrentUIimage().SetActive(false);
        state = newState;
        GetCurrentUIimage().SetActive(true);        

        HandleAnimatorController();
    }

    private void HandleAnimatorController()
    {
        if (animator == null) { throw new System.Exception("This animal does not have an animator controller, but all animals must have one."); }
        
        animator.SetInteger("state", (int)state);

        NavMeshAgent myNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        animator.SetFloat("speed", myNavMeshAgent.speed);        
    }
}
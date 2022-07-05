using AnimationInstancingNamespace;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    public AnimalManager.Species species = AnimalManager.Species.UNDEFINED;
    [SerializeField] private AnimalManager.States state;
    [SerializeField] private AnimalManager.Animation currAnimation;

    private AnimationInstancing animationInstancing;

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

    // Start is called before the first frame update
    void Awake()
    {
        animationInstancing = GetComponent<AnimationInstancing>();

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
        byte random = (byte)Random.Range(1, maxNeed);

        state = AnimalManager.States.IDLE;
        reproductionUrge = random;
        hunger = random;
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
        StopMoving();
        EntityManager entityManager = GameObject.Find("GameManager").GetComponent<EntityManager>();
        entityManager.TryToKill(entityNode);
    }

    public void MoveTo(Vector3 targetPosition)
    {
        if (state == AnimalManager.States.IDLE) return;
        if (state == AnimalManager.States.EATING) return;
        if (state == AnimalManager.States.DYING) return;

        NavMeshAgent myNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        AgeController ageController = gameObject.GetComponent <AgeController>();
        Reproduction reproduction = gameObject.GetComponent<Reproduction>();

        if (myNavMeshAgent == null) { throw new System.Exception("myNavMeshAgent was null on Animal.cs on MoveTo()"); }
        if (ageController == null) { throw new System.Exception("ageController was null on Animal.cs on MoveTo()"); }
        if (reproduction == null) { throw new System.Exception("reproduction was null on Animal.cs on MoveTo()"); }

        if (myNavMeshAgent.destination != targetPosition)
        {
            myNavMeshAgent.SetDestination(targetPosition);
        }

        // Reduce speed to babies and pregnant females
        if (ageController.IsBaby() || reproduction.IsPregnant()) myNavMeshAgent.speed = speedForBabiesAndPregnants;        
        else myNavMeshAgent.speed = speed;
    }

    public void StopMoving()
    {
        NavMeshAgent myNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        if (myNavMeshAgent == null) throw new System.Exception("myNavMeshAgent was null on AnimalManager.cs on StopMoving()");

        if(myNavMeshAgent.hasPath)
        {
            myNavMeshAgent.SetDestination(gameObject.transform.position);
            //myNavMeshAgent.isStopped = true;
        }        
    }

    public AnimalManager.States GetState()
    {
        return state;
    }

    public void SetState(AnimalManager.States newState)
    {
        state = newState;     

        HandleAnimatorController();
    }

    private void HandleAnimatorController()
    {
        switch (state)
        {
            case AnimalManager.States.IDLE:
                currAnimation = AnimalManager.Animation.IDLE;
                break;

            case AnimalManager.States.LOOKING_FOR_FOOD:
                NavMeshAgent myNavMeshAgent0 = gameObject.GetComponent<NavMeshAgent>();
                if(Mathf.Approximately(myNavMeshAgent0.speed, speedForBabiesAndPregnants))
                {
                    currAnimation = AnimalManager.Animation.WALK;
                }
                else
                {
                    currAnimation = AnimalManager.Animation.RUN;
                }                
                break;

            case AnimalManager.States.LOOKING_FOR_MATE:
                NavMeshAgent myNavMeshAgent1 = gameObject.GetComponent<NavMeshAgent>();
                if (Mathf.Approximately(myNavMeshAgent1.speed, speedForBabiesAndPregnants))
                {
                    currAnimation = AnimalManager.Animation.WALK;
                }
                else
                {
                    currAnimation = AnimalManager.Animation.RUN;
                }
                break;

            case AnimalManager.States.EATING:
                currAnimation = AnimalManager.Animation.EAT;
                break;

            default:
                throw new System.Exception("State not being handled on Animal.cs HandleAnimatorController()");
        }

        animationInstancing.PlayAnimation((int)currAnimation);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    float hunger = 50;
    NavMeshAgent myNavMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        hunger -= Time.deltaTime;

        if(hunger <= 0)
        {
            Destroy(gameObject);
        }
    }
}

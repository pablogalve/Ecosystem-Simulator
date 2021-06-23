using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravityAttractor : MonoBehaviour
{

    public Rigidbody rb;
    const float G = 667.4f;

    void FixedUpdate()
    {
        AttractAnimals();
        AttractFood();
    }

    void Attract(GameObject objToAttract)
    {
        Rigidbody rbToAttract = objToAttract.GetComponent<Rigidbody>();

        Vector3 direction = rb.position - rbToAttract.position;
        float distance = direction.sqrMagnitude;

        float forceMagnitude = (rb.mass * rbToAttract.mass) / distance;
        Vector3 force = direction.normalized * forceMagnitude;

        rbToAttract.AddForce(force);
    }

    void AttractFood()
    {
        GameObject[] food = GameObject.FindGameObjectsWithTag("Food");
        foreach (GameObject f in food)
        {
            if (f != this)
                Attract(f);
        }
    }

    void AttractAnimals()
    {
        GameObject[] animals = GameObject.FindGameObjectsWithTag("Animal");
        foreach (GameObject animal in animals)
        {
            if (animal != this)
                Attract(animal);
        }
    }
}

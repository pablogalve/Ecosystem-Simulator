using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravityAttractor : MonoBehaviour
{

    public Rigidbody rb;
    const float G = 667.4f;

    void FixedUpdate()
    {
        GameObject[] animals = GameObject.FindGameObjectsWithTag("Animal");
        foreach (GameObject animal in animals)
        {
            if (animal != this)
                Attract(animal);
        }
    }

    void Attract(GameObject objToAttract)
    {
        Rigidbody rbToAttract = objToAttract.GetComponent<Rigidbody>();

        Vector3 direction = rb.position - rbToAttract.position;
        float distance = direction.magnitude;

        float forceMagnitude = (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * forceMagnitude;

        rbToAttract.AddForce(force);
    }
}

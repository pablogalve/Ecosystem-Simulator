using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class herviboreAI : MonoBehaviour
{
    public float speed = 3.0f;
    public float hunger = 100.0f;

    void Update()
    {
        Move();
        UpdateStats();
    }

    void Move()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void UpdateStats()
    {
        hunger -= Time.deltaTime;

        if (hunger <= 0.0f)
            Destroy(this.gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Food")
        {
            hunger += DataHolder.hungerIncrementAtEating;
            DataHolder.food.Remove(other.gameObject);
            Destroy(other.gameObject);
        }
    }

    //Vision
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Food")
        {
            transform.LookAt(other.gameObject.transform);
        }

        var multiTag = other.gameObject.GetComponent<CustomTag>();
        //Escape from carnivores
        if (multiTag != null && multiTag.HasTag("Carnivore"))
        {
            transform.LookAt(transform.position - (other.gameObject.transform.position - transform.position));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Food")
        {
            transform.LookAt(other.gameObject.transform);
        }
    }
}

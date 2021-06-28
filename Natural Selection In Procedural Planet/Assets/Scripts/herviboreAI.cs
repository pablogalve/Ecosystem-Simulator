using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class herviboreAI : animalAI
{
    protected override void Update()
    {
        base.Update();
        UpdateStats();
    }

    void UpdateStats()
    {

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

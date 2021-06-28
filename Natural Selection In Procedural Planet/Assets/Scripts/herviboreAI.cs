using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class herviboreAI : animalAI
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void UpdateStats()
    {
        Debug.Log("Food in view: " + visibleFood.Count);
        //Debug.Log("Hervibores in view: " + visibleHervibores.Count);
        //Debug.Log("Carnivores in view: " + visibleCarnivores.Count);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Food")
        {
            visibleFood.Remove(other.gameObject);
            hunger += DataHolder.hungerIncrementAtEating;
            DataHolder.food.Remove(other.gameObject);
            Destroy(other.gameObject);
        }
    }

    //Vision
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

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

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (other.gameObject.tag == "Food")
        {
            transform.LookAt(other.gameObject.transform);
        }
    }
}

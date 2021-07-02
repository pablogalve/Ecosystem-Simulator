using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class herbivoreAI : animalAI
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void UpdateStats()
    {
        base.UpdateStats();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Food")
        {
            hunger += DataHolder.hungerIncrementAtEating;

            EntityManager.KillEntity(other.gameObject);
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

        if (other.gameObject.tag == "Food" && visibleFood.Count == 0)
        {
            transform.LookAt(other.gameObject.transform);
        }
    }
}

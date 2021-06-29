using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class carnivoreAI : animalAI
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
        var multiTag = other.gameObject.GetComponent<CustomTag>();
        //Kill hervibores
        if (multiTag != null && multiTag.HasTag("Hervibore"))
        {
            hunger += DataHolder.hungerIncrementAtEating;
            EntityManager.KillEntity(other.gameObject);
        }
    }

    //Vision
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        var multiTag = other.gameObject.GetComponent<CustomTag>();

        if (multiTag != null && multiTag.HasTag("Hervibore"))
        {
            transform.LookAt(other.gameObject.transform.position);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }
}

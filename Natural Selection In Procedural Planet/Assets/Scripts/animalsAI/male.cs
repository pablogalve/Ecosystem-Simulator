using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class male : MonoBehaviour
{
    DataHolder.Species specie = DataHolder.Species.UNDEFINED;
    void Start()
    {
        specie = GetSpecies();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GiveBirth()
    {

    }

    DataHolder.Species GetSpecies()
    {
        var multiTag = gameObject.GetComponent<CustomTag>();

        if (multiTag == null)
            return DataHolder.Species.UNDEFINED;

        if (multiTag.HasTag("Herbivore"))
            return DataHolder.Species.HERBIVORE;
        else if (multiTag.HasTag("Omnivore"))
            return DataHolder.Species.OMNIVORE;
        else if (multiTag.HasTag("Carnivore"))
            return DataHolder.Species.CARNIVORE;

        return DataHolder.Species.UNDEFINED;
    }

    void OnCollisionEnter(Collision female)
    {
        var multiTag = female.gameObject.GetComponent<CustomTag>();

        if (multiTag == null)
            return;

        if (female.gameObject.GetComponent<female>() == null)
            return; //It's not a female

        if (female.gameObject.GetComponent<female>().specie == specie)
        {
            Debug.Log("INSIDE 1");
            if (multiTag.HasTag("Animal")) //No need 
            {

            }
        }


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class female : MonoBehaviour
{
    public DataHolder.Species specie = DataHolder.Species.UNDEFINED;
    public bool isPregnant = false;
    public float incubationTime = 5.0f;
    private float incubationTimer = 0.0f;

    void Start()
    {
        specie = GetSpecies();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPregnant)
        {
            incubationTimer -= Time.deltaTime;
            if (incubationTimer <= 0.0f)
                GiveBirth();
        }
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

    DataHolder.Species ExternalGetSpecies()
    {
        return specie;
    }

    public bool MateRequestReceived(GameObject male)
    {
        if (true) //Accept request
            return true;
        else    //Reject request
            return false;
    }
}

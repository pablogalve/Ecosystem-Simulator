using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class female : MonoBehaviour
{
    public DataHolder.Species specie = DataHolder.Species.UNDEFINED;
    public bool isPregnant = false;
    public float incubationTime = 5.0f;
    public float incubationTimer = 0.0f;
    public GameObject myBabyPrefab = null;

    void Start()
    {
        isPregnant = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPregnant)
        {
            incubationTimer -= Time.deltaTime;
            if (incubationTimer <= 0.0f)
            {
                GiveBirth();
            }

        }
    }

    private void GiveBirth()
    {
        incubationTimer = incubationTime;
        Vector3 babyPos = new Vector3(0, 0, 0);
        GameObject newBaby = Instantiate(myBabyPrefab, this.transform.position + babyPos, Quaternion.identity);

        switch (specie)
        {
            case DataHolder.Species.HERBIVORE:
                EntityManager.CreateEntity("herbivore", newBaby);
                break;
            case DataHolder.Species.OMNIVORE:
                //TODO
                break;
            case DataHolder.Species.CARNIVORE:
                EntityManager.CreateEntity("carnivore", newBaby);
                break;
            case DataHolder.Species.UNDEFINED:
                //TODO
                break;
        }


        isPregnant = false;
    }
    private void GetPregnant()
    {
        isPregnant = true;
        incubationTimer = incubationTime;
    }

    public bool MateRequestReceived(GameObject male)
    {
        if (!isPregnant) //Accept request
        {
            GetPregnant();
            return true;
        }
        else    //Reject request
        {
            return false;
        }
    }
}

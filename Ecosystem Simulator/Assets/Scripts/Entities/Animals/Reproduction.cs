using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reproduction : MonoBehaviour
{
    public byte gender; // 0 = Female, 1 = Male
    [SerializeField] private bool isPregnant;
    public byte gestationPeriod = 2;
    [SerializeField] private byte currPregnancyTime;

    public void OnSpawn()
    {
        isPregnant = false;
        currPregnancyTime = 0;
        gender = (byte)Random.Range(0, 2);

        // Set colors for debugging - Predefined color for males and light version for females //TODO: Remove the colors once fbx are in place
        var renderer = gameObject.GetComponent<Renderer>();
        Color newColor = renderer.material.color;
        newColor.g += 0.5f;
        if (gender == 0) renderer.material.SetColor("_Color", newColor);
    }

    public bool RequestMate(Reproduction other)
    {
        // This function is called by a female with the male as a function argument
        // The female(this) has to accept (return true) or reject (return false)
        if (other.gender == 0) { throw new System.Exception("The argument must be a male"); }
        if (other.gender == this.gender) { throw new System.Exception("Animals must be of different gender to mate"); }
                
        if (IsPregnant() == false) return true;

        return false;
    }

    public void GetPregnant()
    {
        if (gender == 0 && isPregnant == false) isPregnant = true;
        else if (gender == 1) throw new System.Exception("You used the function GetPregnant() with a male. That's illegal");
        else if (isPregnant == true) Debug.LogWarning("This female was already pregnant");
        else throw new System.Exception("Code should never execute this line. If you read this, come to Gender.cs on GetPregnant() to see what happened");
    }

    public bool IsPregnant()
    {
        return isPregnant;
    }

    public void UpdatePregnancy()
    {
        currPregnancyTime++;
        if(currPregnancyTime >= gestationPeriod)
        {
            EntityManager entityManager = GameObject.Find("GameManager").GetComponent<EntityManager>();
            if(entityManager.isMaxCapReached(EntityManager.EntityType.ANIMAL) == false)
            {
                Animal animalScript = gameObject.GetComponent<Animal>();

                EntityFactory entityFactory = GameObject.Find("GameManager").GetComponent<EntityFactory>();
                if (animalScript.species == AnimalManager.Species.R) //TODO:Refactor this to make it cleaner
                {
                    entityFactory.SpawnAnimalOfRandomGender((int)animalScript.species, gameObject.transform.position.x, gameObject.transform.position.z, 2f);
                    entityFactory.SpawnAnimalOfRandomGender((int)animalScript.species, gameObject.transform.position.x, gameObject.transform.position.z, 2f);
                    entityFactory.SpawnAnimalOfRandomGender((int)animalScript.species, gameObject.transform.position.x, gameObject.transform.position.z, 2f);
                    entityFactory.SpawnAnimalOfRandomGender((int)animalScript.species, gameObject.transform.position.x, gameObject.transform.position.z, 2f);
                    entityFactory.SpawnAnimalOfRandomGender((int)animalScript.species, gameObject.transform.position.x, gameObject.transform.position.z, 2f);
                }
                else 
                { 
                    entityFactory.SpawnAnimalOfRandomGender((int)animalScript.species, gameObject.transform.position.x, gameObject.transform.position.z, 2f); 
                    entityFactory.SpawnAnimalOfRandomGender((int)animalScript.species, gameObject.transform.position.x, gameObject.transform.position.z, 2f);
                }
            }

            currPregnancyTime = 0;
            isPregnant = false;
        }
    }
}

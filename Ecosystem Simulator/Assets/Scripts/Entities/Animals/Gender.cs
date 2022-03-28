using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gender : MonoBehaviour
{
    public byte gender = 0; // 0 = Female, 1 = Male
    public bool isPregnant = false;
    public byte gestationPeriod = 2;
    public byte currPregnancyTime = 0;

    // Start is called before the first frame update
    void Awake()
    {
        int gen = Random.Range(0, 2);
        gender = (byte)gen;

        var renderer = gameObject.GetComponent<Renderer>();
        if (gender == 0) renderer.material.SetColor("_Color", Color.red);
        if (gender == 1) renderer.material.SetColor("_Color", Color.blue);
    }

    public void GetPregnant()
    {
        if (gender == 0 && isPregnant == false) isPregnant = true;
        else if (gender == 1) Debug.LogWarning("You used the function GetPregnant() with a male. That's illegal");
        else if (isPregnant == true) Debug.LogWarning("This female was already pregnant");
        else Debug.LogWarning("Code should never execute this line. If you read this, come to Gender.cs on GetPregnant() to see what happened");
    }

    public bool IsPregnant()
    {
        return isPregnant;
    }

    public void UpdatePregnancy(GameObject babyPrefab)
    {
        currPregnancyTime++;
        if(currPregnancyTime >= gestationPeriod)
        {
            EntityManager entityManager = GameObject.Find("GameManager").GetComponent<EntityManager>();
            if(entityManager.isMaxCapReached(EntityManager.EntityType.ANIMAL) == false)
                entityManager.TryToSpawn(EntityManager.EntityType.ANIMAL, babyPrefab, gameObject.transform.position.x, gameObject.transform.position.z, 1f);

            currPregnancyTime = 0;
            isPregnant = false;
        }
    }
}

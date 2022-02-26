using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public List<GameObject> foodList;
    private int spawnedFood = 0;

    public byte maxAge = 3; // Max age (in seconds) is the result of: growthFreq * maxAge
    public float growthFreq = 15.0f;
    private float growthTimer;

    void Start()
    {
        growthTimer = growthFreq;
    }

    // Update is called once per frame
    void Update()
    {
        Timers();

        GrowOrDie();
    }

    void Timers()
    {
        growthTimer -= Time.deltaTime;
    }

    public void SpawnFood(GameObject prefab, Vector3 spawnPos)
    {
        GameObject newFood = Instantiate(prefab, spawnPos, Quaternion.identity);
        foodList.Add(newFood);
        
        Food foodScript = newFood.GetComponent<Food>();
        if(foodScript == null)
        {
            Debug.LogError("foodScript was null on SpawnFood() in FoodManager.cs");
            return;
        }
        foodScript.SetUUID(spawnedFood);
        spawnedFood++;
    }

    // Food increases age and if it is not eaten it will "die"
    private void GrowOrDie()
    {
        if(growthTimer <= 0)
        {
            growthTimer = growthFreq;

            for (int i = 0; i < foodList.Count; i++)
            {
                Food foodScript = foodList[i].GetComponent<Food>();
                if (foodScript == null)
                {
                    Debug.LogError("Food script couldn't be found for foodList[" + i + "] in FoodManager.cs");
                    continue;
                }

                // Update the age, or kill it if it's too old
                if (foodScript.age + 1 > maxAge)
                {
                    // Kill food
                    KillFood(foodScript.gameObject);
                }
                else foodScript.age++;
            }
        }
    }

    // This function should be accessed by animals when they eat the food
    // This should be the only way to remove food from foodList
    public void KillFood(GameObject foodToDelete)
    {
        Food foodToDeleteScript = foodToDelete.GetComponent<Food>();
        if (foodToDeleteScript == null)
        {
            Debug.LogError("foodToDeleteScript was null on KillFood() in FoodManager.cs");
            return;
        }

        for (int i = 0; i < foodList.Count; ++i)
        {
            Food foodScript = foodList[i].GetComponent<Food>();
            if (foodScript == null)
            {
                Debug.LogError("foodScript was null on KillFood() in FoodManager.cs");
                return;
            }

            if (foodScript.GetUUID() == foodToDeleteScript.GetUUID())
            {
                foodList.RemoveAt(i);
                Destroy(foodToDelete);
                return;
            }            
        }

        Debug.LogError("food wasn't found in the list so it can't be deleted on FoodManager.cs");
    }
}
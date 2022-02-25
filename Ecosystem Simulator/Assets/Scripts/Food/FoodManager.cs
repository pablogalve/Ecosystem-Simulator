using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public List<GameObject> foodList;

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

    // Food increases age and if it is not eaten it will "die"
    void GrowOrDie()
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
                    foodList.RemoveAt(i);
                    Destroy(foodScript.gameObject);
                }
                else foodScript.age++;
            }
        }
    }
}
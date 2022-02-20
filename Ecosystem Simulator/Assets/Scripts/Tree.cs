using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    // AGE
    private float maxAge;
    public float age;

    // FOOD
    public GameObject foodPrefab;
    public float foodGenerationFreq;
    private float foodGenerationTimer;

    // REPRODUCTION
    public float reproductionFreq;
    public float reproductionTimer;

    // Start is called before the first frame update
    void Start()
    {
        age = 0.0f;

        // Set data from TreeDataHolder
        maxAge = TreeDataHolder.maxAge;
        foodGenerationFreq = TreeDataHolder.foodGenerationFreq;
        reproductionFreq = TreeDataHolder.reproductionFreq;

        // Initialize timers
        foodGenerationTimer = foodGenerationFreq;
        reproductionTimer = reproductionFreq;

        // Keep track of alive trees
        TreeDataHolder.amountOfTrees++;
    }

    // Update is called once per frame
    void Update()
    {
        Timers();

        FoodGeneration();

        Grow();

        Reproduce();

        if (age > maxAge) Die();        
    }

    private void Timers()
    {
        age += Time.deltaTime;
        foodGenerationTimer -= Time.deltaTime;
        reproductionTimer -= Time.deltaTime;
    }

    private void FoodGeneration()
    {
        if (foodGenerationTimer <= 0)
        {
            if (foodPrefab != null) Instantiate(foodPrefab);
            else Debug.LogError("foodPrefab is null on Tree.cs");

            foodGenerationTimer = foodGenerationFreq;
        }
    }

    private void Grow()
    {
        //The tree's scale will increase as its age increases
        gameObject.transform.localScale = Vector3.one * (age / maxAge);
    }

    private void Reproduce()
    {
        if(reproductionTimer <= 0)
        {
            reproductionTimer = reproductionFreq;
            
            // Generate a spawn position
            Vector3 spawnPos = gameObject.transform.position;
            spawnPos.x += GetRandomVariation(-20.0f, +20.0f);            
            spawnPos.z += GetRandomVariation(-20.0f, +20.0f);
            spawnPos = HeighmapData.GetTerrainHeight(spawnPos.x, spawnPos.z);

            Instantiate(gameObject, spawnPos, Quaternion.identity);
        }
    }

    private void Die()
    {
        // Keep track of alive trees
        TreeDataHolder.amountOfTrees--;
                
        Debug.Log("Amount of trees: " + TreeDataHolder.amountOfTrees);
        
        Destroy(gameObject);
    }

    private float GetRandomVariation(float min, float max)
    {
        return Random.Range(min, max);
    }
}

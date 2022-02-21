using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public GameObject treePrefab = null;
    public List<GameObject> trees = new List<GameObject>();
    public int amountOfTrees = 0;
    public int maxTrees = 3000;

    // AGE
    public byte maxAge = 10; // Max age (in seconds) is the result of: growthFreq * maxAge
    public float growthFreq = 10.0f;
    private float growthTimer;

    // FOOD
    public GameObject foodPrefab;
    public float foodGenerationFreq = 30.0f;
    private float foodGenerationTimer;

    // REPRODUCTION
    public float reproductionFreq = 5.0f;
    public float reproductionTimer;

    void Start()
    {
        {
            // Create initial tree
            // Generate a spawn position
            TryToSpawn(treePrefab, 0f, 0f, 0.0f);            
        }
    }

    // Update is called once per frame
    void Update()
    {
        Timers();

        FoodGeneration();

        GrowOrDie();

        Reproduce();
    }

    private void Timers()
    {        
        growthTimer -= Time.deltaTime;
        foodGenerationTimer -= Time.deltaTime;
        reproductionTimer -= Time.deltaTime;
    }

    private void FoodGeneration()
    {

    }

    private void GrowOrDie()
    {
        if(growthTimer <= 0f)
        {
            growthTimer = growthFreq;

            for (int i = 0; i < trees.Count; i++)
            {
                // Try to get tree script, or fail with error log
                Tree tree = trees[i].GetComponent<Tree>();
                if (tree == null)
                {
                    Debug.LogError("tree " + i + " is null on TreeManager.cs. Couldn't update age");
                    continue;
                }

                // Update the age, or kill it if it's too old
                if (tree.age + 1 > maxAge) {
                    // Kill tree
                    trees.RemoveAt(i);
                    Destroy(tree.gameObject);

                    // Keep track of alive trees
                    amountOfTrees--;
                }
                else tree.age++;

                // Scale the tree in the world according to its age
                float agePercentage = (float)(tree.age) / (float)(maxAge);
                trees[i].transform.localScale = Vector3.one * agePercentage;
            }
        }        
    }

    private void Reproduce()
    {
        if (reproductionTimer <= 0)
        {
            reproductionTimer = reproductionFreq;

            for(int i = 0; i < trees.Count; i++)
            {
                TryToSpawn(trees[i], trees[i].transform.position.x, trees[i].transform.position.z, 200.0f);                
            }            
        }
    }

    private float GetRandomVariation(float min, float max)
    {
        return Random.Range(min, max);
    }

    private bool isValidSpawnPoint(Vector3 position)
    {
        if (position == null) return false;

        // TODO: Numbers shouldn't be hardcoded
        if (position.y < 25.0f) return false; // Position too low, out of grass bioma 
        if (position.y > 115.0f) return false; // Position too high, out of grass bioma 

        return true;
    }

    private void TryToSpawn(GameObject prefab, float x, float z, float randomVariation)
    {
        if(prefab == null) return;

        // Generate a spawn position
        Vector3 spawnPos = prefab.transform.position;
        spawnPos.x += GetRandomVariation(-randomVariation, randomVariation);
        spawnPos.z += GetRandomVariation(-randomVariation, randomVariation);
        spawnPos = HeighmapData.GetTerrainHeight(spawnPos.x, spawnPos.z);

        if (isValidSpawnPoint(spawnPos) && amountOfTrees < maxTrees)
        {
            GameObject babyTree = Instantiate(prefab, spawnPos, Quaternion.identity);
            trees.Add(babyTree);
            amountOfTrees++;
        }
    }
}
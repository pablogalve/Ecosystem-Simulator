using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public enum Types{
        TREE,
        FOOD
    }

    public GameObject treePrefab = null;
    public List<GameObject> trees = new List<GameObject>();
    public int maxTrees = 3000;

    // AGE
    public byte maxAge = 10; // Max age (in seconds) is the result of: growthFreq * maxAge
    public float growthFreq = 10.0f;
    private float growthTimer;

    // FOOD
    FoodManager foodManager = null;
    public GameObject foodPrefab = null;
    public float foodGenerationFreq = 30.0f;
    private float foodGenerationTimer;
    public float foodGenerationAge = 0.5f; // 0.0f <> 1.0f // Percentage of age when tree can start to produce food

    // REPRODUCTION
    public float reproductionFreq = 5.0f;
    private float reproductionTimer;
    public float reproductionAge = 0.5f; // 0.0f <> 1.0f // Percentage of age when tree can start to reproduce

    void Start()
    {
        SetInitialTreeScene();

        foodManager = GetComponent<FoodManager>();
        if (foodManager == null) Debug.LogError("FoodManager script couldn't be found on TreeManager.cs");
    }

    void SetInitialTreeScene()
    {
        TryToSpawn(Types.TREE, treePrefab, 0f, 0f, 0.0f);
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
        if(foodGenerationTimer <= 0f)
        {
            foodGenerationTimer = foodGenerationFreq;

            for(int i = 0; i < trees.Count; i++)
            {
                Tree treeScript = trees[i].GetComponent<Tree>();
                if (treeScript == null)
                {
                    Debug.LogError("tree " + i + " is null on TreeManager.cs: FoodGeneration");
                    continue;
                }

                // Check that tree is old enough to produce food
                if (treeScript.age >= foodGenerationAge * maxAge)
                {
                    TryToSpawn(Types.FOOD, foodPrefab, trees[i].transform.position.x, trees[i].transform.position.z, 2f);
                }    
            }
        }
    }

    private void GrowOrDie()
    {
        if(growthTimer <= 0f)
        {
            growthTimer = growthFreq;

            for (int i = 0; i < trees.Count; i++)
            {
                // Try to get tree script, or fail with error log
                Tree treeScript = trees[i].GetComponent<Tree>();
                if (treeScript == null)
                {
                    Debug.LogError("tree " + i + " is null on TreeManager.cs. Couldn't update age");
                    continue;
                }

                // Update the age, or kill it if it's too old
                if (treeScript.age + 1 > maxAge) {
                    // Kill tree
                    trees.RemoveAt(i);
                    Destroy(treeScript.gameObject);
                }
                else treeScript.age++;

                // Scale the tree in the world according to its age
                float agePercentage = (float)(treeScript.age) / (float)(maxAge);
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
                Tree treeScript = trees[i].GetComponent<Tree>();
                if (treeScript == null)
                {
                    Debug.LogError("tree " + i + " is null on TreeManager.cs: Reproduce");
                    continue;
                }

                // Check that tree is old enough to reproduce
                if (treeScript.age >= reproductionAge * maxAge)
                {
                    TryToSpawn(Types.TREE, trees[i], trees[i].transform.position.x, trees[i].transform.position.z, 200.0f);
                }                          
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

    private void TryToSpawn(Types type, GameObject prefab, float x, float z, float randomVariation)
    {
        if(prefab == null) return;

        // Generate a spawn position
        Vector3 spawnPos = new Vector3(x, 0f, z);
        spawnPos.x += GetRandomVariation(-randomVariation, randomVariation);
        spawnPos.z += GetRandomVariation(-randomVariation, randomVariation);
        spawnPos = HeighmapData.GetTerrainHeight(spawnPos.x, spawnPos.z);

        if (!isValidSpawnPoint(spawnPos)) return;

        if (type == Types.TREE)
        {
            if (trees.Count < maxTrees)
            {
                GameObject babyTree = Instantiate(prefab, spawnPos, Quaternion.identity);
                trees.Add(babyTree);
            }
        }
        else if(type == Types.FOOD)
        {
            foodManager.SpawnFood(prefab, spawnPos);
        }       
    }
}
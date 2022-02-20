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
    public float maxAge = 20.0f;

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

        Grow();

        Reproduce();

        Death();
    }

    private void Timers()
    {
        for(int i = 0; i < trees.Count; i++)
        {
            Tree tree = trees[i].GetComponent<Tree>();
            if (tree != null) tree.age += Time.deltaTime;
            else Debug.LogError("tree " + i + " is null on TreeManager.cs. Couldn't update age");
        }
        
        foodGenerationTimer -= Time.deltaTime;
        reproductionTimer -= Time.deltaTime;
    }

    private void FoodGeneration()
    {

    }

    private void Grow()
    {

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

    private void Death()
    {
        for (int i = 0; i < trees.Count; i++)
        {
            Tree tree = trees[i].GetComponent<Tree>();
            if (tree == null) {
                Debug.LogError("tree " + i + " is null on TreeManager.cs. Couldn't update age");
                continue;
            }
            
            if(tree.age >= maxAge)
            {
                trees.RemoveAt(i);
                Destroy(tree.gameObject);

                // Keep track of alive trees
                amountOfTrees--;
            }
        }            

        Debug.Log("Amount of trees: " + amountOfTrees);        
    }

    private float GetRandomVariation(float min, float max)
    {
        return Random.Range(min, max);
    }

    private bool isValidSpawnPoint(Vector3 position)
    {
        if (position == null) return false;
        if (position.y < 20.0f || position.y > 115.0f) return false; // Out of grass bioma // TODO: Numbers shouldn't be hardcoded

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
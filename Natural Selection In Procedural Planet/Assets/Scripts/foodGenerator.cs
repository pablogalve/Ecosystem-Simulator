using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodGenerator : MonoBehaviour
{
    public float timer = 20.0f;
    public float currTimer;

    public GameObject foodPrefab;    


    void Start(){
        currTimer = timer;
    }

    // Update is called once per frame
    void Update()
    {
        if(currTimer <= 0.0f){
            currTimer = timer;
            SpawnFood();
        }else currTimer -= Time.deltaTime;
    }

    void SpawnFood(){
        GameObject newFood = Instantiate(foodPrefab, transform.position, Quaternion.identity);
        DataHolder.AddToList("food", newFood);
    }
}

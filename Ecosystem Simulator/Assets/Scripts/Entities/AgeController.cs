using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgeController : MonoBehaviour
{
    public byte age = 0;
    public byte maxAge = 10;
    public float reproductionAge = 0.5f; // 0.0f <> 1.0f // Percentage of age when tree can start to reproduce
    public bool growsInSize = false;
    public GameObject prefab;

    private void Start()
    {
        // Initialize at age 0 and size 0
        {
            age = 0;
            if(growsInSize) transform.transform.localScale = Vector3.zero;
        }
    }
    public bool canReproduce()
    {
        return age >= maxAge * reproductionAge;
    }
}

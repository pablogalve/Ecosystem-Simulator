using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgeController : MonoBehaviour
{
    public byte age = 1;
    public byte maxAge = 10;
    public float reproductionAge = 0.5f; // 0.0f <> 1.0f // Percentage of age when tree can start to reproduce
    public bool growsInSize = false;
    public float scaleFactor = 1.0f; // Increase or decrease if the fbx is not at scale

    public void OnSpawn()
    {
        if (growsInSize)
        {
            float agePercentage = (float)(age) / (float)(maxAge);
            transform.localScale = Vector3.one * agePercentage * 2 * scaleFactor;
        }
    }

    public bool IsBaby()
    {
        return age < maxAge * reproductionAge;
    }
}
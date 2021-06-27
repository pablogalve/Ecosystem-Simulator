using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomNumbers : MonoBehaviour
{
    public static Vector3 GetRandomVector3()
    {
        Vector3 min = new Vector3(-60, -60, -60);
        Vector3 max = new Vector3(70, 70, 70);
        return new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
    }
}

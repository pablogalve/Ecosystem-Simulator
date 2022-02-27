using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    // AGE
    public byte age; // 0 - 255

    void Start()
    {
        age = 0;
        transform.localScale = Vector3.zero; // Trees initial scale is 0f until they start growing
    }
}

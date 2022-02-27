using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    // AGE
    public byte age; // 0 - 255
    private int uuid;

    void Start()
    {
        age = 0;
    }

    public void SetUUID(int num)
    {
        uuid = num;
    }

    public int GetUUID()
    {
        return uuid;
    }
}

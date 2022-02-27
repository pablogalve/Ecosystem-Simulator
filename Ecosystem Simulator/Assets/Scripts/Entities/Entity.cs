using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // AGE
    public byte age; // 0 - 255
    private string uuid;

    void Start()
    {
        age = 0;
        SetUUID();
    }

    private void SetUUID()
    {
        uuid = System.Guid.NewGuid().ToString();
    }

    public string GetUUID()
    {
        return uuid;
    }
}

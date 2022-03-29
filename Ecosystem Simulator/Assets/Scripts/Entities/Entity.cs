using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private string uuid;

    void Awake()
    {
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

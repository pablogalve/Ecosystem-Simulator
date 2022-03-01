using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private string uuid;
    //public bool isActive; // TODO: Should I do an entity pool? 

    void Start()
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

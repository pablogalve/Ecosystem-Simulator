using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    private float speed = 0.5f;
    
    void Update()
    {
        Vector3 newPos = gameObject.transform.position;
        newPos.z += Time.deltaTime * speed;
        gameObject.transform.position = newPos;
    }
}

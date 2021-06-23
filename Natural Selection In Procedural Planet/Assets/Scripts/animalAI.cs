using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animalAI : MonoBehaviour
{
    float speed = 3.0f;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}

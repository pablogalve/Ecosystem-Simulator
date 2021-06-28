using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animalAI : MonoBehaviour
{
    public float speed = 3.0f;
    public float hunger = 100.0f;

    protected virtual void Update()
    {
        Move();
        UpdateStats();
    }

    void Move()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void UpdateStats()
    {
        hunger -= Time.deltaTime;

        if (hunger <= 0.0f)
            Destroy(this.gameObject);
    }
}

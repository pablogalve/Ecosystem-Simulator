﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animalAI : MonoBehaviour
{
    public float speed = 3.0f;
    public float hunger = 100.0f;

    public List<GameObject> visibleCarnivores = new List<GameObject>();
    public List<GameObject> visibleHervibores = new List<GameObject>();
    public List<GameObject> visibleFood = new List<GameObject>();

    protected virtual void Update()
    {
        Move();
        UpdateStats();
    }

    void Move()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    protected virtual void UpdateStats()
    {
        hunger -= Time.deltaTime;

        if (hunger <= 0.0f)
            entityManager.KillEntity(this.gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        var multiTag = other.gameObject.GetComponent<CustomTag>();

        {   //Keep track of objects inside area of vision
            if (other.gameObject.tag == "Food")
                visibleFood.Add(other.gameObject);

            if (multiTag != null)
            {
                if (multiTag.HasTag("Carnivore"))
                    visibleCarnivores.Add(other.gameObject);
                else if (multiTag.HasTag("Hervibore"))
                    visibleHervibores.Add(other.gameObject);
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        var multiTag = other.gameObject.GetComponent<CustomTag>();

        {   //Keep track of objects inside area of vision
            if (other.gameObject.tag == "Food")
                visibleFood.Remove(other.gameObject);

            if (multiTag != null)
            {
                if (multiTag.HasTag("Carnivore"))
                    visibleCarnivores.Remove(other.gameObject);
                else if (multiTag.HasTag("Hervibore"))
                    visibleHervibores.Remove(other.gameObject);
            }
        }
    }
}

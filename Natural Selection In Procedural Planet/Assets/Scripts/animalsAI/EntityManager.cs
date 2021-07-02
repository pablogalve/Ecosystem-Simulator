using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static List<GameObject> food = new List<GameObject>();
    public static List<GameObject> herbivores = new List<GameObject>();
    public static List<GameObject> carnivores = new List<GameObject>();

    public static void CreateEntity(string entityType, GameObject newObj)
    {
        switch (entityType)
        {
            case "food":
                if (food == null)
                    food = new List<GameObject>();

                if (!food.Contains(newObj))
                    food.Add(newObj);
                break;
            case "herbivore":
                if (herbivores == null)
                    herbivores = new List<GameObject>();

                if (!herbivores.Contains(newObj))
                    herbivores.Add(newObj);
                break;
            case "carnivore":
                if (carnivores == null)
                    carnivores = new List<GameObject>();

                if (!carnivores.Contains(newObj))
                    carnivores.Add(newObj);
                break;
            default:
                Debug.Log("AddToList: 'ListName' is not valid");
                break;
        }
    }
    public static void KillEntity(GameObject killedObj)
    {
        killedObj.GetComponent<entity>().toDelete = true;

        var multiTag = killedObj.gameObject.GetComponent<CustomTag>();

        if (killedObj.tag == "Food")
            RemoveFoodFromLists();

        if (multiTag != null)
        {
            if (multiTag.HasTag("Herbivores"))
                RemoveHerbivoresFromLists();
            if (multiTag.HasTag("Omnivores"))
                RemoveOmnivoresFromLists();
            if (multiTag.HasTag("Carnivores"))
                RemoveCarnivoresFromLists();
        }

        Destroy(killedObj);
    }

    private static void RemoveFoodFromLists()
    {
        //Remove food from food list
        for (var i = food.Count - 1; i > -1; i--)
        {
            if (food[i].GetComponent<entity>().toDelete)
                food.RemoveAt(i);
        }

        if (herbivores != null)
        {
            for (var i = herbivores.Count - 1; i > -1; i--)
            {
                //Remove food from herbivores visible area
                if (herbivores[i] != null)
                {
                    for (var j = herbivores[i].GetComponent<animalAI>().visibleFood.Count - 1; j > -1; j--)
                    {
                        if (herbivores[i].GetComponent<animalAI>().visibleFood[j].GetComponent<entity>().toDelete)
                            herbivores[i].GetComponent<animalAI>().visibleFood.RemoveAt(j);
                    }
                }
            }
        }

        if (carnivores != null)
        {
            for (var i = carnivores.Count - 1; i > -1; i--)
            {
                //Remove food from carnivores visible area
                if (carnivores[i] != null)
                {
                    for (var j = carnivores[i].GetComponent<animalAI>().visibleFood.Count - 1; j > -1; j--)
                    {
                        if (carnivores[i].GetComponent<animalAI>().visibleFood[j].GetComponent<entity>().toDelete)
                            carnivores[i].GetComponent<animalAI>().visibleFood.RemoveAt(j);
                    }
                }
            }
        }

    }

    private static void RemoveHerbivoresFromLists()
    {
        //Remove herbivores from hervbivores list
        for (var i = herbivores.Count - 1; i >= 0; i--)
        {
            if (herbivores[i].GetComponent<entity>().toDelete)
                herbivores.RemoveAt(i);
        }

        for (var i = herbivores.Count - 1; i > -1; i--)
        {
            //Remove other herbivores from herbivores visible area
            for (var j = herbivores[i].GetComponent<animalAI>().visibleHerbivores.Count - 1; j > -1; j--)
            {
                if (herbivores[i].GetComponent<animalAI>().visibleHerbivores[j].GetComponent<entity>().toDelete)
                    herbivores[i].GetComponent<animalAI>().visibleHerbivores.RemoveAt(j);
            }
        }

        for (var i = carnivores.Count - 1; i > -1; i--)
        {
            //Remove herbivores from carnivores visible area
            for (var j = carnivores[i].GetComponent<animalAI>().visibleHerbivores.Count - 1; j > -1; j--)
            {
                if (carnivores[i].GetComponent<animalAI>().visibleHerbivores[j].GetComponent<entity>().toDelete)
                    carnivores[i].GetComponent<animalAI>().visibleHerbivores.RemoveAt(j);
            }
        }
    }

    private static void RemoveOmnivoresFromLists()
    {

    }

    private static void RemoveCarnivoresFromLists()
    {
        //Remove carnivores from carnivores list
        for (var i = herbivores.Count - 1; i >= 0; i--)
        {
            if (herbivores[i].GetComponent<entity>().toDelete)
                herbivores.RemoveAt(i);
        }

        for (var i = herbivores.Count - 1; i > -1; i--)
        {
            //Remove carnivores from herbivores visible area
            for (var j = herbivores[i].GetComponent<animalAI>().visibleCarnivores.Count - 1; j > -1; j--)
            {
                if (herbivores[i].GetComponent<animalAI>().visibleCarnivores[j].GetComponent<entity>().toDelete)
                    herbivores[i].GetComponent<animalAI>().visibleCarnivores.RemoveAt(j);
            }
        }

        for (var i = carnivores.Count - 1; i > -1; i--)
        {
            //Remove carnivores from carnivores visible area
            for (var j = carnivores[i].GetComponent<animalAI>().visibleCarnivores.Count - 1; j > -1; j--)
            {
                if (carnivores[i].GetComponent<animalAI>().visibleCarnivores[j].GetComponent<entity>().toDelete)
                    carnivores[i].GetComponent<animalAI>().visibleCarnivores.RemoveAt(j);
            }
        }
    }
}
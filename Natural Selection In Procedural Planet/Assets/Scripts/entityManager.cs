using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static List<GameObject> food = new List<GameObject>();
    public static List<GameObject> hervibores = new List<GameObject>();
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
            case "hervibores":
                if (hervibores == null)
                    hervibores = new List<GameObject>();

                if (!hervibores.Contains(newObj))
                    hervibores.Add(newObj);
                break;
            case "carnivores":
                if (carnivores == null)
                    carnivores = new List<GameObject>();

                if (!carnivores.Contains(newObj))
                    carnivores.Add(newObj);
                break;
                break;
            default:
                Debug.Log("AddToList: 'ListName' is not valid");
                break;
        }
    }
    public static void KillEntity(GameObject killedObj)
    {
        killedObj.GetComponent<entity>().toDelete = true;
        RemoveToDeleteObjectsFromLists();
        Destroy(killedObj);
    }

    private static void RemoveToDeleteObjectsFromLists()
    {
        for (var i = hervibores.Count - 1; i > -1; i--)
        {
            if (hervibores[i].GetComponent<entity>().toDelete)
                hervibores[i].GetComponent<herviboreAI>().visibleFood.RemoveAt(i);
        }

        for (var i = food.Count - 1; i > -1; i--)
        {
            if (food[i].GetComponent<entity>().toDelete)
                food.RemoveAt(i);
        }

        for (var i = hervibores.Count - 1; i > -1; i--)
        {
            for (var j = hervibores[i].GetComponent<animalAI>().visibleFood.Count - 1; j > -1; j--)
            {
                if (hervibores[i].GetComponent<animalAI>().visibleFood[j].GetComponent<entity>().toDelete)
                {
                    hervibores[i].GetComponent<animalAI>().visibleFood.RemoveAt(j);
                }
            }
        }
    }
}
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
            case "herbivores":
                if (herbivores == null)
                    herbivores = new List<GameObject>();

                if (!herbivores.Contains(newObj))
                    herbivores.Add(newObj);
                break;
            case "carnivores":
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
        RemoveToDeleteObjectsFromLists();
        Destroy(killedObj);
    }

    private static void RemoveToDeleteObjectsFromLists()
    {
        //Remove deleted objects from visibleAreas
        {
            //Remove all objects from herbivores visible area
            for (var i = herbivores.Count - 1; i > -1; i--)
            {
                //Remove food from herbivores visible area
                for (var j = herbivores[i].GetComponent<animalAI>().visibleFood.Count - 1; j > -1; j--)
                {
                    if (herbivores[i].GetComponent<animalAI>().visibleFood[j].GetComponent<entity>().toDelete)
                        herbivores[i].GetComponent<animalAI>().visibleFood.RemoveAt(j);
                }

                //Remove other herbivores from herbivores visible area
                for (var j = herbivores[i].GetComponent<animalAI>().visibleHerbivores.Count - 1; j > -1; j--)
                {
                    if (herbivores[i].GetComponent<animalAI>().visibleHerbivores[j].GetComponent<entity>().toDelete)
                        herbivores[i].GetComponent<animalAI>().visibleHerbivores.RemoveAt(j);
                }

                //Remove carnivores from herbivores visible area
                for (var j = herbivores[i].GetComponent<animalAI>().visibleCarnivores.Count - 1; j > -1; j--)
                {
                    if (herbivores[i].GetComponent<animalAI>().visibleCarnivores[j].GetComponent<entity>().toDelete)
                        herbivores[i].GetComponent<animalAI>().visibleCarnivores.RemoveAt(j);
                }
            }

            //Remove objects from carnivores visible area
            for (var i = carnivores.Count - 1; i > -1; i--)
            {
                //Remove food from carnivores visible area
                for (var j = carnivores[i].GetComponent<animalAI>().visibleFood.Count - 1; j > -1; j--)
                {
                    if (carnivores[i].GetComponent<animalAI>().visibleFood[j].GetComponent<entity>().toDelete)
                        carnivores[i].GetComponent<animalAI>().visibleFood.RemoveAt(j);
                }

                //Remove herbivores from carnivores visible area
                for (var j = carnivores[i].GetComponent<animalAI>().visibleHerbivores.Count - 1; j > -1; j--)
                {
                    if (carnivores[i].GetComponent<animalAI>().visibleHerbivores[j].GetComponent<entity>().toDelete)
                        carnivores[i].GetComponent<animalAI>().visibleHerbivores.RemoveAt(j);
                }

                //Remove carnivores from carnivores visible area
                for (var j = carnivores[i].GetComponent<animalAI>().visibleCarnivores.Count - 1; j > -1; j--)
                {
                    if (carnivores[i].GetComponent<animalAI>().visibleCarnivores[j].GetComponent<entity>().toDelete)
                        carnivores[i].GetComponent<animalAI>().visibleCarnivores.RemoveAt(j);
                }
            }
        }

        //Objects list
        {
            //Remove herbivores from hervbivores list
            for (var i = herbivores.Count - 1; i >= 0; i--)
            {
                if (herbivores[i].GetComponent<entity>().toDelete)
                    herbivores.RemoveAt(i);
            }

            //Remove food from food list
            for (var i = food.Count - 1; i > -1; i--)
            {
                if (food[i].GetComponent<entity>().toDelete)
                    food.RemoveAt(i);
            }

            //Remove carnivores from carnivores list
            for (var i = herbivores.Count - 1; i >= 0; i--)
            {
                if (herbivores[i].GetComponent<entity>().toDelete)
                    herbivores.RemoveAt(i);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    public static List<GameObject> animals;
    public static List<GameObject> food;

    public static float hungerIncrementAtEating = 50.0f;

    public static void AddToList(string listName, GameObject newObj)
    {
        switch (listName)
        {
            case "food":
                if (food == null)
                    food = new List<GameObject>();

                food.Add(newObj);
            break;
            case "animals":
                if (animals == null)
                    animals = new List<GameObject>();

                animals.Add(newObj);
            break;
            default:
                Debug.Log("AddToList: 'ListName' is not valid");
            break;
        }
        
    }
}

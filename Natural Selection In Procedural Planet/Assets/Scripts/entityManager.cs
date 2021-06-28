using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entityManager : MonoBehaviour
{
    public static void KillEntity(GameObject killedObj)
    {
        RemoveElementFromLists(killedObj);
        RemoveFromDataHolder(killedObj);
        Destroy(killedObj);
    }

    //Remove element from all the references in all the animals's lists
    private static void RemoveElementFromLists(GameObject killedObj)
    {
        var multiTag = killedObj.GetComponent<CustomTag>();

        int tag = -1;
        if (killedObj.tag == "Food") tag = 1;
        if (multiTag != null && multiTag.HasTag("Carnivore")) tag = 2;
        if (multiTag != null && multiTag.HasTag("Hervibore")) tag = 3;

        if (tag == -1)
            return;

        for (int i = 0; i < DataHolder.animals.Count; i++)
        {
            switch (tag)
            {
                case 1:
                    DataHolder.animals[i].GetComponent<animalAI>().visibleFood.Remove(killedObj);
                    break;
                case 2:
                    DataHolder.animals[i].GetComponent<animalAI>().visibleCarnivores.Remove(killedObj);
                    break;
                case 3:
                    DataHolder.animals[i].GetComponent<animalAI>().visibleHervibores.Remove(killedObj);
                    break;
                default:
                    break;
            }
        }
    }

    private static void RemoveFromDataHolder(GameObject killedObj)
    {
        if (killedObj.tag == "Food")
        {
            DataHolder.food.Remove(killedObj);
            return;
        }

        var multiTag = killedObj.GetComponent<CustomTag>();

        if (multiTag != null)
        {
            if (multiTag.HasTag("Carnivore") || multiTag.HasTag("Hervibore"))
            {
                DataHolder.animals.Remove(killedObj);
            }
        }
    }
}
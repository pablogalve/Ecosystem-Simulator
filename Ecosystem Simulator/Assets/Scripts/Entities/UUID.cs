using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UUID : MonoBehaviour
{
    // This script keeps a reference to the LinkedListNode from the EntityManager.cs' UUID's LinkedList to make kill operations easier

    private LinkedListNode<EntityManager.Entity> myInfo;

    public void SetMyUUIDInfo(LinkedListNode<EntityManager.Entity> entityNode)
    {
        myInfo = entityNode;
    }

    public LinkedListNode<EntityManager.Entity> GetMyUUIDInfo()
    {
        return myInfo;
    }
}

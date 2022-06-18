using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public List<Canvas> visibleCanvas = new List<Canvas>();
    

    // Update is called once per frame
    void Update()
    {
        FindObjects();
        RotateToCamera();
    }

    void RotateToCamera()
    {
        for(int i = 0; i < visibleCanvas.Count; i++)
        {
            Vector3 v = gameObject.transform.position - visibleCanvas[i].gameObject.transform.position;
            v.x = v.z = 0.0f;
            visibleCanvas[i].gameObject.transform.LookAt(gameObject.transform.position - v);
            visibleCanvas[i].gameObject.transform.Rotate(0, 180, 0);
        }        
    }

    void FindObjects()
    {
        visibleCanvas.Clear();

        Vector3 position = gameObject.transform.position;

        Collider[] hitColliders = Physics.OverlapSphere(position, 100.0f);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].transform.childCount == 0) continue;

            GameObject canvasGO = hitColliders[i].gameObject.transform.GetChild(0).gameObject;
            if (canvasGO == null) continue;

            Canvas canvas = canvasGO.GetComponent<Canvas>();
            if (canvas == null) continue;

            visibleCanvas.Add(canvas);
        }
    }
}
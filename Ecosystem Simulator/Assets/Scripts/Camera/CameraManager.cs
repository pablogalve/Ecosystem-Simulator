using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject mainCamera = null;
    CameraMovement cameraMovement = null;

    // The purpose of this script is to ensure that the camera is enabled at all times
    // When there is a selected gameObject, the camera is set as child of the selectedGO, but if the animal gets killed, the camera is disabled too
    // Therefore, it is needed to be checked constantly that the camera remains enabled
    void LateUpdate()
    {        
        if(mainCamera.activeInHierarchy == false)
        {
            mainCamera.transform.parent = null;
            mainCamera.SetActive(true);

            cameraMovement = mainCamera.GetComponent<CameraMovement>();

            if(cameraMovement != null)
            {
                cameraMovement.selectedGO = null;
            }
            else
            {
                throw new System.Exception("CameraMovement.cs has not been added to the camera, or the camera has not been added to CameraManager.cs in the inspector");
            }
        }
    }
}

using AnimationInstancingNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLODs : MonoBehaviour
{
    public float distance;

    private EntityManager eM = null;
    private Camera cam;

    private GPUInstancedRendering sheepRenderer = null;

    private void Start()
    {
        eM = GameObject.Find("GameManager").GetComponent<EntityManager>();
        cam = Camera.main;

        sheepRenderer = GameObject.Find("SheepUnanimatedRenderer").GetComponent<GPUInstancedRendering>();

        StartCoroutine(CullAnimators());
    }

    private IEnumerator CullAnimators()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        Vector3 cameraPos = cam.gameObject.transform.position;

        List<Matrix4x4> sheepMatrices = new List<Matrix4x4>();

        for (int i = 0; i < eM.entitiesByType[(int)EntityManager.EntityType.ANIMAL].Count; i++)
        {
            GameObject go = eM.entities[eM.entitiesByType[(int)EntityManager.EntityType.ANIMAL][i]];
            AnimationInstancing animator = go.GetComponent<AnimationInstancing>();
            if (IsInAngle(planes, go)) // Check if is in view using angle and distance
            {
                if((cameraPos - go.transform.position).sqrMagnitude < distance * distance) // It's close to the camera
                {
                    // Good quality fbx and animations
                    animator.enabled = true;
                }
                else 
                {
                    // GPU Instanced renderer without animations and lower quality fbx
                    animator.enabled = false;
                    Matrix4x4 matrix = Matrix4x4.TRS(
                            pos: go.transform.position,
                            q: Quaternion.Euler(-90 + go.transform.rotation.x, go.transform.rotation.y, go.transform.rotation.z),
                            s: go.transform.lossyScale
                            );
                    sheepMatrices.Add(matrix);
                }
            }
            else
            {
                animator.enabled = false;
            }
        }

        sheepRenderer.RecalculateMatrices(sheepMatrices);

        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CullAnimators());
    }

    private bool IsInAngle(Plane[] planes, GameObject go)
    {
        return GeometryUtility.TestPlanesAABB(planes, go.GetComponent<Collider>().bounds);
    }
}

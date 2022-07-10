using AnimationInstancingNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLODs : MonoBehaviour
{
    public float distance;
    public float updateRate;

    private EntityManager eM = null;
    private Camera cam;

    private GPUInstancedRendering sheepRenderer = null;
    private GPUInstancedRendering longhornMaleRenderer = null;
    private GPUInstancedRendering longhornFemaleRenderer = null;
    private GPUInstancedRendering wolfRenderer = null;

    private void Start()
    {
        eM = GameObject.Find("GameManager").GetComponent<EntityManager>();
        cam = Camera.main;

        sheepRenderer = GameObject.Find("SheepUnanimatedRenderer").GetComponent<GPUInstancedRendering>();
        longhornMaleRenderer = GameObject.Find("LonghornMaleUnanimatedRenderer").GetComponent<GPUInstancedRendering>();
        longhornFemaleRenderer = GameObject.Find("LonghornFemaleUnanimatedRenderer").GetComponent<GPUInstancedRendering>();
        wolfRenderer = GameObject.Find("WolfUnanimatedRenderer").GetComponent<GPUInstancedRendering>();

        StartCoroutine(CullAnimators()); 
    }    

    private IEnumerator CullAnimators()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        Vector3 cameraPos = cam.gameObject.transform.position;

        List<Matrix4x4> sheepMatrices = new List<Matrix4x4>();
        List<Matrix4x4> longhornMaleMatrices = new List<Matrix4x4>();
        List<Matrix4x4> longhornFemaleMatrices = new List<Matrix4x4>();
        List<Matrix4x4> wolfMatrices = new List<Matrix4x4>();

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
                    Animal animalScript = go.GetComponent<Animal>();                

                    // Add to the matrices list depending on the mesh                    
                    if (animalScript.species == AnimalManager.Species.SHEEP)
                    {
                        Matrix4x4 matrix = Matrix4x4.TRS(
                            pos: go.transform.position,
                            q: Quaternion.Euler(-90f + go.transform.rotation.x, go.transform.rotation.y, go.transform.rotation.z),
                            s: go.transform.localScale * 1.5f
                            );
                        sheepMatrices.Add(matrix);
                    }
                    else if (animalScript.species == AnimalManager.Species.LONGHORN)
                    {
                        Matrix4x4 matrix = Matrix4x4.TRS(
                            pos: go.transform.position,
                            q: Quaternion.Euler(go.transform.rotation.x, go.transform.rotation.y, go.transform.rotation.z),
                            s: go.transform.localScale
                            );

                        // In longhorns, the mesh is different depending on gender
                        Reproduction reproduction = go.GetComponent<Reproduction>();
                        if(reproduction.gender == 0) longhornFemaleMatrices.Add(matrix);
                        else longhornMaleMatrices.Add(matrix);
                    }
                    else if (animalScript.species == AnimalManager.Species.WOLF)
                    {
                        // Modify wolf matrices because its mesh is not working
                        Matrix4x4 matrix = Matrix4x4.TRS(
                            pos: new Vector3(go.transform.position.x, go.transform.position.y + 0.365f, go.transform.position.z),
                            q: Quaternion.Euler(go.transform.rotation.x -9.47f, go.transform.rotation.y, go.transform.rotation.z),
                            s: new Vector3(go.transform.localScale.x * 0.16f, go.transform.localScale.y * 0.26f, go.transform.localScale.z * 0.63f)
                            );
                        wolfMatrices.Add(matrix);
                    }
                }
            }
            else
            {
                animator.enabled = false;
            }

            if(i % 1000 == 0) yield return null;
        }

        sheepRenderer.RecalculateMatrices(sheepMatrices);
        longhornMaleRenderer.RecalculateMatrices(longhornMaleMatrices);
        longhornFemaleRenderer.RecalculateMatrices(longhornFemaleMatrices);
        wolfRenderer.RecalculateMatrices(wolfMatrices);

        yield return null;
        StartCoroutine(CullAnimators());
    }

    private bool IsInAngle(Plane[] planes, GameObject go)
    {
        return GeometryUtility.TestPlanesAABB(planes, go.GetComponent<Collider>().bounds);
    }
}

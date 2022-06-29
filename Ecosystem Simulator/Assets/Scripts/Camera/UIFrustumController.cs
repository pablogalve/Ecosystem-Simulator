using System.Collections.Generic;
using UnityEngine;

public class UIFrustumController : MonoBehaviour
{
    // Instanced Renderers for UI states
    public GPUInstancedRendering idleInstancedRenderer = null;
    public GPUInstancedRendering hungerInstancedRenderer = null;
    public GPUInstancedRendering loveInstancedRenderer = null;

    public float radius;

    // Update is called once per frame
    void FixedUpdate()
    {
        // Detect animals in view
        Collider[] hits = Physics.OverlapSphere(gameObject.transform.position, radius);

        // Calculate expected matrices
        List<Matrix4x4> idleMatrices = new List<Matrix4x4>();
        List<Matrix4x4> hungerMatrices = new List<Matrix4x4>();
        List<Matrix4x4> loveMatrices = new List<Matrix4x4>();

        for(int i = 0; i < hits.Length; i++)
        {
            Animal animalScript = hits[i].gameObject.GetComponent<Animal>();
            if (animalScript == null) continue;

            GameObject go = animalScript.gameObject;

            Matrix4x4 matrix = Matrix4x4.TRS(
                    pos: go.transform.position + new Vector3(0f, 2f, 0f),
                    q: Quaternion.LookRotation(go.transform.position - gameObject.transform.position),
                    s: new Vector3(1f, 1f, 1f)
                    );

            int state = (int)animalScript.GetState();
            switch (state)
            {
                case 0:
                    idleMatrices.Add(matrix);
                    break;
                case 1:
                    hungerMatrices.Add(matrix);
                    break;
                case 2:
                    loveMatrices.Add(matrix);
                    break;
                default:
                {
                    break;
                }
            }
        }

        // Send data to instanced renderer
        idleInstancedRenderer.RecalculateMatrices(idleMatrices);    
        hungerInstancedRenderer.RecalculateMatrices(hungerMatrices);    
        loveInstancedRenderer.RecalculateMatrices(loveMatrices);    
    }
}

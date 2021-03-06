using System.Collections.Generic;
using UnityEngine;

public class GPUInstancedRendering : MonoBehaviour
{
    public Mesh mesh;
    public Material[] materials;
    private List<List<Matrix4x4>> batches = new List<List<Matrix4x4>>();

    void LateUpdate()
    {
        RenderBatches();
    }

    private void RenderBatches()
    {
        foreach(var batch in batches)
        {
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                Graphics.DrawMeshInstanced(mesh, i, materials[i], batch);
            }
        }
    }

    public void RecalculateMatrices(List<Matrix4x4> matrices)
    {
        // Graphics.DrawMeshInstanced allows a maximum of 1023 instances at once
        int maxInstancesPerBatch = 1023;
        int addedMatrices = 0;    

        batches.Clear();
        batches.Add(new List<Matrix4x4>());

        for(int i = 0; i < matrices.Count; i++)
        {
            if(addedMatrices >= maxInstancesPerBatch)
            {
                batches.Add(new List<Matrix4x4>());
                addedMatrices = 0;
            }

            batches[batches.Count - 1].Add(matrices[i]);
            addedMatrices++;
        }
    }
}
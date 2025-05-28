
//Based on: QuickOutline Created by Chris Nolet on 3/30/18.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TafraKit
{
    [DisallowMultipleComponent]
    public class TafraOutline : MonoBehaviour
    {
        [Serializable]
        private class ListVector3
        {
            public List<Vector3> data;
        }

        private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

        public enum Mode
        {
            OutlineAll,
            OutlineVisible,
            OutlineHidden,
            OutlineAndSilhouette,
            SilhouetteOnly
        }

        [SerializeField] private Mode outlineMode;
        [SerializeField] private Color outlineColor = Color.black;
        [Range(0f, 10f)]
        [SerializeField] private float outlineWidth = 2f;

        [Header("Optional")]
        [SerializeField] private bool alwaysUpdate;

        [Header("References")]
        [SerializeField] private List<Renderer> renderers = new List<Renderer>();
        [SerializeField] private List<MeshFilter> meshFilters = new List<MeshFilter>();

        [SerializeField, HideInInspector] private List<Mesh> bakeKeys = new List<Mesh>();
        [SerializeField, HideInInspector] private List<ListVector3> bakeValues = new List<ListVector3>();

        private Material outlineMaskMaterial;
        private Material outlineFillMaterial;
        private bool needsUpdate;
        private List<Mesh> tempRegisteredMeshes = new List<Mesh>();

        public Mode OutlineMode
        {
            get { return outlineMode; }
            set
            {
                outlineMode = value;
                needsUpdate = true;
            }
        }
        public Color OutlineColor
        {
            get { return outlineColor; }
            set
            {
                outlineColor = value;
                needsUpdate = true;
            }
        }
        public float OutlineWidth
        {
            get { return outlineWidth; }
            set
            {
                outlineWidth = value;
                needsUpdate = true;
            }
        }
        public List<Renderer> Renderers => renderers;
        
        private void Awake()
        {
            if(renderers.Count == 0)
            {
                TafraDebugger.Log("Tafra Outline", "There are no renderers, the outline will not work.", TafraDebugger.LogType.Error);
                return;
            }

            // Instantiate outline materials
            outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
            outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));

            outlineMaskMaterial.name = "OutlineMask (Instance)";
            outlineFillMaterial.name = "OutlineFill (Instance)";

            // Retrieve or generate smooth normals
            LoadSmoothNormals();

            // Apply material properties immediately
            needsUpdate = true;
        }
        private void OnEnable()
        {
            StartCoroutine(LateOnEnable());
        }
        private void OnDisable()
        {
            for(int i = 0; i < renderers.Count; i++)
            {
                var renderer = renderers[i];

                // Remove outline shaders
                var materials = renderer.sharedMaterials.ToList();

                materials.Remove(outlineMaskMaterial);
                materials.Remove(outlineFillMaterial);

                renderer.materials = materials.ToArray();
            }
        }
        private void OnValidate()
        {
            // Update material properties
            needsUpdate = true;
        }
        private void OnDestroy()
        {
            for(int i = 0; i < tempRegisteredMeshes.Count; i++)
            {
                registeredMeshes.Remove(tempRegisteredMeshes[i]);
            }

            // Destroy material instances
            Destroy(outlineMaskMaterial);
            Destroy(outlineFillMaterial);
        }
        private void Update()
        {
            if(needsUpdate || alwaysUpdate)
            {
                needsUpdate = false;
                UpdateMaterialProperties();
            }
        }

        //To give a chance to other whoever creates material instances on awake/start/enable. Because if that happened after we assign the outline materials, we'll lose the reference.
        private IEnumerator LateOnEnable()
        {
            yield return Yielders.EndOfFrame;

            for(int i = 0; i < renderers.Count; i++)
            {
                var renderer = renderers[i];

                // Append outline shaders
                var materials = renderer.sharedMaterials.ToList();

                materials.Add(outlineMaskMaterial);
                materials.Add(outlineFillMaterial);

                renderer.materials = materials.ToArray();
            }
        }

        private void LoadSmoothNormals()
        {
            // Retrieve or generate smooth normals
            for(int i = 0; i < meshFilters.Count; i++)
            {
                var meshFilter = meshFilters[i];

                if(meshFilter == null)
                    continue;

                // Skip if smooth normals have already been adopted
                if(!registeredMeshes.Add(meshFilter.sharedMesh))
                    continue;

                tempRegisteredMeshes.Add(meshFilter.sharedMesh);

                // Retrieve or generate smooth normals
                var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
                var smoothNormals = (index >= 0) ? bakeValues[index].data : SmoothNormals(meshFilter.sharedMesh);

                // Store smooth normals in UV3
                meshFilter.sharedMesh.SetUVs(3, smoothNormals);

                // Combine submeshes
                var renderer = renderers[i];

                if(renderer != null)
                {
                    CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
                }
            }

            // Clear UV3 on skinned mesh renderers
            for(int i = 0; i < renderers.Count; i++)
            {
                var renderer = renderers[i];

                if(renderer is not SkinnedMeshRenderer skinnedMeshRenderer)
                    continue;

                // Skip if UV3 has already been reset
                if(!registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
                    continue;

                tempRegisteredMeshes.Add(skinnedMeshRenderer.sharedMesh);

                // Clear UV3
                skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];

                // Combine submeshes
                CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
            }
        }
        private void Bake()
        {
            bakeKeys.Clear();
            bakeValues.Clear();

            // Generate smooth normals for each mesh
            var bakedMeshes = new HashSet<Mesh>();

            for(int i = 0; i < meshFilters.Count; i++)
            {
                MeshFilter meshFilter = meshFilters[i];

                if(meshFilter == null)
                    continue;

                // Skip duplicates
                if(!bakedMeshes.Add(meshFilter.sharedMesh))
                    continue;

                // Serialize smooth normals
                var smoothNormals = SmoothNormals(meshFilter.sharedMesh);

                bakeKeys.Add(meshFilter.sharedMesh);
                bakeValues.Add(new ListVector3() { data = smoothNormals });
            }
        }
        private List<Vector3> SmoothNormals(Mesh mesh)
        {
            // Group vertices by location
            var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

            // Copy normals to a new list
            var smoothNormals = new List<Vector3>(mesh.normals);

            // Average normals for grouped vertices
            foreach(var group in groups)
            {

                // Skip single vertices
                if(group.Count() == 1)
                {
                    continue;
                }

                // Calculate the average normal
                var smoothNormal = Vector3.zero;

                foreach(var pair in group)
                {
                    smoothNormal += smoothNormals[pair.Value];
                }

                smoothNormal.Normalize();

                // Assign smooth normal to each vertex
                foreach(var pair in group)
                {
                    smoothNormals[pair.Value] = smoothNormal;
                }
            }

            return smoothNormals;
        }
        private void CombineSubmeshes(Mesh mesh, Material[] materials)
        {

            // Skip meshes with a single submesh
            if(mesh.subMeshCount == 1)
            {
                return;
            }

            // Skip if submesh count exceeds material count
            if(mesh.subMeshCount > materials.Length)
            {
                return;
            }

            // Append combined submesh
            mesh.subMeshCount++;
            mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
        }
        private void UpdateMaterialProperties()
        {

            // Apply properties according to mode
            outlineFillMaterial.SetColor("_OutlineColor", outlineColor);

            switch(outlineMode)
            {
                case Mode.OutlineAll:
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                    break;

                case Mode.OutlineVisible:
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                    break;

                case Mode.OutlineHidden:
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                    outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                    break;

                case Mode.OutlineAndSilhouette:
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                    break;

                case Mode.SilhouetteOnly:
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                    outlineFillMaterial.SetFloat("_OutlineWidth", 0f);
                    break;
            }
        }

        public void FetchReferences()
        {
            renderers.Clear();
            meshFilters.Clear();

            // Cache renderers
            Renderer[] renderersArray = GetComponentsInChildren<Renderer>();

            for(int i = 0; i < renderersArray.Length; i++)
            {
                Renderer renderer = renderersArray[i];

                if(renderer is not MeshRenderer && renderer is not SkinnedMeshRenderer)
                    continue;

                renderers.Add(renderersArray[i]);

                MeshFilter mf = renderer.GetComponent<MeshFilter>();

                //We'll still add the mesh filter even if it's null, this way we guarantee that each element in the renderers list will have it's mesh filter in the same index in the filters list.
                meshFilters.Add(mf);
            }

            Bake();
        }
    }
}
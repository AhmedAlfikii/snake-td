#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.AI.Navigation.Editor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class NavMeshBaker : MonoBehaviour
{
    private UnityEvent onBaked = new UnityEvent();

    public UnityEvent OnBaked => onBaked;

    public void Bake()
    {
        GameObject modifiers = null;

        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if(child.name == "NavModifiers")
            {
                modifiers = child.gameObject;
                modifiers.SetActive(true);
            }
        }

        List<NavMeshSurface> surfaces = new List<NavMeshSurface>();
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            NavMeshSurface surface = child.GetComponent<NavMeshSurface>();

            if(surface == null)
                continue;
                
            surfaces.Add(surface);
        }

        NavMeshAssetManager.instance.StartBakingSurfaces(surfaces.ToArray());
        
        if(modifiers != null)
            modifiers.SetActive(false);

        StartCoroutine(Baking(surfaces));
    }
    private IEnumerator Baking(List<NavMeshSurface> surfaces)
    {
        int surfacesInProgressCount = surfaces.Count;
        
        while (surfacesInProgressCount > 0)
        {
            for (int i = 0; i < surfaces.Count; i++)
            {
                if (!NavMeshAssetManager.instance.IsSurfaceBaking(surfaces[i]))
                    surfacesInProgressCount--;
            }
            
            yield return null;
        }
        
        yield return null;
        
        onBaked?.Invoke();
    }
}
#endif
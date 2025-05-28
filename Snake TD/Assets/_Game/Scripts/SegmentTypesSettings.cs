using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SegmentTypesSettings", menuName = "Scriptable Objects/SegmentTypesSettings")]
public class SegmentTypesSettings : ScriptableObject
{
    [SerializeField] private List<Material> materials = new List<Material>();
    private Dictionary<int, Material> materialsDictionary = new Dictionary<int, Material>();

    public List<Material> Materials => materials;
    public void Initialize()
    {
        materialsDictionary.Clear();

        for (int i = 0; i < materials.Count; i++)
        {
            materialsDictionary.Add(i, materials[i]);
        }

        Debug.Log($"FF->Settings Init");
    }
    public Material GetMaterial(int index)
    {

        Debug.Log($"FF->Key {index}");

        return materialsDictionary[index];
    }
    public Material GetRandomMaterial()
    {
        return materialsDictionary[Random.Range(0, materials.Count)];
    }
}

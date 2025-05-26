using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RigConstraintProperties
{
    [SerializeField] private string name;
    [Range(0f, 1f)]
    [SerializeField] private float weight;
    [SerializeField] private Vector3 offset;

    public string Name => name;
    public float Weight => weight;
    public Vector3 Offset => offset;
}

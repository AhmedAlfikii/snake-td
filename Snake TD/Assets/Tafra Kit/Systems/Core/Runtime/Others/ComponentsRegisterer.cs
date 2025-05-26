using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Narrative;

namespace TafraKit
{
    public class ComponentsRegisterer : MonoBehaviour
    {
        [SerializeField] private Component[] components;

        void Awake()
        {
            for (int i = 0; i < components.Length; i++)
            {
                ComponentProvider.RegisterComponent(components[i]);
            }
        }
    }
}
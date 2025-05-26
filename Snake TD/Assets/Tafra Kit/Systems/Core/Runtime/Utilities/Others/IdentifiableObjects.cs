using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    [System.Serializable]
    public class IdentifiableObjects<T>
    {
        [SerializeField] protected string id;
        [SerializeField] protected List<T> values;
        public string ID => id;
        public List<T> Values => values;
    }
}
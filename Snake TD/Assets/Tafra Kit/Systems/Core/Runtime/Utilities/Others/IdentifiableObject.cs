using UnityEngine;

namespace TafraKit
{
    [System.Serializable]
    public class IdentifiableObject<T>
    {
        [SerializeField] protected string id;
        [SerializeField] protected T value;
        public string ID => id;
        public T Value => value;
    }
}
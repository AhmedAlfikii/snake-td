using System;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    [Serializable]
    public class IntUnityEvent : UnityEvent<int> { }
    [Serializable]
    public class IntFloatUnityEvent : UnityEvent<int, float> { }
    [Serializable]
    public class FloatUnityEvent : UnityEvent<float> { }
    [Serializable]
    public class StringUnityEvent : UnityEvent<string> { }
    [Serializable]
    public class BoolUnityEvent : UnityEvent<bool> { }
    [Serializable]
    public class Vector3UnityEvent : UnityEvent<Vector3> { }
    [Serializable]
    public class Vector2UnityEvent : UnityEvent<Vector2> { }
    [Serializable]
    public class GameObjectUnityEvent : UnityEvent<GameObject> { }

    [Serializable]
    public class TafraAudioSourcePool : DynamicPool<TafraAudioSource> { };

    [Serializable]
    public class ColliderArrayIntEvent : UnityEvent<Collider[], int> { }
}

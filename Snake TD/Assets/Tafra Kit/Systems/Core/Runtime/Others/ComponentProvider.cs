using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TafraKit
{
    //TODO: Handling registering more than one component for the same game object.
    public static class ComponentProvider
    {
        private static Dictionary<Type, Dictionary<GameObject, Component>> typeDictionary = new Dictionary<Type, Dictionary<GameObject, Component>>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            typeDictionary.Clear();
        }

        public static void RegisterComponent<T>(Component comp)
        {
            if (comp == null)
            {
                TafraDebugger.Log("Component Provider", "The component you want to register is null.", TafraDebugger.LogType.Error);
                return;
            }

            Type type = typeof(T);

            if (typeDictionary.TryGetValue(type, out var gameObjectsDict))
            {
                bool added = gameObjectsDict.TryAdd(comp.gameObject, comp);

                if (!added)
                    TafraDebugger.Log("Component Provider", $"Couldn't register the component {comp} because it was registered before.", TafraDebugger.LogType.Error);
            }
            else
            {
                Dictionary<GameObject, Component> newDict = new Dictionary<GameObject, Component>
                {
                    { comp.gameObject, comp }
                };

                typeDictionary.Add(type, newDict);
            }
        }
        public static void RegisterComponent(Component comp)
        {
            if (comp == null)
            {
                TafraDebugger.Log("Component Provider", "The component you want to register is null.", TafraDebugger.LogType.Error);
                return;
            }

            Type type = comp.GetType();

            if (typeDictionary.TryGetValue(type, out var gameObjectsDict))
            {
                bool added = gameObjectsDict.TryAdd(comp.gameObject, comp);

                if (!added)
                    TafraDebugger.Log("Component Provider", $"Couldn't register the component {comp} because it was registered before.", TafraDebugger.LogType.Error);
            }
            else
            {
                Dictionary<GameObject, Component> newDict = new Dictionary<GameObject, Component>
                {
                    { comp.gameObject, comp }
                };

                typeDictionary.Add(type, newDict);
            }
        }
        public static void RegisterComponent(Component comp, Type type)
        {
            if (comp == null)
            {
                TafraDebugger.Log("Component Provider", "The component you want to register is null.", TafraDebugger.LogType.Error);
                return;
            }

            if (typeDictionary.TryGetValue(type, out var gameObjectsDict))
            {
                bool added = gameObjectsDict.TryAdd(comp.gameObject, comp);

                if (!added)
                    TafraDebugger.Log("Component Provider", $"Couldn't register the component {comp} because it was registered before.", TafraDebugger.LogType.Error);
            }
            else
            {
                Dictionary<GameObject, Component> newDict = new Dictionary<GameObject, Component>
                {
                    { comp.gameObject, comp }
                };

                typeDictionary.Add(type, newDict);
            }
        }
        public static bool UnregisterComponent(Component comp) 
        {
            if(comp == null)
            {
                TafraDebugger.Log("Component Provider", "The component you want to unregister is null.", TafraDebugger.LogType.Error);
                return false;
            }

            Type type = comp.GetType();

            if(typeDictionary.TryGetValue(type, out var gameObjectsDict))
                return gameObjectsDict.Remove(comp.gameObject);
       
            return false;
        }
        public static bool UnregisterComponent(Component comp, Type type) 
        {
            if(comp == null)
            {
                TafraDebugger.Log("Component Provider", "The component you want to unregister is null.", TafraDebugger.LogType.Error);
                return false;
            }

            if(typeDictionary.TryGetValue(type, out var gameObjectsDict))
                return gameObjectsDict.Remove(comp.gameObject);
       
            return false;
        }
        public static T GetComponent<T>(GameObject go) where T : Component
        {
            Type type = typeof(T);

            if(typeDictionary.TryGetValue(type, out var gameObjectsDict) && gameObjectsDict.TryGetValue(go, out Component component))
            {
                return component as T;
            }
            else
            {
                TafraDebugger.Log("Component Provider", $"There's no {typeof(T)} cached in {go.name}. Getting component in a normal way and will then cache it.", TafraDebugger.LogType.Warning);
                T result = go.GetComponent<T>();

                if (result != null)
                    RegisterComponent<T>(result);

                return result;
            }
        }
    }
}
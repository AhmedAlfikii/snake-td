using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.ModularSystem;
using UnityEngine;

namespace TafraKit
{
    public abstract class TafraActor : InternallyModularComponent<TafraActorModule>
    {
        [Header("Caching")]
        [Tooltip("A list of components that the actor would cache on awake in case other objects need to reference them. This eliminates the need for them to call GetComponent.")]
        [SerializeField] private Component[] cachedComponents;
        [Tooltip("A list of components that will be cached in the components provider.")]
        [SerializeField] private Component[] componentProviderCachedComponents;

        [Header("Modules")]
        [SerializeReferenceListContainer("modules", false, "Module", "Modules")]
        [SerializeField] private TafraActorModulesContainer modulesContainer;

        protected Dictionary<Type, Component> cachedComponentsByType;
        protected bool cachedComponentsFetched;
        protected InfluenceReceiver<int> layerChangingInfluences;
        protected bool initializedLayerChangingInfluences;
        private int defaultLayer;

        protected override List<TafraActorModule> InternalModules => modulesContainer.Modules;

        protected override void Awake()
        {
            base.Awake();

            if(!cachedComponentsFetched)
                CacheComponents();

            if(!initializedLayerChangingInfluences)
                InitializeLayerChangingInfluences();

            for (int i = 0; i < modulesCount; i++)
            {
                allModules[i].Initialize(this);
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
           
            for(int i = 0; i < componentProviderCachedComponents.Length; i++)
            {
                Component component = componentProviderCachedComponents[i];

                ComponentProvider.UnregisterComponent(component);
            }
        }
        private void InitializeLayerChangingInfluences()
        {
            defaultLayer = gameObject.layer;

            layerChangingInfluences = new InfluenceReceiver<int>(ShouldChangeLayer, OnActiveLayerInfluenceUpdated, null, OnAllLayerInfluencesCleared);

            initializedLayerChangingInfluences = true;
        }
        private bool ShouldChangeLayer(int newLayer, int oldLayer)
        {
            return true;
        }
        private void OnActiveLayerInfluenceUpdated(int layer)
        {
            gameObject.layer = layer;
        }
        private void OnAllLayerInfluencesCleared()
        {
            gameObject.layer = defaultLayer;
        }

        protected void CacheComponents()
        {
            cachedComponentsByType = new Dictionary<Type, Component>();

            for(int i = 0; i < cachedComponents.Length; i++)
            {
                Component component = cachedComponents[i];
                Type componentType = component.GetType();

                cachedComponentsByType.Add(componentType, component);

                Type baseType = componentType.BaseType;
                while(baseType != null && baseType != typeof(MonoBehaviour) && baseType != typeof(Component))
                {
                    cachedComponentsByType.Add(baseType, component);
                    baseType = baseType.BaseType;
                }
            }

            for (int i = 0; i < componentProviderCachedComponents.Length; i++)
            {
                Component component = componentProviderCachedComponents[i];

                ComponentProvider.RegisterComponent(component);
            }

            cachedComponentsFetched = true;
        }

        /// <summary>
        /// Attempts to get the required component from the cached components list. If it's not cached but exists in the game object, get it and cache it.
        /// </summary>
        /// <typeparam name="T">The type of the component you would like to get.</typeparam>
        /// <returns></returns>
        public T GetCachedComponent<T>() where T : Component
        {
            if(!cachedComponentsFetched)
                CacheComponents();

            if(cachedComponentsByType.TryGetValue(typeof(T), out var cachedComponent))
                return cachedComponent as T;

            TafraDebugger.LogType logType = TafraDebugger.LogType.Info;

            #if UNITY_EDITOR
            logType = TafraDebugger.LogType.Warning;
            #endif

            TafraDebugger.Log("Tafra Actor", $"Failed to get component {typeof(T)} from the cached components list. Will attempt a regular GetComponent and cache it.", logType);

            Component c = GetComponent<T>();

            if(c != null)
            {
                Type componentType = c.GetType();

                cachedComponentsByType.Add(componentType, c);

                Type baseType = componentType.BaseType;
                while(baseType != null && baseType != typeof(MonoBehaviour) && baseType != typeof(Component))
                {
                    cachedComponentsByType.Add(baseType, c);
                    baseType = baseType.BaseType;
                }
            }

            return c as T;
        }
        /// <summary>
        /// Change the layer of the actor's game object given a specific layer changer.
        /// </summary>
        /// <param name="layerChangerID"></param>
        /// <param name="layer"></param>
        public void AddLayerChanger(string layerChangerID, int layer)
        {
            if(!initializedLayerChangingInfluences)
                InitializeLayerChangingInfluences();

            layerChangingInfluences.AddInfluence(layerChangerID, layer);
        }
        /// <summary>
        /// No longer make this layer changer change the actor game object's layer.
        /// </summary>
        /// <param name="layerChangerID"></param>
        public void RemoveLayerChanger(string layerChangerID)
        {
            if(!initializedLayerChangingInfluences)
                InitializeLayerChangingInfluences();

            layerChangingInfluences.RemoveInfluence(layerChangerID);
        }
    }
}

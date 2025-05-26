using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TafraKit.ContentManagement;

namespace TafraKit.UI
{
    [CreateAssetMenu(fileName = "UI Skin", menuName = "Tafra Kit/UI/UI Skin")]
    public class UISkin : ScriptableObject
    {
        [SerializeField] private List<IdentifiableObject<TafraAsset<Sprite>>> sprites;
        [SerializeField] private List<IdentifiableObject<Color>> colors;
        [SerializeField] private List<IdentifiableObject<string>> texts;
        [SerializeField] private List<IdentifiableObject<TafraAsset<Material>>> materials;
        [SerializeField] private List<IdentifiableObject<bool>> gameObjectsActiveStat;

        [NonSerialized] private Dictionary<string, Sprite> loadedSpritesDict;
        [NonSerialized] private Dictionary<string, Color> loadedColorsDict;
        [NonSerialized] private Dictionary<string, string> loadedTextsDict;
        [NonSerialized] private Dictionary<string, Material> loadedMaterialsDict;
        [NonSerialized] private Dictionary<string, bool> loadedGameObjectsActiveStatDict;
        [NonSerialized] private bool isLoaded;
        [NonSerialized] private int loadersCount;

        private void LoadElements<T>(List<IdentifiableObject<TafraAsset<T>>> elements, ref Dictionary<string, T> loadedDictionary) where T : UnityEngine.Object
        {
            if(elements.Count > 0)
            {
                if(loadedDictionary == null)
                    loadedDictionary = new Dictionary<string, T>();
                else
                    loadedDictionary.Clear();

                for(int i = 0; i < elements.Count; i++)
                {
                    IdentifiableObject<TafraAsset<T>> element = elements[i];

                    T elementValue = element.Value.Load();

                    loadedDictionary.Add(element.ID, elementValue);
                }
            }
        }
        private void LoadElements<T>(List<IdentifiableObject<T>> elements, ref Dictionary<string, T> loadedDictionary)
        {
            if(elements.Count > 0)
            {
                if(loadedDictionary == null)
                    loadedDictionary = new Dictionary<string, T>();
                else
                    loadedDictionary.Clear();

                for(int i = 0; i < elements.Count; i++)
                {
                    IdentifiableObject<T> element = elements[i];

                    loadedDictionary.Add(element.ID, element.Value);
                }
            }
        }
        private void ReleaseElements<T>(List<IdentifiableObject<TafraAsset<T>>> elements, Dictionary<string, T> loadedDictionary) where T : UnityEngine.Object
        {
            if(elements.Count > 0)
            {
                if(loadedDictionary != null)
                    loadedDictionary.Clear();

                for(int i = 0; i < elements.Count; i++)
                {
                    IdentifiableObject<TafraAsset<T>> element = elements[i];
                    element.Value.Release();
                }
            }
        }
        private void ReleaseElements<T>(List<IdentifiableObject<T>> elements, Dictionary<string, T> loadedDictionary)
        {
            if(elements.Count > 0)
            {
                if(loadedDictionary != null)
                    loadedDictionary.Clear();
            }
        }

        public void Load()
        {
            loadersCount++;

            if (isLoaded)
                return;

            LoadElements(sprites, ref loadedSpritesDict);
            LoadElements(colors, ref loadedColorsDict);
            LoadElements(texts, ref loadedTextsDict);
            LoadElements(materials, ref loadedMaterialsDict);
            LoadElements(gameObjectsActiveStat, ref loadedGameObjectsActiveStatDict);

            isLoaded = true;
        }
        public void Release()
        {
            loadersCount--;

            if(loadersCount > 0)
                return;

            ReleaseElements(sprites, loadedSpritesDict);
            ReleaseElements(colors, loadedColorsDict);
            ReleaseElements(texts, loadedTextsDict);
            ReleaseElements(materials, loadedMaterialsDict);
            ReleaseElements(gameObjectsActiveStat, loadedGameObjectsActiveStatDict);

            isLoaded = false;
        }

        public bool TryGetSprite(string id, out Sprite sprite)
        {
            if(loadedSpritesDict == null)
            {
                sprite = null;
                return false;
            }

            return loadedSpritesDict.TryGetValue(id, out sprite);
        }
        public bool TryGetColor(string id, out Color color)
        {
            if(loadedColorsDict == null)
            {
                color = default;
                return false;
            }

            return loadedColorsDict.TryGetValue(id, out color);
        }
        public bool TryGetText(string id, out string text)
        {
            if(loadedTextsDict == null)
            {
                text = null;
                return false;
            }

            return loadedTextsDict.TryGetValue(id, out text);
        }
        public bool TryGetMaterial(string id, out Material material)
        {
            if(loadedMaterialsDict == null)
            {
                material = null;
                return false;
            }

            return loadedMaterialsDict.TryGetValue(id, out material);
        }
        public bool TryGetActiveState(string id, out bool state)
        {
            if(loadedGameObjectsActiveStatDict == null)
            {
                state = false;
                return false;
            }

            return loadedGameObjectsActiveStatDict.TryGetValue(id, out state);
        }
    }
}
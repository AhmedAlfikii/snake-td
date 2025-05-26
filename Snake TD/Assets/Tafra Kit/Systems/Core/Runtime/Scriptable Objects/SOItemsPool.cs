using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit
{
    /// <summary>
    /// A container Scriptable Object that will hold a list of objects, fetch you a random one upon request, but will not fetch the ones it already fetched again until all of its...
    /// ...objects have been fetched, it will then refill the list and restart. Fetched objects are saved/loaded to make sure the next time the game runs new objects will be fetched.
    /// </summary>
    public abstract class SOItemsPool<T> : ScriptableObject, IResettable
    {
        [Tooltip("The ID that will be used to save and load data. If empty, the Scriptable Object's name will be used instead.")]
        [SerializeField] protected string id;

        [NonSerialized] private bool isInitialized;
        [NonSerialized] protected string curId;
        [NonSerialized] private int usedItemsCount;
        [NonSerialized] private StringBuilder randomListUsedIndeciesSB = new StringBuilder();
        [NonSerialized] private List<int> randomListUnusedIndecies = new List<int>();
        [NonSerialized] private string usedItemsCountPrefsKey;
        [NonSerialized] private string randomListUsedIndeciesPrefsKey;

        protected abstract List<T> SequentialItems { get; }
        protected abstract List<T> RandomItems { get; }
        private int UsedItemsCount
        { 
            get => usedItemsCount;
            set 
            { 
                usedItemsCount = value;
                PlayerPrefs.SetInt(usedItemsCountPrefsKey, usedItemsCount);
            }
        }

        protected virtual void OnEnable()
        {
            #if UNITY_EDITOR
            if(!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            #endif

            if(!isInitialized)
                Initialize();
        }

        private void Initialize()
        {
            if(string.IsNullOrEmpty(id))
                curId = name;
            else
                curId = id;

            usedItemsCountPrefsKey = $"SO_ITEMS_POOL_{curId}_USED_ITEMS_COUNT";
            randomListUsedIndeciesPrefsKey = $"SO_ITEMS_POOL_{curId}_RANDOM_LIST_USED_INDECIES";

            usedItemsCount = PlayerPrefs.GetInt(usedItemsCountPrefsKey, 0);
            randomListUsedIndeciesSB.Append(PlayerPrefs.GetString(randomListUsedIndeciesPrefsKey));

            string[] indecies = randomListUsedIndeciesSB.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);

            int randomItemsCount = RandomItems.Count;
            for(int i = 0; i < randomItemsCount; i++)
            {
                randomListUnusedIndecies.Add(i);
            }

            for(int i = 0; i < indecies.Length; i++)
            {
                if(int.TryParse(indecies[i], out int itemIndex))
                    randomListUnusedIndecies.Remove(itemIndex);
            }

            isInitialized = true;
        }

        public T GetItem()
        {
            T item;

            if(usedItemsCount < SequentialItems.Count)
                item = SequentialItems[usedItemsCount];
            else
            {
                if (randomListUnusedIndecies.Count == 0)
                    return default;

                int randomIndex = UnityEngine.Random.Range(0, randomListUnusedIndecies.Count);
                int randomIndexValue = randomListUnusedIndecies[randomIndex];
                item = RandomItems[randomIndexValue];

                randomListUnusedIndecies.RemoveAt(randomIndex);
               
                //Refill the unused items if all the items have been used.
                if(randomListUnusedIndecies.Count == 0)
                {
                    randomListUsedIndeciesSB.Clear();

                    int randomItemsCount = RandomItems.Count;
                    for(int i = 0; i < randomItemsCount; i++)
                    {
                        //Make sure that the item that was just used right now doesn't get added to the unused list...
                        //...so that the next request doens't have a chance of getting the same item (if there's at least 1 more item).
                        if(i != randomIndexValue || randomItemsCount <= 1)
                            randomListUnusedIndecies.Add(i);
                        else
                        {
                            //Since the item that was just used is the only one in the new used list, we should save it to the player prefs.
                            randomListUsedIndeciesSB.Append(i);
                        }
                    }
                }
                else
                {
                    randomListUsedIndeciesSB.Append(',').Append(randomIndexValue);
                }

                PlayerPrefs.SetString(randomListUsedIndeciesPrefsKey, randomListUsedIndeciesSB.ToString());
            }

            UsedItemsCount++;
            return item;
        }

        public void ResetSavedData()
        {
            PlayerPrefs.DeleteKey(usedItemsCountPrefsKey);
            PlayerPrefs.DeleteKey(randomListUsedIndeciesPrefsKey);

            usedItemsCount = 0;
            randomListUsedIndeciesSB.Clear();

            int randomItemsCount = RandomItems.Count;
            for(int i = 0; i < randomItemsCount; i++)
            {
                randomListUnusedIndecies.Add(i);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public interface IInstanceableScriptableObject
    {
        public int InstanceNumber { get; set; }
        public string OriginalID { get; }
        public bool IsInstance { get; set; }
        public ScriptableObject OriginalScriptableObject { get; set; }

        /// <summary>
        /// Create a new instance object of this storable and assign an incremented instance number to it (unless loadedInstanceNumber is provided).
        /// </summary>
        /// <param name="loadedInstanceNumber">If this instance is meant to represent an already created before instance then assign that instance's number here.</param>
        /// <returns></returns>
        public ScriptableObject CreateInstance(int instanceNumber = -1)
        {
            ScriptableObject thisSO = null;

            if (IsInstance)
                thisSO = OriginalScriptableObject;
            else
                thisSO = this as ScriptableObject;

            ScriptableObject newInstance = ScriptableObject.Instantiate(thisSO);
            IInstanceableScriptableObject newInstancable = newInstance as IInstanceableScriptableObject;

            bool createNewInstance = instanceNumber < 0;

            int newInstanceNumber = 0;

            if(createNewInstance)
            {
                newInstanceNumber = PlayerPrefs.GetInt($"SO_INTANCES_COUNT_{OriginalID}", 0) + 1;
                PlayerPrefs.SetInt($"SO_INTANCES_COUNT_{OriginalID}", newInstanceNumber);
            }
            else
            {
                int savedInstancesCount = PlayerPrefs.GetInt($"SO_INTANCES_COUNT_{OriginalID}", 0);

                if (instanceNumber > savedInstancesCount)
                    PlayerPrefs.SetInt($"SO_INTANCES_COUNT_{OriginalID}", instanceNumber);
            }

            newInstance.name = thisSO.name;

            newInstancable.IsInstance = true;
            newInstancable.InstanceNumber = createNewInstance ? newInstanceNumber : instanceNumber;
            newInstancable.OriginalScriptableObject = IsInstance ? OriginalScriptableObject : thisSO;

            return newInstance;
        }
        public string GetSOInstanceID()
        {
            return OriginalID + "_" + InstanceNumber;
        }
        /// <summary>
        /// If this object is an instance, return it, otherwise create a new instance.
        /// </summary>
        /// <returns></returns>
        public ScriptableObject GetOrCreateInstance()
        {
            if(IsInstance)
                return this as ScriptableObject;
            else
                return CreateInstance();
        }
    }
}
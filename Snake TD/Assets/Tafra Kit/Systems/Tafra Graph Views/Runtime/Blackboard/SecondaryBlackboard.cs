using System;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal.GraphViews
{
    public class SecondaryBlackboard
    {
        private Dictionary<int, float> floatsByNameHash;
        private Dictionary<int, TafraAdvancedFloat> advancedFloatsByNameHash;
        private Dictionary<int, int> intByNameHash;
        private Dictionary<int, bool> boolByNameHash;
        private Dictionary<int, string> stringByNameHash;
        private Dictionary<int, Vector3> vector3ByNameHash;
        private Dictionary<int, GameObject> gameObjectByNameHash;
        private Dictionary<int, ScriptableObject> scriptableObjectByNameHash;
        private Dictionary<int, TafraActor> actorByNameHash;
        private Dictionary<int, UnityEngine.Object> objectByNameHash;
        private Dictionary<int, object> systemObjectByNameHash;
        private Dictionary<int, object> generalObjectByNameHash;

        #region Setters
        public void SetFloatProperty(int nameHash, float value)
        {
            if (floatsByNameHash == null)
                floatsByNameHash = new Dictionary<int, float>();

            if(floatsByNameHash.ContainsKey(nameHash))
                floatsByNameHash[nameHash] = value;
            else
                floatsByNameHash.Add(nameHash, value);
        }
        public void SetAdvancedFloatProperty(int nameHash, TafraAdvancedFloat value)
        {
            if(advancedFloatsByNameHash == null)
                advancedFloatsByNameHash = new Dictionary<int, TafraAdvancedFloat>();

            if(advancedFloatsByNameHash.ContainsKey(nameHash))
                advancedFloatsByNameHash[nameHash] = value;
            else
                advancedFloatsByNameHash.Add(nameHash, value);
        }
        public void SetIntProperty(int nameHash, int value)
        {
            if(intByNameHash == null)
                intByNameHash = new Dictionary<int, int>();

            if(intByNameHash.ContainsKey(nameHash))
                intByNameHash[nameHash] = value;
            else
                intByNameHash.Add(nameHash, value);
        }
        public void SetBoolProperty(int nameHash, bool value)
        {
            if(boolByNameHash == null)
                boolByNameHash = new Dictionary<int, bool>();

            if(boolByNameHash.ContainsKey(nameHash))
                boolByNameHash[nameHash] = value;
            else
                boolByNameHash.Add(nameHash, value);
        }
        public void SetStringProperty(int nameHash, string value)
        {
            if(stringByNameHash == null)
                stringByNameHash = new Dictionary<int, string>();

            if(stringByNameHash.ContainsKey(nameHash))
                stringByNameHash[nameHash] = value;
            else
                stringByNameHash.Add(nameHash, value);
        }
        public void SetVector3Property(int nameHash, Vector3 value)
        {
            if(vector3ByNameHash == null)
                vector3ByNameHash = new Dictionary<int, Vector3>();

            if(vector3ByNameHash.ContainsKey(nameHash))
                vector3ByNameHash[nameHash] = value;
            else
                vector3ByNameHash.Add(nameHash, value);
        }
        public void SetGameObjectProperty(int nameHash, GameObject value)
        {
            if(gameObjectByNameHash == null)
                gameObjectByNameHash = new Dictionary<int, GameObject>();

            if(gameObjectByNameHash.ContainsKey(nameHash))
                gameObjectByNameHash[nameHash] = value;
            else
                gameObjectByNameHash.Add(nameHash, value);
        }
        public void SetScriptableObjectProperty(int nameHash, ScriptableObject value)
        {
            if(scriptableObjectByNameHash == null)
                scriptableObjectByNameHash = new Dictionary<int, ScriptableObject>();

            if(scriptableObjectByNameHash.ContainsKey(nameHash))
                scriptableObjectByNameHash[nameHash] = value;
            else
                scriptableObjectByNameHash.Add(nameHash, value);
        }
        public void SetTafraActorProperty(int nameHash, TafraActor value)
        {
            if(actorByNameHash == null)
                actorByNameHash = new Dictionary<int, TafraActor>();

            if(actorByNameHash.ContainsKey(nameHash))
                actorByNameHash[nameHash] = value;
            else
                actorByNameHash.Add(nameHash, value);
        }
        public void SetObjectProperty(int nameHash, UnityEngine.Object value)
        {
            if(objectByNameHash == null)
                objectByNameHash = new Dictionary<int, UnityEngine.Object>();

            if(objectByNameHash.ContainsKey(nameHash))
                objectByNameHash[nameHash] = value;
            else
                objectByNameHash.Add(nameHash, value);
        }
        public void SetSystemObjectProperty(int nameHash, object value)
        {
            if(systemObjectByNameHash == null)
                systemObjectByNameHash = new Dictionary<int, object>();

            if(systemObjectByNameHash.ContainsKey(nameHash))
                systemObjectByNameHash[nameHash] = value;
            else
                systemObjectByNameHash.Add(nameHash, value);
        }
        public void SetGeneralObjectProperty<T>(int nameHash, T value)
        {
            if(generalObjectByNameHash == null)
                generalObjectByNameHash = new Dictionary<int, object>();

            if(generalObjectByNameHash.ContainsKey(nameHash))
                generalObjectByNameHash[nameHash] = value;
            else
                generalObjectByNameHash.Add(nameHash, value);
        }
        #endregion

        #region Getters
        public bool TryGetFloatProperty(int nameHash, out float value)
        {
            if(floatsByNameHash == null)
                floatsByNameHash = new Dictionary<int, float>();

            return floatsByNameHash.TryGetValue(nameHash, out value);
        }
        public bool TryGetAdvancedFloatProperty(int nameHash, out TafraAdvancedFloat value)
        {
            if(advancedFloatsByNameHash == null)
                advancedFloatsByNameHash = new Dictionary<int, TafraAdvancedFloat>();

            return advancedFloatsByNameHash.TryGetValue(nameHash, out value);
        }
        public bool TryGetIntProperty(int nameHash, out int value)
        {
            if(intByNameHash == null)
                intByNameHash = new Dictionary<int, int>();

            return intByNameHash.TryGetValue(nameHash, out value);
        }
        public bool TryGetBoolProperty(int nameHash, out bool value)
        {
            if(boolByNameHash == null)
                boolByNameHash = new Dictionary<int, bool>();

            return boolByNameHash.TryGetValue(nameHash, out value);
        }
        public bool TryGetStringProperty(int nameHash, out string value)
        {
            if(stringByNameHash == null)
                stringByNameHash = new Dictionary<int, string>();

            return stringByNameHash.TryGetValue(nameHash, out value);
        }
        public bool TryGetVector3Property(int nameHash, out Vector3 value)
        {
            if(vector3ByNameHash == null)
                vector3ByNameHash = new Dictionary<int, Vector3>();

            return vector3ByNameHash.TryGetValue(nameHash, out value);
        }
        public bool TryGetGameObjectProperty(int nameHash, out GameObject value)
        {
            if(gameObjectByNameHash == null)
                gameObjectByNameHash = new Dictionary<int, GameObject>();

            return gameObjectByNameHash.TryGetValue(nameHash, out value);
        }
        public bool TryGetScriptableObjectProperty(int nameHash, out ScriptableObject value)
        {
            if(scriptableObjectByNameHash == null)
                scriptableObjectByNameHash = new Dictionary<int, ScriptableObject>();

            return scriptableObjectByNameHash.TryGetValue(nameHash, out value);
        }
        public bool TryGetTafraActorProperty(int nameHash, out TafraActor value)
        {
            if(actorByNameHash == null)
                actorByNameHash = new Dictionary<int, TafraActor>();

            return actorByNameHash.TryGetValue(nameHash, out value);
        }
        public bool TryGetObjectProperty(int nameHash, out UnityEngine.Object value)
        {
            if(objectByNameHash == null)
                objectByNameHash = new Dictionary<int, UnityEngine.Object>();

            return objectByNameHash.TryGetValue(nameHash, out value);
        }
        public bool TryGetSystemObjectProperty(int nameHash, out object value)
        {
            if(systemObjectByNameHash == null)
                systemObjectByNameHash = new Dictionary<int, object>();

            return systemObjectByNameHash.TryGetValue(nameHash, out value);
        }
        public bool TryGetGeneralObjectProperty<T>(int nameHash, out object value)
        {
            if(generalObjectByNameHash == null)
                generalObjectByNameHash = new Dictionary<int, object>();

            return generalObjectByNameHash.TryGetValue(nameHash, out value);
        }
        #endregion
    }
}
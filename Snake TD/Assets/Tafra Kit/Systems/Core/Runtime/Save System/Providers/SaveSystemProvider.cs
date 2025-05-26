using UnityEngine;

namespace TafraKit.Internal
{
    public abstract class SaveSystemProvider
    {
        public abstract void SaveString(string key, string value);
        public abstract void SaveFloat(string key, float value);
        public abstract void SaveInt(string key, int value);
        public abstract void SaveBool(string key, bool value);
        public abstract void SaveVector2(string key, Vector2 value);
        public abstract void SaveVector2(string key, float x, float y);
        public abstract void SaveVector2Int(string key, Vector2Int value);
        public abstract void SaveVector2Int(string key, int x, int y);
        public abstract void SaveVector3(string key, Vector3 value);
        public abstract void SaveVector3(string key, float x, float y, float z);
        public abstract void SaveVector3Int(string key, Vector3Int value);
        public abstract void SaveVector3Int(string key, int x, int y, int z);

        public abstract string LoadString(string key, string defaultValue);
        public abstract float LoadFloat(string key, float defaultValue);
        public abstract int LoadInt(string key, int defaultValue);
        public abstract bool LoadBool(string key, bool defaultValue);
        public abstract Vector2 LoadVector2(string key, Vector2 defaultValue);
        public abstract Vector2 LoadVector2(string key, float defaultX, float defaultY);
        public abstract Vector2Int LoadVector2Int(string key, Vector2Int defaultValue);
        public abstract Vector2Int LoadVector2Int(string key, int defaultX, int defaultY);
        public abstract Vector3 LoadVector3(string key, Vector3 defaultValue);
        public abstract Vector3 LoadVector3(string key, float defaultX, float defaultY, float defaultZ);
        public abstract Vector3Int LoadVector3Int(string key, Vector3Int defaultValue);
        public abstract Vector3Int LoadVector3Int(string key, int defaultX, int defaultY, int defaultZ);

        public abstract bool HasKey(string key);

        public abstract void DeleteKey(string key);
        public abstract void DeleteAll();
    }
}
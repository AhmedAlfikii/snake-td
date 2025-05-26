using TafraKit.Internal;
using UnityEngine;

namespace TafraKit
{
    public static class TafraSaveSystem
    {
        private static SaveSystemProvider provider = new PlayerPrefsProvider();

        public static void SaveString(string key, string value)
        { 
            provider.SaveString(key, value);
        }
        public static void SaveFloat(string key, float value)
        { 
            provider.SaveFloat(key, value);
        }
        public static void SaveInt(string key, int value)
        {
            provider.SaveInt(key, value);
        }
        public static void SaveBool(string key, bool value)
        {
            provider.SaveBool(key, value);
        }
        public static void SaveVector2(string key, Vector2 value)
        {
            provider.SaveVector2(key, value);
        }
        public static void SaveVector2(string key, float x, float y)
        {
            provider.SaveVector2(key, x, y);
        }
        public static void SaveVector2Int(string key, Vector2Int value)
        {
            provider.SaveVector2Int(key, value);
        }
        public static void SaveVector2Int(string key, int x, int y)
        {
            provider.SaveVector2Int(key, x, y);
        }
        public static void SaveVector3(string key, Vector3 value)
        {
            provider.SaveVector3(key, value);
        }
        public static void SaveVector3(string key, float x, float y, float z)
        {
            provider.SaveVector3(key, x, y, z);
        }
        public static void SaveVector3Int(string key, Vector3Int value)
        {
            provider.SaveVector3Int(key, value);
        }
        public static void SaveVector3Int(string key, int x, int y, int z)
        {
            provider.SaveVector3Int(key, x, y, z);
        }

        public static string LoadString(string key, string defaultValue = "")
        {
            return provider.LoadString(key, defaultValue);
        }
        public static float LoadFloat(string key, float defaultValue = 0)
        {
            return provider.LoadFloat(key, defaultValue);
        }
        public static int LoadInt(string key, int defaultValue = 0)
        {
            return provider.LoadInt(key, defaultValue);
        }
        public static bool LoadBool(string key, bool defaultValue = false)
        {
            return provider.LoadBool(key, defaultValue);
        }
        public static Vector2 LoadVector2(string key, Vector2 defaultValue)
        {
            return provider.LoadVector2(key, defaultValue);
        }
        public static Vector2 LoadVector2(string key, float defaultX = 0, float defaultY = 0)
        {
            return provider.LoadVector2(key, defaultX, defaultY);
        }
        public static Vector2Int LoadVector2Int(string key, Vector2Int defaultValue)
        {
            return provider.LoadVector2Int(key, defaultValue);
        }
        public static Vector2Int LoadVector2Int(string key, int defaultX = 0, int defaultY = 0)
        {
            return provider.LoadVector2Int(key, defaultX, defaultY);
        }
        public static Vector3 LoadVector3(string key, Vector3 defaultValue)
        {
            return provider.LoadVector3(key, defaultValue);
        }
        public static Vector3 LoadVector3(string key, float defaultX = 0, float defaultY = 0, float defaultZ = 0)
        {
            return provider.LoadVector3(key, defaultX, defaultY, defaultZ);
        }
        public static Vector3Int LoadVector3Int(string key, Vector3Int defaultValue)
        {
            return provider.LoadVector3Int(key, defaultValue);
        }
        public static Vector3Int LoadVector3Int(string key, int defaultX = 0, int defaultY = 0, int defaultZ = 0)
        {
            return provider.LoadVector3Int(key, defaultX, defaultY, defaultZ);
        }

        public static bool HasKey(string key)
        {
            return provider.HasKey(key);
        }

        public static void DeleteKey(string key)
        {
            provider.DeleteKey(key);
        }
        public static void DeleteAll()
        {
            provider.DeleteAll();
        }
    }
}
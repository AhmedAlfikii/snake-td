using UnityEngine;

namespace TafraKit.Internal
{
    public class PlayerPrefsProvider : SaveSystemProvider
    {
        public override void SaveString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
        public override void SaveFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }
        public override void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
        public override void SaveBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }
        public override void SaveVector2(string key, Vector2 value)
        {
            PlayerPrefs.SetFloat($"{key}_TSS_X", value.x);
            PlayerPrefs.SetFloat($"{key}_TSS_Y", value.y);
        }
        public override void SaveVector2(string key, float x, float y)
        {
            PlayerPrefs.SetFloat($"{key}_TSS_X", x);
            PlayerPrefs.SetFloat($"{key}_TSS_Y", y);
        }
        public override void SaveVector2Int(string key, Vector2Int value)
        {
            PlayerPrefs.SetInt($"{key}_TSS_X", value.x);
            PlayerPrefs.SetInt($"{key}_TSS_Y", value.y);
        }
        public override void SaveVector2Int(string key, int x, int y)
        {
            PlayerPrefs.SetInt($"{key}_TSS_X", x);
            PlayerPrefs.SetInt($"{key}_TSS_Y", y);
        }
        public override void SaveVector3(string key, Vector3 value)
        {
            PlayerPrefs.SetFloat($"{key}_TSS_X", value.x);
            PlayerPrefs.SetFloat($"{key}_TSS_Y", value.y);
            PlayerPrefs.SetFloat($"{key}_TSS_Z", value.z);
        }
        public override void SaveVector3(string key, float x, float y, float z)
        {
            PlayerPrefs.SetFloat($"{key}_TSS_X", x);
            PlayerPrefs.SetFloat($"{key}_TSS_Y", y);
            PlayerPrefs.SetFloat($"{key}_TSS_Z", z);
        }
        public override void SaveVector3Int(string key, Vector3Int value)
        {
            PlayerPrefs.SetInt($"{key}_TSS_X", value.x);
            PlayerPrefs.SetInt($"{key}_TSS_Y", value.y);
            PlayerPrefs.SetInt($"{key}_TSS_Z", value.z);
        }
        public override void SaveVector3Int(string key, int x, int y, int z)
        {
            PlayerPrefs.SetInt($"{key}_TSS_X", x);
            PlayerPrefs.SetInt($"{key}_TSS_Y", y);
            PlayerPrefs.SetInt($"{key}_TSS_Z", z);
        }

        public override string LoadString(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
        public override float LoadFloat(string key, float defaultValue)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }
        public override int LoadInt(string key, int defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
        public override bool LoadBool(string key, bool defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }
        public override Vector2 LoadVector2(string key, Vector2 defaultValue)
        {
            Vector2 result = new Vector2();

            result.x = PlayerPrefs.GetFloat($"{key}_TSS_X", defaultValue.x);
            result.y = PlayerPrefs.GetFloat($"{key}_TSS_Y", defaultValue.y);

            return result;
        }
        public override Vector2 LoadVector2(string key, float defaultX, float defaultY)
        {
            Vector2 result = new Vector2();

            result.x = PlayerPrefs.GetFloat($"{key}_TSS_X", defaultX);
            result.y = PlayerPrefs.GetFloat($"{key}_TSS_Y", defaultY);

            return result;
        }
        public override Vector2Int LoadVector2Int(string key, Vector2Int defaultValue)
        {
            Vector2Int result = new Vector2Int();

            result.x = PlayerPrefs.GetInt($"{key}_TSS_X", defaultValue.x);
            result.y = PlayerPrefs.GetInt($"{key}_TSS_Y", defaultValue.y);

            return result;
        }
        public override Vector2Int LoadVector2Int(string key, int defaultX, int defaultY)
        {
            Vector2Int result = new Vector2Int();

            result.x = PlayerPrefs.GetInt($"{key}_TSS_X", defaultX);
            result.y = PlayerPrefs.GetInt($"{key}_TSS_Y", defaultY);

            return result;
        }
        public override Vector3 LoadVector3(string key, Vector3 defaultValue)
        {
            Vector3 result = new Vector3();

            result.x = PlayerPrefs.GetFloat($"{key}_TSS_X", defaultValue.x);
            result.y = PlayerPrefs.GetFloat($"{key}_TSS_Y", defaultValue.y);
            result.z = PlayerPrefs.GetFloat($"{key}_TSS_Z", defaultValue.z);

            return result;
        }
        public override Vector3 LoadVector3(string key, float defaultX, float defaultY, float defaultZ)
        {
            Vector3 result = new Vector3();

            result.x = PlayerPrefs.GetFloat($"{key}_TSS_X", defaultX);
            result.y = PlayerPrefs.GetFloat($"{key}_TSS_Y", defaultY);
            result.z = PlayerPrefs.GetFloat($"{key}_TSS_Z", defaultZ);

            return result;
        }
        public override Vector3Int LoadVector3Int(string key, Vector3Int defaultValue)
        {
            Vector3Int result = new Vector3Int();

            result.x = PlayerPrefs.GetInt($"{key}_TSS_X", defaultValue.x);
            result.y = PlayerPrefs.GetInt($"{key}_TSS_Y", defaultValue.y);
            result.z = PlayerPrefs.GetInt($"{key}_TSS_Z", defaultValue.z);

            return result;
        }
        public override Vector3Int LoadVector3Int(string key, int defaultX, int defaultY, int defaultZ)
        {
            Vector3Int result = new Vector3Int();

            result.x = PlayerPrefs.GetInt($"{key}_TSS_X", defaultX);
            result.y = PlayerPrefs.GetInt($"{key}_TSS_Y", defaultY);
            result.z = PlayerPrefs.GetInt($"{key}_TSS_Z", defaultZ);

            return result;
        }

        public override bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public override void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
        public override void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
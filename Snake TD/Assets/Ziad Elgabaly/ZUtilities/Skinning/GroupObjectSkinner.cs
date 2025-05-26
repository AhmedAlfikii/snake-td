using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Use this to skin multiple "ObjectSkinners" with the same skin, without having to load it on each one (which is the ObjectSkinner's default behaviour).
/// </summary>
public class GroupObjectSkinner : MonoBehaviour
{
    public UnityEvent OnSkinChanged;

    [Tooltip("The objkect skinners to skin all at once.")]
    [SerializeField] private ObjectSkinner[] objectSkinners;
    [SerializeField] private string objectID;
    [Tooltip("The path in the resources folder where the skins of this object exists")]
    [SerializeField] private string skinsPath;
    [SerializeField] private string defaultSkin;
    [SerializeField] private bool autoLoadSavedSkin;

    void Start()
    {
        if (autoLoadSavedSkin)
        {
            string savedSkin = PlayerPrefs.GetString(objectID + "_SKIN", defaultSkin);
            LoadSkin(savedSkin);
        }
    }

    IEnumerator LoadingSkin(string skinName)
    {
        GameObject curSkinGO = objectSkinners[0].CurrentSkin();

        if (curSkinGO != null && curSkinGO.name == skinName)
        {
            Debug.Log("No need to load the skin again.");
            yield break;
        }

        ResourceRequest rr = Resources.LoadAsync<GameObject>(skinsPath + "/" + skinName);

        while (!rr.isDone)
        {
            yield return null;
        }

        GameObject skinPrefab = rr.asset as GameObject;

        if (skinPrefab == null)
        {
            Debug.Log("Couldn't load the skin.");
            yield break;
        }

        for (int i = 0; i < objectSkinners.Length; i++)
        {
            objectSkinners[i].ApplySkin(skinPrefab);
        }

        OnSkinChanged?.Invoke();
    }

    public void LoadSkin(string skinName)
    {
        StartCoroutine(LoadingSkin(skinName));
    }

    public GameObject CurrentSkin()
    {
        return objectSkinners[0].CurrentSkin();
    }
}

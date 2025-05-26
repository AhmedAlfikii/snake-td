using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectSkinner : MonoBehaviour
{
    #region Public Events
    public UnityEvent OnSkinChanged;
    #endregion

    #region Private Serialized Fields
    [Tooltip("If this object is controlled by a group (GroupObjectSkinner), it won't do anything itself.")]
    [SerializeField] private bool controlledByGroup;
    [Tooltip("The ID that will be used to save the object's skin.")]
    [SerializeField] private string objectID;
    [Tooltip("The path in the resources folder where the skins of this object exists")]
    [SerializeField] private string skinsPath;
    [SerializeField] private string defaultSkin;
    [Tooltip("The transform that the skin will be instantiated inside. If empty, it will be instantiated inside the current transform.")]
    [SerializeField] private Transform skinHolder;
    [Tooltip("If a default skin is already under the skin holder, make sure to put it here so that it gets replaced when a new skin is loaded.")]
    [SerializeField] private GameObject curSkinGO;
    [Tooltip("Should the last applied or default skin be loaded on start?")]
    [SerializeField] private bool autoLoadSavedSkin;
    [Tooltip("If true, the skin will be loaded immediately without the async operation.")]
    [SerializeField] private bool autoLoadSavedSkinImmediately;
    #endregion

    #region MonoBehaviour Messages
    void Start()
    {
        if (!controlledByGroup && autoLoadSavedSkin)
        {
            string savedSkin = PlayerPrefs.GetString(objectID + "_SKIN", defaultSkin);
            LoadSkin(savedSkin);
        }
    }
    #endregion

    #region Private Functions
    IEnumerator LoadingSkin(string skinName)
    {
        if (curSkinGO != null && curSkinGO.name == skinName)
        {
            Debug.Log("No need to load the skin again.");
            yield break;
        }

        ResourceRequest rr = Resources.LoadAsync<GameObject>(skinsPath  + "/" + skinName);

        while (!rr.isDone)
        {
            yield return null;
        }

        ApplySkin(rr.asset as GameObject);
    }
    #endregion

    #region Public Functions
    /// <summary>
    /// Loads a skin from the object's skin path under resources folder with the given name.
    /// </summary>
    /// <param name="skinName">The name of the skin prefab under the objecet's skin path.</param>
    /// <param name="immediate">Should the skin be loaded immediately without the async operation?</param>
    public void LoadSkin(string skinName, bool immediate = false)
    {
        if (!immediate)
            StartCoroutine(LoadingSkin(skinName));
        else
        {
            GameObject prefab = Resources.Load<GameObject>(skinsPath + "/" + skinName);
            ApplySkin(prefab);
        }
    }

    /// <summary>
    /// Applies an already loaded skin, you most likely will not be directly using this.
    /// </summary>
    /// <param name="skinPrefab"></param>
    public void ApplySkin(GameObject skinPrefab)
    {
        if (curSkinGO)
            Destroy(curSkinGO);

        GameObject skinGO = Instantiate(skinPrefab, skinHolder == null? transform : skinHolder);
        skinGO.name = skinGO.name.Substring(0, skinGO.name.Length - 7);

        curSkinGO = skinGO;

        OnSkinChanged.Invoke();
    }

    public void ClearSkin()
    {
        if (curSkinGO)
        {
            Destroy(curSkinGO);
            curSkinGO = null;
        }
    }

    public GameObject CurrentSkin()
    {
        return curSkinGO;
    }
    #endregion
}

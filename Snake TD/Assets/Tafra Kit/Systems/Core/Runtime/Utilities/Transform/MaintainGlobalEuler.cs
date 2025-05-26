using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;

[ExecuteAlways]
public class MaintainGlobalEuler : MonoBehaviour
{
    [SerializeField] private Vector3 globalEuler;
    [SerializeField] private UnityMessagesBool updateAt;

    private void Awake()
    {
        if(updateAt.Awake)
            UpdateEuler();
    }
    private void Start()
    {
        if(updateAt.Start)
            UpdateEuler();
    }
    private void Update()
    {
        if(updateAt.Update)
            UpdateEuler();
    }
    private void LateUpdate()
    {
        if(updateAt.LateUpdate)
            UpdateEuler();

        #if UNITY_EDITOR
        if(!Application.isPlaying)
            UpdateEuler();
        #endif
    }
    private void FixedUpdate()
    {
        if(updateAt.FixedUpdate)
            UpdateEuler();
    }
    private void Reset()
    {
        globalEuler = transform.eulerAngles;
    }

    void UpdateEuler()
    {
        transform.eulerAngles = globalEuler;
    }
}

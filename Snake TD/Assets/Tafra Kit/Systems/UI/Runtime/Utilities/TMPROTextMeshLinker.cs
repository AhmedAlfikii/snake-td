using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TafraKit.UI
{
    [ExecuteAlways]
    public class TMPROTextMeshLinker : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI main;

        private TextMeshProUGUI myTM;
        private void Awake()
        {
            myTM = GetComponent<TextMeshProUGUI>();
        }
        private void OnEnable()
        {
            if(myTM == null || main == null)
                return;

            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChange);
        }
        private void OnDisable()
        {
            if(myTM == null || main == null)
                return;

            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChange);

        }

        #if UNITY_EDITOR
        private void Update()
        {
            if(myTM == null || main == null)
                return;

            myTM.text = main.text;
        }
        #endif

        void OnTextChange(Object obj)
        {
            if(obj != main)
                return;

            StartCoroutine(UpdateTextEnum());
        }
        IEnumerator UpdateTextEnum()
        {
            yield return Yielders.EndOfFrame;

            myTM.text = main.text;
        }
    }
}
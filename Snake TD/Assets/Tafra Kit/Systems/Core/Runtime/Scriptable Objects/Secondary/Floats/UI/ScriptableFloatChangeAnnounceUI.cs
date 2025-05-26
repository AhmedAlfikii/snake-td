using System;
using System.Collections;
using TafraKit.MotionFactory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit
{
    public class ScriptableFloatChangeAnnounceUI : MonoBehaviour
    {
        [SerializeField] private ScriptableFloat scriptableFloat;

        [Header("UI")]
        [SerializeField] private VisibilityMotionController motionController;
        [SerializeField] private TextMeshProUGUI valueTXT;
        [SerializeField] private Image arrow;

        [Header("Properties")]
        [SerializeField] private Color positiveColor = Color.green;
        [SerializeField] private Color negativeColor = Color.red;

        private float currentValue;
        private IEnumerator animationEnum;
        private bool listen;
        
        private void Awake()
        {
            currentValue = scriptableFloat.Value;
        }
        private IEnumerator Start()
        {
            yield return null;
            listen = true;
        }
        private void OnEnable()
        {
            currentValue = scriptableFloat.Value;

            scriptableFloat.OnValueChange.AddListener(OnValueChange);
        }
        private void OnDisable()
        {
            scriptableFloat.OnValueChange.RemoveListener(OnValueChange);
        }

        private void OnValueChange(float newValue)
        {
            if(!listen)
            {
                currentValue = newValue;

                return;
            }

            float diff = newValue - currentValue;
            
            if(diff == 0)
                return;

            if(diff > 0)
            {
                valueTXT.color = positiveColor;
                arrow.color = positiveColor;
            }
            else
            {
                valueTXT.color = negativeColor;
                arrow.color = negativeColor;
            }

            valueTXT.text = "0";

            if(animationEnum != null)
                StopCoroutine(animationEnum);

            animationEnum = Animating(diff);

            StartCoroutine(animationEnum);

            currentValue = newValue;
        }

        private IEnumerator Animating(float diff)
        {
            motionController.Show();

            yield return Yielders.GetWaitForSecondsRealtime(0.1f);

            float startTime = Time.unscaledTime;
            float duration = 0.5f;
            float endTime = startTime + duration;

            while(Time.unscaledTime <  endTime)
            {
                float t = (Time.unscaledTime - startTime) / duration;

                int val = Mathf.RoundToInt(diff * t);

                valueTXT.text = val.ToString();

                yield return null;
            }

            yield return Yielders.GetWaitForSecondsRealtime(1f);

            motionController.Hide();
        }
    }
}
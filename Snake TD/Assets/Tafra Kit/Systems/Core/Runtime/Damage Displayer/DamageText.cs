using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TafraKit
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMesh;
        
        [Header("Animation")]
        [SerializeField] private float riseDuration = 1f;
        [Tooltip("The height the damage text will move to in comparison to where the animation started.")]
        [SerializeField] private float rise = 3f;
        [SerializeField] private AnimationCurve scaleCurve;

        [Header("Effects")]
        [SerializeField] private SFXClips SFX;

        private void OnEnable()
        {
            StartCoroutine(Animation());

            if(SFX != null)
                SFXPlayer.Play(SFX);
        }

        private IEnumerator Animation()
        { 
            float startTime = Time.time;
            float endTime = startTime + riseDuration;

            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + new Vector3(0, rise, 0);
            
            while(Time.time < endTime)
            {
                float t = (Time.time - startTime) / riseDuration;

                transform.position = Vector3.LerpUnclamped(startPosition, targetPosition, t);

                float scaleT = scaleCurve.Evaluate(t);

                transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, scaleT);

                yield return null;
            }
            transform.position = targetPosition;
            transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, 1);

            gameObject.SetActive(false);
        }

        public void SetText(string damageText)
        {
            textMesh.text = damageText;
        }
    }
}
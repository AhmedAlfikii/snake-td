using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TafraKit;

namespace ZCasualGameKit
{
    public class Typewriter : MonoBehaviour
    {
        [TextArea]
        [SerializeField] private string textToType;

        [Header("Settings")]
        [SerializeField] private bool playOnStart = true;
        [Tooltip("The duration to wait after taking the order and actually starting to type.")]
        [SerializeField] private float startAfter;
        [SerializeField] private float durationBetweenLetters = 0.025f;
        [Tooltip("The sound effect to play when a letter is typed.")]
        [SerializeField] private AudioClip clickSFX;

        private TextMesh textMesh;
        private TextMeshProUGUI tmproText;
        private bool normalTextMesh = true;
        private StringBuilder curText;
        private IEnumerator typingEnum;

        void Start()
        {
            textMesh = GetComponent<TextMesh>();
            if (!textMesh)
            {
                normalTextMesh = false;
                tmproText = GetComponent<TextMeshProUGUI>();
            }

            if (playOnStart)
                StartTyping();
        }

        public void StartTyping()
        {
            if (typingEnum != null)
                StopCoroutine(typingEnum);

            typingEnum = Typing();
            StartCoroutine(typingEnum);
        }

        IEnumerator Typing()
        {
            curText.Clear();

            yield return Yielders.GetWaitForSeconds(startAfter);
            int lettersCount = textToType.Length;

            for (int i = 0; i < lettersCount; i++)
            {
                curText.Append(textToType[i]);
                if (normalTextMesh)
                    textMesh.text = curText.ToString();
                else
                    tmproText.text = curText.ToString();

                SFXPlayer.Play(clickSFX);

                yield return Yielders.GetWaitForSeconds(durationBetweenLetters);
            }
        }
    }
}
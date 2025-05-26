using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TafraKit.Internal.UI
{
    public class FleetingMessage : MonoBehaviour
    {
        [SerializeField] private TMP_Text messageTXT;
        [SerializeField] private CanvasGroup canvasGroup;

        public CanvasGroup CanvasGroup => canvasGroup;

        public void SetText(string message)
        {
            messageTXT.text = message;
        }
    }
}
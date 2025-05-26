using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TafraKit
{
    public class UITextCopier : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public void CopyToClipboard()
        {
            text.text.CopyToClipboard();
        }
    }
}
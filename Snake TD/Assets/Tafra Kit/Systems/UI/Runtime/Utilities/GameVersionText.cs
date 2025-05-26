using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TafraKit.Internal
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class GameVersionText : MonoBehaviour
    {
        [SerializeField] private string prefix = "V";
        private TextMeshProUGUI textMesh;

        private void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
            textMesh.text = $"{prefix}{Application.version}";
        }
    }
}
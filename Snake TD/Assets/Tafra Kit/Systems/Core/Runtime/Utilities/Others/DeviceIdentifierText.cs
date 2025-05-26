using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TafraKit
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DeviceIdentifierText : MonoBehaviour
    {
        void Start()
        {
            TextMeshProUGUI txt = GetComponent<TextMeshProUGUI>();

            txt.text = SystemInfo.deviceUniqueIdentifier;
        }
    }
}
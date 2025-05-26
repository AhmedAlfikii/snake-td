using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TafraKitEditor
{
    public class MenuItems
    {
        [MenuItem("Tafra Games/Documentation", priority = 100)]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://docs.google.com/document/d/1dps5bqJb2gS2XkYy2Jr9yCeTIv5jxM6zFPON3k6eRPg/");
        }
    }
}
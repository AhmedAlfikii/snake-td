using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor
{
    public class TafraTextureImporterWindow : EditorWindow
    {
        private static bool autoConvertUI;

        public static bool AutoConvertUI => autoConvertUI;

        [MenuItem("Tafra Games/Windows/Importers/Texture Importer")]
        private static void Open()
        {
            GetWindow<TafraTextureImporterWindow>("Tafra Texture Importer");
        }

        private void CreateGUI()
        {
            rootVisualElement.style.paddingTop = rootVisualElement.style.paddingBottom = rootVisualElement.style.paddingRight = rootVisualElement.style.paddingLeft = 2;

            #region UI Section
            Label uiHeader = new Label("UI");
            uiHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            uiHeader.style.fontSize = 14;
            uiHeader.style.marginTop = 5;
            uiHeader.style.marginBottom = 5;

            Toggle autoConvertUIToggle = new Toggle("Auto Convert UI Assets");
            autoConvertUIToggle.tooltip = "If true, all textures imported in a folder that is called \"UI\" or has a parent that is called \"UI\" will be converted to \"(Sprite (2D and UI)\".";
            autoConvertUIToggle.value = EditorPrefs.GetBool("TAFRA_TEXTURE_IMPORTER_AUTO_CONVERT_AUI", false);
            autoConvertUI = autoConvertUIToggle.value;
            autoConvertUIToggle.RegisterValueChangedCallback((ev) =>
            {
                EditorPrefs.SetBool("TAFRA_TEXTURE_IMPORTER_AUTO_CONVERT_AUI", ev.newValue);
                autoConvertUI = ev.newValue;
            });
            #endregion

            rootVisualElement.Add(uiHeader);
            rootVisualElement.Add(autoConvertUIToggle);
        }
    }
}
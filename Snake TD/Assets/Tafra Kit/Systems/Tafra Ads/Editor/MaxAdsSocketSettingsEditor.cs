#if HAS_LION_APPLOVIN_SDK
using TafraKitEditor;
using UnityEditor;

namespace TafraKit
{
    [CustomEditor(typeof(MaxAdsSocketSettings))]

    public class MaxAdsSocketSettingsEditor : SettingsModuleEditor
    {
        private SerializedProperty enableAmazonAds;

        private string APSDefineSymbol = "TAFRA_AMAZON_ADS";

        private void OnEnable()
        {
            enableAmazonAds = serializedObject.FindProperty("apsEnabled");

            if (enableAmazonAds.boolValue)
            {
                TafraEditorUtility.AddDefiningSymbols(APSDefineSymbol, BuildTargetGroup.Android);
                TafraEditorUtility.AddDefiningSymbols(APSDefineSymbol, BuildTargetGroup.iOS);
            }
            else
            {
                TafraEditorUtility.RemoveDefiningSymbols(APSDefineSymbol, BuildTargetGroup.Android);
                TafraEditorUtility.RemoveDefiningSymbols(APSDefineSymbol, BuildTargetGroup.iOS);
            }
        }

        public override void OnFocus()
        {
            if (enableAmazonAds.boolValue)
            {
                TafraEditorUtility.AddDefiningSymbols(APSDefineSymbol, BuildTargetGroup.Android);
                TafraEditorUtility.AddDefiningSymbols(APSDefineSymbol, BuildTargetGroup.iOS);
            }
            else
            {
                TafraEditorUtility.RemoveDefiningSymbols(APSDefineSymbol, BuildTargetGroup.Android);
                TafraEditorUtility.RemoveDefiningSymbols(APSDefineSymbol, BuildTargetGroup.iOS);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
#endif
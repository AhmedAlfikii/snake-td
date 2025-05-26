using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TafraKit.ContentManagement;

namespace TafraKitEditor.ContentManagement
{
    public class TafraAssetField : VisualElement
    {
        private SerializedProperty property;
        private SerializedProperty assetTypeProperty;

        private PropertyBox propertyBox;
        private PropertyField assetTypeField;

        public TafraAssetField(SerializedProperty property, string displayName, string tooltip = "")
        {
            this.property = property;

            propertyBox = new PropertyBox(property, null, "", tooltip, true);
            Add(propertyBox);

            this.RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            this.RegisterCallback<DetachFromPanelEvent>(OnDetachedToPanel);

            DrawContent();
        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            assetTypeField.RegisterCallback<SerializedPropertyChangeEvent>(AssetTypeChanged);
        }
        private void OnDetachedToPanel(DetachFromPanelEvent evt)
        {
            assetTypeField.UnregisterCallback<SerializedPropertyChangeEvent>(AssetTypeChanged);
        }
        private void AssetTypeChanged(SerializedPropertyChangeEvent evt)
        {
            assetTypeField.UnregisterCallback<SerializedPropertyChangeEvent>(AssetTypeChanged);

            AssetType selectedFloatType = (AssetType)assetTypeProperty.enumValueIndex;
           
            switch(selectedFloatType)
            {
                case AssetType.Direct:
                    //SerializedProperty addressablesAssetProperty = property.FindPropertyRelative("addressablesAsset");
                    //addressablesAssetProperty.boxedValue = default;
                    break;
                case AssetType.Addressables:
                    SerializedProperty directAssetProperty = property.FindPropertyRelative("directAsset");
                    directAssetProperty.objectReferenceValue = null;
                    break;
                default:
                    break;
            }
            property.serializedObject.ApplyModifiedProperties();

            DrawContent();

            assetTypeField.schedule.Execute(() =>
            {
                assetTypeField.RegisterCallback<SerializedPropertyChangeEvent>(AssetTypeChanged);
            });
        }

        private void DrawContent()
        {
            propertyBox.Content.Clear();

            VisualElement contentRow = new VisualElement();
            contentRow.name = "content-row";
            contentRow.style.flexDirection = FlexDirection.Row;
            propertyBox.Content.Add(contentRow);

            assetTypeProperty = property.FindPropertyRelative("assetType");
            assetTypeField = new PropertyField(assetTypeProperty, "");
            assetTypeField.style.marginRight = 5;
            assetTypeField.style.flexShrink = 0;

            AssetType selectedFloatType = (AssetType)assetTypeProperty.enumValueIndex;

            switch(selectedFloatType)
            {
                case AssetType.Direct:
                    PropertyField directAssetField = new PropertyField(property.FindPropertyRelative("directAsset"), property.displayName);
                    directAssetField.style.flexGrow = 1;
                    contentRow.Add(directAssetField);
                    propertyBox.MainContainer.style.backgroundColor = new Color(0, 0, 0, 0.025f);
                    break;
                case AssetType.Addressables:
                    #if TAFRA_ADDRESSABLES
                    PropertyField addressablesAssetField = new PropertyField(property.FindPropertyRelative("addressablesAsset"), property.displayName);
                    addressablesAssetField.style.flexGrow = 1;
                    contentRow.Add(addressablesAssetField);
                    #else
                    Label info = new Label("Addressables package isn't in the project.");
                    info.style.flexGrow = 1;
                    contentRow.Add(info);
                    #endif

                    propertyBox.MainContainer.style.backgroundColor = new Color(1f, 0, 1f, 0.025f);
                    break;
                default:
                    break;
            }

            contentRow.Add(assetTypeField);

            this.Bind(property.serializedObject);
        }
    }
}
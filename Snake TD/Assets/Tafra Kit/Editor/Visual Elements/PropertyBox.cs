using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using System;
using TafraKit;

namespace TafraKitEditor
{
    public class PropertyBox : VisualElement
    {
        private SerializedProperty property;
        private VisualElement mainContainer;
        private VisualElement headerRow;
        private VisualElement content;

        /// <summary>
        /// The main container of the property box..
        /// </summary>
        public VisualElement MainContainer => mainContainer;
        /// <summary>
        /// The header of the property box.
        /// </summary>
        public VisualElement Header => headerRow;
        /// <summary>
        /// The content of the property box.
        /// </summary>
        public VisualElement Content => content;

        /// <summary>
        /// Draw a property box.
        /// </summary>
        /// <param name="property">The property to draw inside the box.</param>
        /// <param name="title">The title to display on the box.</param>
        /// <param name="subtitle">The subtitle to display below the main title. Send an empty string if you don't want a subtitle.</param>
        /// <param name="tooltip">The tooltip that will be displayed when the user hovers over the header.</param>
        /// <param name="dontDrawContent">Prevent the box from automatically drawing the content of the property (this means you'll be drawing it by yourself).</param>
        /// <param name="drawPropertyInContent">If dontDrawContent is disabled. Should the content contain the property itself? 
        /// If false, the box will go through each of the property's children and draw them one by one instead of drawing the property as a whole.</param>
        public PropertyBox(SerializedProperty property, string title, string subtitle, string tooltip, bool dontDrawContent = false, bool drawPropertyInContent = false)
        {
            DrawBase(property, title, subtitle, tooltip);

            if(!dontDrawContent)
            {
                if(!drawPropertyInContent)
                {
                    var childProperties = property.GetVisibleChildren();
                    foreach(var child in childProperties)
                    {
                        PropertyField childPF = new PropertyField(child);
                        content.Add(childPF);
                    }
                }
                else
                {
                    PropertyField propertyPF = new PropertyField(property);
                    content.Add(propertyPF);
                }
            }
        }

        private void DrawBase(SerializedProperty property, string title, string subtitle, string tooltip)
        {
            this.property = property;

            mainContainer = new VisualElement();
            mainContainer.name = "main-container";
            mainContainer.style.backgroundColor = new Color(0, 0, 0, 0.025f);
            mainContainer.style.SetBorderColor(new Color(0, 0, 0, 0.3f));
            mainContainer.style.SetBorderWidth(1);
            mainContainer.style.SetBorderRadius(5);
            mainContainer.style.marginTop = 5;
            mainContainer.style.marginBottom = 5;
            mainContainer.style.SetPadding(5);
            Add(mainContainer);

            if (title != null)
            {
                headerRow = new VisualElement();
                headerRow.name = "header-row";
                headerRow.tooltip = tooltip;
                headerRow.style.flexDirection = FlexDirection.Row;
                headerRow.style.justifyContent = Justify.SpaceBetween;
                headerRow.style.marginBottom = 2;
                mainContainer.Add(headerRow);

                Label titleLabel = new Label(title);
                titleLabel.name = "title";
                titleLabel.style.fontSize = 12;
                titleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                headerRow.Add(titleLabel);

                titleLabel.AddManipulator(new ContextualMenuManipulator((ev) =>
                {

                    TafraEditorUtility.BuildPropertyContextualMenu(property, ev);
                    return;
                    //TODO: take all of these menu items and add them to a centeralized function where they can be used on any custom property field.

                    #region Misc. Items
                    ev.menu.AppendAction("Copy Property Path", (a) =>
                    {
                        property.propertyPath.CopyToClipboard();
                    });
                    #endregion

                    #region Prefab Items
                    GameObject targetGameObject = property.serializedObject.targetObject.GameObject();
                    bool propertyIsModified = false;

                    if (targetGameObject != null)
                    {
                        var modifiedProperties = PrefabUtility.GetPropertyModifications(targetGameObject);

                        for (int i = 0; i < modifiedProperties.Length; i++)
                        {
                            var modifiedProp = modifiedProperties[i];
                            if (modifiedProp.propertyPath.Contains(property.propertyPath))
                            {
                                propertyIsModified = true;
                                break;
                            }
                        }

                        if (propertyIsModified)
                        {
                            List<GameObject> prefabsList = new List<GameObject>();

                            TafraEditorUtility.GetAllPrefabInstanceParents(property.serializedObject.targetObject.GameObject(), prefabsList);

                            for (int i = 0; i < prefabsList.Count; i++)
                            {
                                GameObject prefab = prefabsList[i];

                                string title = "";

                                if (i < prefabsList.Count - 1)  //If this isn't the root parent.
                                    title = $"Apply as Override in Prefab '{prefab.name}'";
                                else //If this is the root parent.
                                    title = $"Apply to Prefab '{prefab.name}'";

                                ev.menu.AppendAction(title, (a) =>
                                {
                                    PrefabUtility.ApplyPropertyOverride(property, AssetDatabase.GetAssetPath(prefab), InteractionMode.UserAction);
                                    property.serializedObject.ApplyModifiedProperties();
                                });
                            }
                            ev.menu.AppendAction("Revert", (a) =>
                            {
                                PrefabUtility.RevertPropertyOverride(property, InteractionMode.UserAction);
                                property.serializedObject.ApplyModifiedProperties();
                            });
                        }
                    }
                    #endregion

                    #region Copy & Paste
                    ev.menu.AppendSeparator();

                    bool canPaste = TafraEditorUtility.CanPasteClipboardObjectOnObject(property.boxedValue);

                    ev.menu.AppendAction("Copy", (a) =>
                    {
                        property.serializedObject.Update();
                        TafraEditorUtility.AddObjectToClipboard(property.boxedValue);
                    });
                    ev.menu.AppendAction("Paste", (a) =>
                    {
                        object clipboardCopy = TafraEditorUtility.GetClipboardObjectCopy();
                        if (clipboardCopy != null)
                        {
                            property.boxedValue = clipboardCopy;
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    }, canPaste ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                    #endregion
                }));
            }

            if (!string.IsNullOrEmpty(subtitle))
            {
                Label subtitleLabel = new Label(subtitle);
                subtitleLabel.name = "subtitle";
                subtitleLabel.style.marginLeft = 4;
                subtitleLabel.style.marginTop = 2;
                subtitleLabel.style.marginBottom = 5;
                subtitleLabel.style.color = new Color(1, 1, 1, 0.35f);
                subtitleLabel.style.SetPadding(0);
                subtitleLabel.style.fontSize = 11;
                mainContainer.Add(subtitleLabel);
            }

            content = new VisualElement();
            content.name = "content";
            mainContainer.Add(content);
        }
    }
}
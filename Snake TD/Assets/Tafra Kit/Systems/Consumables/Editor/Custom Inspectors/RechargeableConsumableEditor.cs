using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit.Consumables;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TafraKitEditor.ResourcesSystem
{
    [CustomEditor(typeof(RechargeableConsumable))]
    public class RechargeableConsumableEditor : ScriptableFloatEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            root.name = "root";

            Label header = new Label("Rechargeable Consumable");
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.marginTop = 5;

            PropertyField displayNamesPF = new PropertyField(serializedObject.FindProperty("displayNames"));
            PropertyField iconsPF = new PropertyField(serializedObject.FindProperty("icons"));
            PropertyField colorsPF = new PropertyField(serializedObject.FindProperty("colors"));
            PropertyField descriptionPF = new PropertyField(serializedObject.FindProperty("description"));
            PropertyField rechargeDurationPF = new PropertyField(serializedObject.FindProperty("rechargeDuration"));
            PropertyField autoRechargeCapPF = new PropertyField(serializedObject.FindProperty("autoRechargeCap"));

            IMGUIContainer sfContainer = new IMGUIContainer(() => {
                serializedObject.Update();
                BuildSFUI();
                serializedObject.ApplyModifiedProperties();
            });

            root.Add(header);

            root.Add(displayNamesPF);
            root.Add(iconsPF);
            root.Add(colorsPF);
            root.Add(descriptionPF);
            root.Add(rechargeDurationPF);
            root.Add(autoRechargeCapPF);

            root.Add(sfContainer);

            return root;
        }
    }
}
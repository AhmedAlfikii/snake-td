using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConsumableData))]
public class ConsumableDataEditor : Editor
{
    private SerializedProperty id;
    private SerializedProperty displayName;
    private SerializedProperty icon;
    private SerializedProperty startingValue;

    private SerializedProperty isRechargeable;
    private SerializedProperty chargingDuration;
    private SerializedProperty chargeAmount;

    private SerializedProperty isCapped;
    private SerializedProperty maximumAmount;

    void OnEnable()
    {
        id = serializedObject.FindProperty("ID");
        displayName = serializedObject.FindProperty("DisplayName");
        icon = serializedObject.FindProperty("Icon");
        startingValue = serializedObject.FindProperty("StartingValue");

        isRechargeable = serializedObject.FindProperty("IsRechargeable");
        chargingDuration = serializedObject.FindProperty("ChargingDuration");
        chargeAmount = serializedObject.FindProperty("ChargeAmount");

        isCapped = serializedObject.FindProperty("IsCapped");
        maximumAmount = serializedObject.FindProperty("MaximumAmount");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        #region Header
        StringBuilder header = new StringBuilder();
        if (!string.IsNullOrEmpty(displayName.stringValue) && !string.IsNullOrEmpty(id.stringValue))
        {
            header.Append(displayName.stringValue);
            header.Append(" (");
            header.Append(id.stringValue);
            header.Append(")");
        }
        else if (!string.IsNullOrEmpty(displayName.stringValue) && string.IsNullOrEmpty(id.stringValue))
            header.Append(displayName.stringValue);
        else if (string.IsNullOrEmpty(displayName.stringValue) && !string.IsNullOrEmpty(id.stringValue))
            header.Append(id.stringValue);

        EditorGUILayout.LabelField(header.ToString(), EditorStyles.boldLabel);
        #endregion

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(id);
        EditorGUILayout.PropertyField(displayName);
        EditorGUILayout.PropertyField(icon);
        EditorGUILayout.PropertyField(startingValue);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Charging", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(isRechargeable);
        if (isRechargeable.boolValue)
        {
            EditorGUILayout.PropertyField(chargingDuration);
            EditorGUILayout.PropertyField(chargeAmount);
        }

        if (isRechargeable.boolValue && isCapped.boolValue)
        {
            EditorGUILayout.Space();
            float totalChargesNeeded = Mathf.Ceil(maximumAmount.floatValue / chargeAmount.floatValue);
            float totalTime = totalChargesNeeded * chargingDuration.floatValue;

            float totalHours = totalTime / 60f;

            int hours = Mathf.FloorToInt(totalHours);
            int minutes = Mathf.RoundToInt((totalHours - hours) * 60);

            EditorGUILayout.LabelField(string.Format("Total charges needed are: {0}.", totalChargesNeeded));
            EditorGUILayout.LabelField(string.Format("Time needed to fully charge: {0} minutes. ({1} hour(s) {2} minute(s))", totalTime, hours, minutes));
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Cap", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(isCapped);

        if (isCapped.boolValue)
            EditorGUILayout.PropertyField(maximumAmount);

        serializedObject.ApplyModifiedProperties();
    }
}

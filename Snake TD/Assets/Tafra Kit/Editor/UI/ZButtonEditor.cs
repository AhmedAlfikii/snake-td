using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;
using UnityEditor;
using UnityEditor.UI;
using TafraKit.UI;

namespace TafraKitEditor.UI
{
    [CustomEditor(typeof(ZButton)), CanEditMultipleObjects]
    public class ZButtonEditor : ZSelectableEditor
    {
        private SerializedProperty m_OnClickProperty;
        private SerializedProperty m_OnClickSFXProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
            m_OnClickSFXProperty = serializedObject.FindProperty("m_OnClickSFX");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.PropertyField(m_OnClickProperty);
            EditorGUILayout.PropertyField(m_OnClickSFXProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
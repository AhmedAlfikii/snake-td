using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using TafraKit.Mathematics;

namespace TafraKitEditor.Mathematics
{
    [CustomPropertyDrawer(typeof(FormulasContainer))]
    public class FormulasContainerDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializeReferenceListContainerField srListContainer = new SerializeReferenceListContainerField(property, "formulas", false, "Formula", "Formulas", property.tooltip);
            
            VisualElement previewContainer = new VisualElement();
            
            previewContainer.style.flexDirection = FlexDirection.Row;
            previewContainer.style.flexWrap = Wrap.NoWrap;
            previewContainer.style.marginTop = 5f;
            previewContainer.style.marginLeft = 5f;
            previewContainer.style.marginRight = 5f;

            VisualElement previewContainerHalf1 = new VisualElement();
            VisualElement previewContainerHalf2 = new VisualElement();

            previewContainerHalf1.style.flexGrow = 0;
            previewContainerHalf1.style.flexDirection = FlexDirection.Row;
            previewContainerHalf1.style.flexWrap = Wrap.NoWrap;
            previewContainerHalf2.style.flexDirection = FlexDirection.Row;
            previewContainerHalf2.style.flexWrap = Wrap.NoWrap;

            Label previewLabel = new Label("Preview");
            previewLabel.style.alignSelf = Align.Center;
            previewLabel.style.flexGrow = 1;

            Label atXLabel = new Label("at x = ");
            atXLabel.style.alignSelf = Align.Center;

            FloatField previewField = new FloatField();
            previewField.style.width = 100;
            previewField.style.marginRight = 10;

            Label previewValue = new Label("y = 0");
            previewValue.style.alignSelf = Align.Center;

            previewContainerHalf1.Add(previewLabel);
            previewContainerHalf2.Add(atXLabel);
            previewContainerHalf2.Add(previewField);
            previewContainerHalf2.Add(previewValue);

            previewContainer.Add(previewContainerHalf1);
            previewContainer.Add(previewContainerHalf2);

            srListContainer.Content.Insert(0, previewContainer);

            previewField.RegisterValueChangedCallback(OnPreviewInputChanged);

            void OnPreviewInputChanged(ChangeEvent<float> evnt)
            {
                float val = evnt.newValue;
            }

            srListContainer.schedule.Execute(() =>
            {
                if(float.TryParse(previewField.text, out float val))
                {
                    FormulasContainer equationsContainer = (FormulasContainer)property.boxedValue;

                    previewValue.text = $"y = {equationsContainer.Evaluate(val)}";
                }
            }).Every(100);

            return srListContainer;
        }
    }
}
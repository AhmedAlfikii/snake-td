#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TafraKitEditor.UIElement
{
    public static class PropertyFieldExtentions
    {
        /// <summary>
        /// Ties a group of visual elements to a property field of a bool property, the tied elements will be hidden or displayed depending on the bool value of the property field.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="defaultState">The starting state of the bool, pass in the property's current value.</param>
        /// <param name="showOnTrue">Should the tied elements appear if the bool value is true? otherwise they will appear if it's false and hide if it's true.</param>
        /// <param name="tiedElements">The elements to hide/show depeneding on the bool's current value.</param>
        public static void TieVisualElementsToBoolField(this PropertyField field, bool defaultState, bool showOnTrue, params VisualElement[] tiedElements)
        {
            DisplayStyle initialStyle;

            if (showOnTrue)
                initialStyle = defaultState ? DisplayStyle.Flex : DisplayStyle.None;
            else
                initialStyle = defaultState ? DisplayStyle.None : DisplayStyle.Flex;

            for(int i = 0; i < tiedElements.Length; i++)
            {
                var element = tiedElements[i];
                if(element != null)
                    element.style.display = initialStyle;
            }

            field.RegisterCallback<ChangeEvent<bool>>((ev) =>
            {
                DisplayStyle displayStyle;

                if(showOnTrue)
                    displayStyle = ev.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                else
                    displayStyle = ev.newValue ? DisplayStyle.None : DisplayStyle.Flex;

                for(int i = 0; i < tiedElements.Length; i++)
                {
                    var element = tiedElements[i];
                    if (element != null)
                        element.style.display = displayStyle;
                }
            });
        }
    }
}
#endif
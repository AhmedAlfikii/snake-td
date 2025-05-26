using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor
{
    public static class StyleExtentions
    {
        public static void SetMargins(this IStyle style, float margin)
        {
            style.marginTop =
            style.marginRight =
            style.marginBottom =
            style.marginLeft =

            margin;
        }
        public static void SetPadding(this IStyle style, float margin)
        {
            style.paddingTop =
            style.paddingRight =
            style.paddingBottom =
            style.paddingLeft =

            margin;
        }
        public static void SetBorderWidth(this IStyle style, float borderWidth)
        {
            style.borderTopWidth = 
            style.borderRightWidth = 
            style.borderBottomWidth =
            style.borderLeftWidth =
            
            borderWidth;
        }

        public static void SetBorderRadius(this IStyle style, float borderRadius)
        {
            style.borderTopRightRadius = 
            style.borderBottomRightRadius =
            style.borderBottomLeftRadius = 
            style.borderTopLeftRadius =

            borderRadius;
        }

        public static void SetBorderColor(this IStyle style, Color color)
        {
            style.borderTopColor =
            style.borderRightColor =
            style.borderBottomColor =
            style.borderLeftColor =
            
            color;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace TafraKit
{
    public static class StringExtentions
    {
        public static void CopyToClipboard(this string value)
        {
            GUIUtility.systemCopyBuffer = value;
        }
        public static void SplitNonAlloc(this string value, char separator, List<string> listToFill) 
        {
            listToFill.Clear();

            if(string.IsNullOrEmpty(value))
                return;

            int segmentStartIndex = 0;

            int lettersCount = value.Length;

            for(int i = 0; i < lettersCount; i++)
            {
                var c = value[i];

                if(c == separator)
                {
                    listToFill.Add(value.Substring(segmentStartIndex, i - segmentStartIndex));
                    segmentStartIndex = i + 1;
                }
                else if(i == lettersCount - 1)
                {
                    listToFill.Add(value.Substring(segmentStartIndex, i - segmentStartIndex + 1));
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit
{
    public class TextIndentLevels : MonoBehaviour
    {
        [System.Serializable]
        struct Indent
        {
            [Range(0, 1)]
            public float DescentLevel;
            [Range(0, 1)]
            public float IndentLevel;
        }

        [SerializeField] private List<Indent> indentLevels;
        [SerializeField] private TextMeshProUGUI tmproTXT;
        [SerializeField] private RectTransform myRT;

        private StringBuilder messageConstructionSB = new StringBuilder();
        private StringBuilder posTagSB = new StringBuilder();

        public void AdaptTextToIndentLevels(string message)
        {
            tmproTXT.enableAutoSizing = true;
            tmproTXT.textWrappingMode = TextWrappingModes.Normal;
            tmproTXT.text = message;
            tmproTXT.ForceMeshUpdate();

            float autoSizedSize = tmproTXT.fontSize;
            tmproTXT.enableAutoSizing = false;
            //tmproTXT.fontSize = autoSizedSize;

            TMP_TextInfo textInfo = tmproTXT.textInfo;

            int linesCount = textInfo.lineCount;

            messageConstructionSB.Clear();
            messageConstructionSB.Append(message);

            Vector3 topLeftLocal = new Vector3(myRT.rect.xMin, myRT.rect.yMax);
            Vector3 botLeftLocal = new Vector3(myRT.rect.xMin, myRT.rect.yMin);
            Vector3 topToDown = botLeftLocal - topLeftLocal;
            float height = topToDown.magnitude;

            int addedCharactersCount = 0;
            for(int i = 0; i < linesCount; i++)
            {
                TMP_LineInfo lineInfo = tmproTXT.textInfo.lineInfo[i];
                int firstCharacterIndex = lineInfo.firstCharacterIndex;
                TMP_CharacterInfo firstCharacter = textInfo.characterInfo[firstCharacterIndex];

                float distance = Vector3.Distance(topLeftLocal, firstCharacter.bottomLeft);
                float descentPercentage = distance / height;

                float indentPercentage = 0;

                for(int indentLevelIndex = 0; indentLevelIndex < indentLevels.Count; indentLevelIndex++)
                {
                    if(descentPercentage > indentLevels[indentLevelIndex].DescentLevel)
                    {
                        if(indentLevelIndex < indentLevels.Count - 1)
                        {
                            Indent indentLevelA = indentLevels[indentLevelIndex];
                            Indent indentLevelB = indentLevels[indentLevelIndex + 1];

                            float inverseLerp = Mathf.InverseLerp(indentLevelA.DescentLevel, indentLevelB.DescentLevel, descentPercentage);
                            indentPercentage = Mathf.Lerp(indentLevelA.IndentLevel, indentLevelB.IndentLevel, inverseLerp);
                        }
                        else
                            indentPercentage = indentLevels[indentLevelIndex].IndentLevel;
                    }
                    else if(descentPercentage == indentLevels[indentLevelIndex].DescentLevel)
                        indentPercentage = indentLevels[indentLevelIndex].IndentLevel;
                }

                posTagSB.Clear();

                int toAddCharacters = 0;
                if(i != 0)
                {
                    posTagSB.Append("\n");
                    toAddCharacters++;
                }

                string pos = $"<pos={Mathf.RoundToInt(indentPercentage * 100)}%>";

                posTagSB.Append(pos);

                toAddCharacters += pos.Length;

                messageConstructionSB.Insert(firstCharacterIndex + addedCharactersCount, posTagSB.ToString());

                addedCharactersCount += toAddCharacters;
            }

            tmproTXT.textWrappingMode = TextWrappingModes.NoWrap;

            tmproTXT.text = messageConstructionSB.ToString();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(!myRT || !tmproTXT) return;

            Vector3 topLeftLocal = new Vector3(myRT.rect.xMin, myRT.rect.yMax);
            Vector3 topRightLocal = new Vector3(myRT.rect.xMax, myRT.rect.yMax);
            Vector3 botLeftLocal = new Vector3(myRT.rect.xMin, myRT.rect.yMin);

            Vector3 topLeftWorld = transform.TransformPoint(topLeftLocal);
            Vector3 topRightWorld = transform.TransformPoint(topRightLocal);
            Vector3 botLeftWorld = transform.TransformPoint(botLeftLocal);

            Vector3 topToDown = botLeftWorld - topLeftWorld;
            Vector3 teftToRight = topRightWorld - topLeftWorld;

            for(int i = 0; i < indentLevels.Count; i++)
            {
                if(i > 0)
                {
                    Vector3 lineStart = topLeftWorld + topToDown * indentLevels[i - 1].DescentLevel + teftToRight * indentLevels[i - 1].IndentLevel;
                    Vector3 lineEnd = topLeftWorld + topToDown * indentLevels[i].DescentLevel + teftToRight * indentLevels[i].IndentLevel;

                    Handles.DrawBezier(lineStart, lineEnd, lineStart, lineEnd, Color.black, null, 5f);
                }
            }
        }
#endif
    }
}
using System.Collections.Generic;
using TafraKitEditor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using TafraKit;

namespace TafraKitEditor
{
    [CustomPropertyDrawer(typeof(TilesGroup))]
    public class TilesGroupProperty : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Tafra Kit/UI Toolkit Assets/Style Sheets/Grid Styling.uss");
            root.styleSheets.Add(styleSheet);

            CollapsibleContainer mainContainer = new CollapsibleContainer(property.displayName, property.propertyPath);

            VisualElement tilesContainer = new VisualElement();

            mainContainer.Content.Add(tilesContainer);

            DrawGrid();

            Button clearButton = new Button(() =>
            {
                ClearGrid();
            });
            clearButton.text = "Clear";
            clearButton.style.alignSelf = Align.Center;
            clearButton.style.marginTop = 20;
            clearButton.style.width = 100;
            clearButton.style.height = 30;

            mainContainer.Content.Add(clearButton);

            root.Add(mainContainer);

            root.RegisterCallback<AttachToPanelEvent>((ev) =>
            {
                Undo.undoRedoPerformed += DrawGrid;
            });
            root.RegisterCallback<DetachFromPanelEvent>((ev) =>
            {
                Undo.undoRedoPerformed -= DrawGrid;
            });

            return root;

            void DrawGrid()
            {
                SerializedProperty additionalTilesProperty = property.FindPropertyRelative("additionalTiles");

                List<Vector2Int?> tiles = new List<Vector2Int?>();

                tilesContainer.Clear();

                int tilesCount = additionalTilesProperty.arraySize;
                Vector2Int pivotColumnRow = new Vector2Int(1, 1);
                Vector2Int minTile = new Vector2Int(0, 0);
                Vector2Int maxTile = new Vector2Int(0, 0);

                for(int i = 0; i < tilesCount; i++)
                {
                    Vector2Int? v = additionalTilesProperty.GetArrayElementAtIndex(i).boxedValue as Vector2Int?;
                    tiles.Add(v);

                    Vector2Int vVal = v.Value;

                    if(vVal.x < minTile.x)
                    {
                        minTile.x = vVal.x;
                        pivotColumnRow.x = 1 + (minTile.x) * -1;
                    }
                    else if(vVal.x > maxTile.x)
                        maxTile.x = vVal.x;

                    if(vVal.y < minTile.y)
                    {
                        minTile.y = vVal.y;
                        pivotColumnRow.y = 1 + (minTile.y) * -1;
                    }
                    else if(vVal.y > maxTile.y)
                        maxTile.y = vVal.y;
                }

                int rows = 3 + (maxTile.y - minTile.y);
                int columns = 3 + (maxTile.x - minTile.x);

                float spacing = 5;
                float tileSize = 45;

                for(int rowIndex = 0; rowIndex < rows; rowIndex++)
                {
                    VisualElement rowElement = new VisualElement();
                    rowElement.AddToClassList("row");
                    rowElement.style.height = tileSize;
                    rowElement.style.flexDirection = FlexDirection.Row;

                    if(rowIndex != rows - 1)
                        rowElement.style.marginTop = spacing / 2f;
                    if(rowIndex != 0)
                        rowElement.style.marginBottom = spacing / 2f;

                    for(int columnIndex = 0; columnIndex < columns; columnIndex++)
                    {
                        int x = columnIndex - pivotColumnRow.x;
                        int y = rowIndex - pivotColumnRow.y;

                        Button tile = new Button();

                        if(x == 0 && y == 0) //Main tile
                        {
                            tile.AddToClassList("tileMain");

                            tile.tooltip = "Tile (0, 0) is the main tile and is always active.";
                        }
                        else
                        if(tiles.Contains(new Vector2Int(x, y))) //Active tile
                        {
                            tile.AddToClassList("tileActive");

                            tile.clicked += () =>
                            {
                                int elementIndex = tiles.IndexOf(new Vector2Int(x, y));
                                additionalTilesProperty.DeleteArrayElementAtIndex(elementIndex);
                                property.serializedObject.ApplyModifiedProperties();
                                DrawGrid();
                            };
                        }
                        else //Inactive tile
                        {
                            tile.AddToClassList("tile");

                            tile.clicked += () =>
                            {
                                additionalTilesProperty.InsertArrayElementAtIndex(tilesCount);
                                additionalTilesProperty.GetArrayElementAtIndex(tilesCount).boxedValue = new Vector2Int(x, y);
                                property.serializedObject.ApplyModifiedProperties();
                                DrawGrid();
                            };
                        }

                        tile.text = $"({x}, {y})";
                        tile.style.height = tileSize;
                        tile.style.width = tileSize;

                        if(columnIndex != 0)
                            tile.style.marginLeft = spacing / 2f;
                        if(columnIndex != columns - 1)
                            tile.style.marginRight = spacing / 2f;


                        rowElement.Add(tile);
                    }
                    tilesContainer.style.flexDirection = FlexDirection.ColumnReverse;

                    tilesContainer.Add(rowElement);
                }
            }
            void ClearGrid()
            {
                SerializedProperty additionalTilesProperty = property.FindPropertyRelative("additionalTiles");

                additionalTilesProperty.ClearArray();
                property.serializedObject.ApplyModifiedProperties();

                DrawGrid();
            }
        }
    }
}
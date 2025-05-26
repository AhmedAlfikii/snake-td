using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.UI
{
    public class UISkinReceiver : MonoBehaviour
    {
        [SerializeField] private UISkin testSkin;
        
        [Space()]

        [SerializeField] private List<IdentifiableObjects<Image>> imagesSprites;
        [SerializeField] private List<IdentifiableObjects<Graphic>> graphicColors;
        [SerializeField] private List<IdentifiableObjects<Graphic>> graphicMaterials;
        [SerializeField] private List<IdentifiableObjects<TextMeshProUGUI>> textStrings;
        [SerializeField] private List<IdentifiableObjects<GameObject>> gameObjects;

        private Dictionary<string, List<Image>> imageSpritesDictionary;
        private Dictionary<string, List<Graphic>> graphicColorsDictionary;
        private Dictionary<string, List<Graphic>> graphicMaterialsDictionary;
        private Dictionary<string, List<TextMeshProUGUI>> textStringsDictionary;
        private Dictionary<string, List<GameObject>> gameObjectsDictionary;
        private bool isInitialized;
        private UISkin appliedSkin;

        protected virtual void OnDisable()
        {
            if (appliedSkin != null)
            {
                appliedSkin.Release();
                appliedSkin = null;
            }
        }
        private void Initialize()
        {
            if(isInitialized)
                return;

            isInitialized = true;

            if(imagesSprites.Count > 0)
            {
                imageSpritesDictionary = new Dictionary<string, List<Image>>();

                for (int i = 0; i < imagesSprites.Count; i++)
                {
                    var data = imagesSprites[i];

                    imageSpritesDictionary.Add(data.ID, data.Values);
                }
            }

            if(graphicColors.Count > 0)
            {
                graphicColorsDictionary = new Dictionary<string, List<Graphic>>();

                for (int i = 0; i < graphicColors.Count; i++)
                {
                    var data = graphicColors[i];

                    graphicColorsDictionary.Add(data.ID, data.Values);
                }
            }

            if(graphicMaterials.Count > 0)
            {
                graphicMaterialsDictionary = new Dictionary<string, List<Graphic>>();

                for (int i = 0; i < graphicMaterials.Count; i++)
                {
                    var data = graphicMaterials[i];

                    graphicMaterialsDictionary.Add(data.ID, data.Values);
                }
            }

            if(textStrings.Count > 0)
            {
                textStringsDictionary = new Dictionary<string, List<TextMeshProUGUI>>();

                for (int i = 0; i < textStrings.Count; i++)
                {
                    var data = textStrings[i];

                    textStringsDictionary.Add(data.ID, data.Values);
                }
            }

            if(gameObjects.Count > 0)
            {
                gameObjectsDictionary = new Dictionary<string, List<GameObject>>();

                for (int i = 0; i < gameObjects.Count; i++)
                {
                    var data = gameObjects[i];

                    gameObjectsDictionary.Add(data.ID, data.Values);
                }
            }
        }

        public void ApplySkin(UISkin skin)
        {
            if(!isInitialized)
                Initialize();

            if(appliedSkin != null)
                appliedSkin.Release();

            skin.Load();

            appliedSkin = skin;

            for (int i = 0; i < imagesSprites.Count; i++)
            {
                var imageSprite = imagesSprites[i];

                if(skin.TryGetSprite(imageSprite.ID, out var sprite))
                {
                    for(int j = 0; j < imageSprite.Values.Count; j++)
                    {
                        imageSprite.Values[j].sprite = sprite;
                    }
                }
                else
                    TafraDebugger.Log("UI Skin Receiver", $"Couldn't find the image sprite with the ID {imageSprite.ID}.", TafraDebugger.LogType.Error);
            }

            for (int i = 0; i < textStrings.Count; i++)
            {
                var textString = textStrings[i];

                if(skin.TryGetText(textString.ID, out var text))
                {
                    for(int j = 0; j < textString.Values.Count; j++)
                    {
                        textString.Values[j].text = text;
                    }
                }
                else
                    TafraDebugger.Log("UI Skin Receiver", $"Couldn't find the text string with the ID {textString.ID}.", TafraDebugger.LogType.Error);
            }

            for (int i = 0; i < graphicColors.Count; i++)
            {
                var graphicColor = graphicColors[i];

                if(skin.TryGetColor(graphicColor.ID, out var color))
                {
                    for(int j = 0; j < graphicColor.Values.Count; j++)
                    {
                        graphicColor.Values[j].color = color;
                    }
                }
                else
                    TafraDebugger.Log("UI Skin Receiver", $"Couldn't find the graphic color with the ID {graphicColor.ID}.", TafraDebugger.LogType.Error);
            }

            for (int i = 0; i < graphicMaterials.Count; i++)
            {
                var graphicMaterial = graphicMaterials[i];

                if(skin.TryGetMaterial(graphicMaterial.ID, out var material))
                {
                    for(int j = 0; j < graphicMaterial.Values.Count; j++)
                    {
                        var value = graphicMaterial.Values[j];

                        if (value is TextMeshProUGUI tmpUGUI)
                            tmpUGUI.fontMaterial = material;
                        else
                            graphicMaterial.Values[j].material = material;
                    }
                }
                else
                    TafraDebugger.Log("UI Skin Receiver", $"Couldn't find the graphic material with the ID {graphicMaterial.ID}.", TafraDebugger.LogType.Error);
            }

            for (int i = 0; i < gameObjects.Count; i++)
            {
                var gameObject = gameObjects[i];

                if(skin.TryGetActiveState(gameObject.ID, out var state))
                {
                    for(int j = 0; j < gameObject.Values.Count; j++)
                    {
                        gameObject.Values[j].SetActive(state);
                    }
                }
                else
                    TafraDebugger.Log("UI Skin Receiver", $"Couldn't find the game object active state with the ID {gameObject.ID}.", TafraDebugger.LogType.Error);
            }
        }

        [ContextMenu("Apply Test Skin (Play Mode)")]
        private void ApplyTestSkin()
        {
            if(!Application.isPlaying)
            {
                TafraDebugger.Log("UI Skin Receiver", "Editor is not in play mode, can not apply test skin.", TafraDebugger.LogType.Error);
                return;
            }

            ApplySkin(testSkin);
        }
    }
}
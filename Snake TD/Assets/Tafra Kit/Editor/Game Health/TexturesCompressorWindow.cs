using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace TafraKitEditor
{
    public class TexturesCompressorWindow : EditorWindow
    {
        [MenuItem("Tafra Games/Windows/Game Health/Textures Compressor")]
        private static void Open()
        {
            GetWindow<TexturesCompressorWindow>("Textures Compressor");
        }

        private void CreateGUI()
        {
            Label textureCompressionHeader = new Label("Texture Compressor");
            textureCompressionHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            textureCompressionHeader.style.fontSize = 14;
            textureCompressionHeader.style.marginTop = 5;
            textureCompressionHeader.style.marginBottom = 5;

            HelpBox helpBox = new HelpBox("Overriding all textures compression format to \"ASTC 8X8 block\"", HelpBoxMessageType.Info);

            IntegerField sizeField = new IntegerField("Max Size");
            sizeField.value = 512;

            Button convertAllTexturesButton = new Button(() => { CompressAllTextures(sizeField.value); });
            convertAllTexturesButton.text = "Compress All Textures";

            Button convertSelectedTexturesButton = new Button(() => { CompressSelectedTextures(sizeField.value); });
            convertSelectedTexturesButton.text = "Compress Selected Textures";

            rootVisualElement.Add(textureCompressionHeader);
            rootVisualElement.Add(helpBox);
            rootVisualElement.Add(sizeField);
            rootVisualElement.Add(convertSelectedTexturesButton);
            rootVisualElement.Add(convertAllTexturesButton);
        }

        private void CompressAllTextures(int maxSize)
        {
            if(!EditorUtility.DisplayDialog("Warning", "Compressing all textures could take some time, are you sure you want to proceed?", "Ok", "Cancel"))
                return;

            string[] guids = AssetDatabase.FindAssets("t:texture");

            for(int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);

                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if(textureImporter != null)
                {
                    TextureImporterPlatformSettings androidSettings = new TextureImporterPlatformSettings
                    {
                        name = "Android",
                        overridden = true,
                        format = TextureImporterFormat.ASTC_8x8,
                        maxTextureSize = maxSize
                    };
                    TextureImporterPlatformSettings iOSSettings = new TextureImporterPlatformSettings
                    {
                        name = "iPhone",
                        overridden = true,
                        format = TextureImporterFormat.ASTC_8x8,
                        maxTextureSize = maxSize
                    };

                    textureImporter.SetPlatformTextureSettings(androidSettings);
                    textureImporter.SetPlatformTextureSettings(iOSSettings);

                    break;
                }
            }
        }
        private void CompressSelectedTextures(int maxSize)
        {
            for(int i = 0; i < Selection.objects.Length; i++)
            {
                var selectedObject = Selection.objects[i];

                string path = AssetDatabase.GetAssetPath(selectedObject);

                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if(textureImporter != null)
                {
                    TextureImporterPlatformSettings androidSettings = new TextureImporterPlatformSettings
                    {
                        name = "Android",
                        overridden = true,
                        format = TextureImporterFormat.ASTC_8x8,
                        maxTextureSize = maxSize
                    };
                    TextureImporterPlatformSettings iOSSettings = new TextureImporterPlatformSettings
                    {
                        name = "iPhone",
                        overridden = true,
                        format = TextureImporterFormat.ASTC_8x8,
                        maxTextureSize = maxSize
                    };

                    textureImporter.SetPlatformTextureSettings(androidSettings);
                    textureImporter.SetPlatformTextureSettings(iOSSettings);
                }
            }
        }
    }
}
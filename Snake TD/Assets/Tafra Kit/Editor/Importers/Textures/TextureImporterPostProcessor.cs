using System.Collections.Generic;
using TafraKit;
using UnityEditor;
using UnityEngine;

namespace TafraKitEditor
{
    public class TextureImporterPostProcessor : AssetPostprocessor
    {
        private void OnPostprocessTexture(Texture2D texture)
        {
            if(!TafraTextureImporterWindow.AutoConvertUI)
                return;

            if(!assetPath.Contains("/UI/"))
                return;

            TextureImporter importer = assetImporter as TextureImporter;

            if(importer.textureType == TextureImporterType.Sprite)
                return;

            TafraDebugger.Log("Tafra Texture Importer", $"Converting texture {texture} to 2D Sprite, since one of its parent folders is called \"UI\".", TafraDebugger.LogType.Info, texture);

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;

            AssetDatabase.ForceReserializeAssets(new List<string>() { assetPath });
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
        }
    }
} 
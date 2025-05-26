using System;
using TafraKit.ContentManagement;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    [SearchMenuItem("Object")]
    public class ObjectAbilityAssetModule : AbilityAssetModule
    {
        [SerializeField] private TafraAsset<UnityEngine.Object> asset;
        [SerializeField] private string internalBlackboardObjectProperty;

        public override void LoadAsset(Ability ability)
        {
            int blackboardPropertyNameHash = Animator.StringToHash(internalBlackboardObjectProperty);

            var bbProperty = ability.Blackboard.TryGetObjectProperty(blackboardPropertyNameHash, -1);

            if(bbProperty == null)
            {
                TafraDebugger.Log("Object Ability Asset Module", $"Couldn't find an Object property with the name \"{internalBlackboardObjectProperty}\" in the internal blackboard.",
                    TafraDebugger.LogType.Error, ability);
                return;
            }

            if(asset.IsLoaded)
            {
                bbProperty.value = asset.LoadedAsset;
            }
            else
            {
                bbProperty.value = asset.Load();
                //asset.Load((go) =>
                //{
                //    bbProperty.value = go;
                //}, () =>
                //{
                //    TafraDebugger.Log("Object Ability Asset Module", "Failed to load associate asset.", TafraDebugger.LogType.Error, ability);
                //});
            }
        }

        public override void ReleaseAsset()
        {
            asset.Release();
        }
    }
}
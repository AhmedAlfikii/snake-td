using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using UnityEngine.Serialization;
using ZUI;

namespace TafraKit.GraphViews
{
    public class ExternalBlackboardsSettings : SettingsModule
    {
        [SerializeField] private List<ExternalBlackboard> externalBlackboards = new List<ExternalBlackboard>();

        public List<ExternalBlackboard> ExternalBlackboards => externalBlackboards;
        public override int Priority => 23;
        public override string Name => "AI/External Blackboards";
        public override string Description => "Automatically initializes assigned blackboards on game start (before the first scene is loaded).";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            ExternalBlackboardsSettings settings = TafraSettings.GetSettings<ExternalBlackboardsSettings>();

            if(settings == null)
                return;

            for (int i = 0; i < settings.externalBlackboards.Count; i++)
            {
                settings.externalBlackboards[i].Initialize();
            }
        }
    }
}

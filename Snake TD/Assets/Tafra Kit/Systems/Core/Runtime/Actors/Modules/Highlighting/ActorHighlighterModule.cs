using System;
using System.Collections.Generic;
using TafraKit.Loot;
using TafraKit.RPG;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    [SearchMenuItem("Highlighting/Actor Highlighter")]
    public class ActorHighlighterModule : TafraActorModule
    {
        [Serializable]
        public struct ActorHighlightData
        {
            public Color baseColor;
            public bool colorOutline;
            public Color outlineColor;
            public int priority;

            public ActorHighlightData(Color baseColor, bool colorOutline, Color outlineColor, int priority)
            { 
                this.baseColor = baseColor;
                this.colorOutline = colorOutline;
                this.outlineColor = outlineColor;
                this.priority = priority;
            }
        }

        [Header("Setup")]
        [SerializeField] private List<Renderer> renderers = new List<Renderer>();
        [SerializeField] private string materialOverlayColorName = "_OverlayColor2";
        [SerializeField] private TafraOutline outline;
      
        private List<Material> materials;
        private Color defaultColor = new Color(0, 0, 0, 0);
        private Color defaultOutlineColor;
        private int materialsCount;
        private InfluenceReceiver<ActorHighlightData> highlightInfluenceReceiver;

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => true;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            highlightInfluenceReceiver = new InfluenceReceiver<ActorHighlightData>(ShoudReplaceHighlight, OnActiveHighlightUpdated, null, OnAllHighlightersCleared);
           
            if(outline != null)
                defaultOutlineColor = outline.OutlineColor;

            InitializeRenderers();
        }

        private void OnAllHighlightersCleared()
        {
            SetHighlightColor(defaultColor);
            SetOutlineColor(defaultOutlineColor);
        }
        private void OnActiveHighlightUpdated(ActorHighlightData color)
        {
            SetHighlightColor(color.baseColor);

            if (color.colorOutline)
                SetOutlineColor(color.outlineColor);
            else
                SetOutlineColor(defaultOutlineColor);
        }
        private bool ShoudReplaceHighlight(ActorHighlightData newInfluence, ActorHighlightData oldInfluence)
        {
            return newInfluence.priority <= oldInfluence.priority;
        }

        private void SetHighlightColor(Color color)
        {
            for (int i = 0; i < materialsCount; i++)
            {
                materials[i].SetColor(materialOverlayColorName, color);
            }
        }
        private void SetOutlineColor(Color color)
        {
            if(outline == null)
                return;

            outline.OutlineColor = color;
        }
        private void InitializeRenderers()
        {
            if(renderers.Count == 0)
            {
                return;
            }

            for(int i = 0; i < renderers.Count; i++)
            {
                var renderer = renderers[i];
                for(int j = 0; j < renderer.materials.Length; j++)
                {
                    var material = renderer.materials[j];

                    if(material == null)
                        continue;

                    if(materials == null)
                        materials = new List<Material>();

                    materials.Add(material);
                }
            }

            materialsCount = materials.Count;
        }

        public void AddHighlighter(string highlighterId, ActorHighlightData highlightData)
        {
            highlightInfluenceReceiver.AddInfluence(highlighterId, highlightData);
        }
        public void RemoveHighlighter(string highlighterId)
        { 
            highlightInfluenceReceiver.RemoveInfluence(highlighterId);
        }
    }
}
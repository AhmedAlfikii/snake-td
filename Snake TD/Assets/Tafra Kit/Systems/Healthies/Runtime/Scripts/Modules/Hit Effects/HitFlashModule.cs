using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Hit Effects/Hit Flash")]
    public class HitFlashModule : HealthyModule
    {
        [Header("Properties")]
        [SerializeField] private Color flashColor = new Color(1, 1, 1, 0.63f);
        [SerializeField] private float duration = 0.25f;
        [SerializeField] private AnimationCurve flashCurve = new AnimationCurve(new Keyframe(0, 0, 10.23f, 10.23f), new Keyframe(0.28f, 1), new Keyframe(1, 0, -2, -2));

        [Header("Setup")]
        [SerializeField] private List<Renderer> renderers = new List<Renderer>();
        [SerializeField] private string materialOverlayColorName = "_OverlayColor";

        private List<Material> materials;
        private IEnumerator flashingEnum;
        private bool isFlashing;
        private bool canWork;
        private int materialsCount;
        private Color defaultColor = new Color(0, 0, 0, 0);

        public List<Material> Materials => materials;
        public override bool DisableOnDeath => true;
        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            InitializeRenderers();
        }
        protected override void OnEnable()
        {
            healthy.Events.OnTakenDamage.AddListener(OnTakenDamage);
        }
        protected override void OnDisable()
        {
            healthy.Events.OnTakenDamage.RemoveListener(OnTakenDamage);

            if(isFlashing)
            {
                for(int i = 0; i < materialsCount; i++)
                {
                    var material = materials[i];

                    material.SetColor(materialOverlayColorName, defaultColor);
                }
            }

            if(flashingEnum != null)
                healthy.StopCoroutine(flashingEnum);
        }
        public override void OnDestroy()
        {
            for(int i = 0; i < materialsCount; i++)
            {
                UnityEngine.Object.Destroy(materials[i]);
            }
        }
        private void OnTakenDamage(Healthy healthy, HitInfo hitInfo)
        {
            if(flashingEnum != null)
                healthy.StopCoroutine(flashingEnum);

            flashingEnum = Flashing();

            healthy.StartCoroutine(flashingEnum);
        }
        private void InitializeRenderers()
        {
            if(renderers.Count == 0)
            {
                canWork = false;
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
            canWork = materialsCount > 0;
        }
        private IEnumerator Flashing()
        {
            isFlashing = true;

            float startTime = Time.time;
            float endTime = startTime + duration;

            Color color = flashColor;
            while(Time.time < endTime)
            {
                float t = (Time.time - startTime) / duration;

                color.a = Mathf.Lerp(0, flashColor.a, flashCurve.Evaluate(t));

                for(int i = 0; i < materialsCount; i++)
                {
                    materials[i].SetColor(materialOverlayColorName, color);
                }

                yield return null;
            }

            for(int i = 0; i < materialsCount; i++)
            {
                materials[i].SetColor(materialOverlayColorName, defaultColor);
            }

            isFlashing = false;
        }

        public void UpdateRenderers(List<Renderer> renderers)
        {
            for(int i = 0; i < materialsCount; i++)
            {
                UnityEngine.Object.Destroy(materials[i]);
            }

            this.renderers.Clear();
            for (int i = 0; i < renderers.Count; i++)
            {
                this.renderers.Add(renderers[i]);
            }

            InitializeRenderers();
        }
    }
}
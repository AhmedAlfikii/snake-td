using TafraKit.Healthies;
using UnityEngine;
using TafraKit;
using TafraKit.ContentManagement;
using System;

[SearchMenuItem("Healing/Healing Multiplier")]
public class HealingMultiplierModule : HealthyModule
{
    [SerializeField] private TafraAsset<ScriptableFloat> healingMultiplierFloatAsset;

    [NonSerialized] private ScriptableFloat healingMultiplierFloat;

    public override bool DisableOnDeath => true;
    public override bool UseUpdate => false;
    public override bool UseLateUpdate => false;
    public override bool UseFixedUpdate => false;

    protected override void OnEnable()
    {
        healingMultiplierFloat = healingMultiplierFloatAsset.Load();

        healthy.Events.OnAboutToHeal.AddListener(OnAboutToHeal);
    }
    protected override void OnDisable()
    {
        healthy.Events.OnAboutToHeal.RemoveListener(OnAboutToHeal);

        healingMultiplierFloatAsset.Release();
    }
    private void OnAboutToHeal(Healthy healthy, HealEventArgs args)
    {
        args.ManipulatedHeal *= healingMultiplierFloat.Value;
    }
}
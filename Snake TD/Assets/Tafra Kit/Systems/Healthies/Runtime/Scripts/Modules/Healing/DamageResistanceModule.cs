using UnityEngine;
using TafraKit;
using TafraKit.Healthies;
using TafraKit.AI3;

[SearchMenuItem("Healing/Damage Resistance")]
public class DamageResistanceModule : HealthyModule
{
    [SerializeField] private ScriptableFloat damageResistanceFloat;
    [SerializeField] private ScriptableFloat bossesDamageResistanceFloat;
    [SerializeField] private ScriptableFloat minionsDamageResistanceFloat;
    [SerializeField] private ScriptableFloat meleeDamageResistanceFloat;
    [SerializeField] private ScriptableFloat rangedDamageResistanceFloat;

    public override bool DisableOnDeath => true;
    public override bool UseUpdate => false;
    public override bool UseLateUpdate => false;
    public override bool UseFixedUpdate => false;

    protected override void OnEnable()
    {
        healthy.Events.OnAboutToTakeDamage.AddListener(OnAboutToTakeDamage);
    }
    protected override void OnDisable()
    {
        healthy.Events.OnAboutToTakeDamage.RemoveListener(OnAboutToTakeDamage);
    }
    private void OnAboutToTakeDamage(Healthy healthy, HitEventArgs args)
    {
        float originalDamage = args.ManipulatedHitInfo.damage;
        float resistance = damageResistanceFloat.Value;

        Enemy enemy = null;

        if (args.OriginalHitInfo.hitter != null)
            enemy = ComponentProvider.GetComponent<Enemy>(args.OriginalHitInfo.hitter.gameObject);

        if (enemy)
        {
            if (enemy.IsBoss)
            {
                resistance += bossesDamageResistanceFloat.Value;
            }
            else
            {
                resistance += minionsDamageResistanceFloat.Value;
            }
        }

        if (args.OriginalHitInfo.isRanged)
        {
            resistance += rangedDamageResistanceFloat.Value;
        }
        else
        {
            resistance += meleeDamageResistanceFloat.Value;
        }
        args.SetDamage(originalDamage - (originalDamage * resistance));
    }
}
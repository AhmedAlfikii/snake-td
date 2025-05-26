namespace TafraKit
{
    public class DamageDisplayerSettings : SettingsModule
    {
        public bool Enabled = false;
        public DamageText DamageTextPrefab;
        public DamageText CriticalDamageTextPrefab;
        public DamageText MissDamageTextPrefab;
        public float SpawnRandomRange = 0.5f;

        public override int Priority => 11;
        public override string Name => "Combat/Damage Displayer";
        public override string Description => "Displaying the number of damage taken by a healthy unit.";
    }
}
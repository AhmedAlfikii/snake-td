using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tiles Group")]
    public class AbilitySystemObjectTilesGroupInitiailzer : AbilitySystemObjectInitiailzer
    {
        [SerializeField] private TilesGroup tilesGroup;

        public override object GetObject()
        {
            return tilesGroup;
        }
    }
}
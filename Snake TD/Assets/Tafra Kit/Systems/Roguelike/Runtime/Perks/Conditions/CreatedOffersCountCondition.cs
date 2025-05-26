using TafraKit.Conditions;
using TafraKit.Roguelike;
using UnityEngine;

namespace TafraKit.Internal.Roguelike
{
    [SearchMenuItem("Perks/Created Offers Count")]
    public class CreatedOffersCountCondition : Condition
    {
        [SerializeField] private PerksGroup group;
        [SerializeField] private NumberRelation relation;
        [SerializeField] private int targetCount;

        protected override bool PerformCheck()
        {
            return ZHelper.IsNumberRelationValid(group.TotalCreatedOffersCount, targetCount, relation);
        }
    }
}
using TafraKit.Conditions;
using TafraKit.Roguelike;
using UnityEngine;

namespace TafraKit.Internal.Roguelike
{
    [SearchMenuItem("Perks/Combined Created Offers Count")]
    public class CombinedCreatedOffersCountCondition : Condition
    {
        [SerializeField] private PerksGroup[] groups;
        [SerializeField] private NumberRelation relation;
        [SerializeField] private int targetCount;

        protected override bool PerformCheck()
        {
            int total = 0;
            for (int i = 0; i < groups.Length; i++)
            {
                total += groups[i].TotalCreatedOffersCount;
            }

            return ZHelper.IsNumberRelationValid(total, targetCount, relation);
        }
    }
}
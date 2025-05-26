using System.Collections.Generic;
using System.Linq;
using TafraKit.Consumables;
using TafraKit.RPG;
using UnityEngine;

public class RewarderTest : MonoBehaviour
{
    [SerializeField] private ConsumableChange[] consumableRewards;
    [SerializeField] private StorableScriptableObject[] storableRewards;

    [ContextMenu("Reward")]
    public void Reward()
    {
        ItemRewarder.AddConsumables(consumableRewards.ToList());


        List<StorableScriptableObject> storablesInstances = new List<StorableScriptableObject>();
        
        for (int i = 0; i < storableRewards.Length; i++)
        {
            StorableScriptableObject storable = storableRewards[i].InstancableSO.CreateInstance() as StorableScriptableObject;

            storable.Quantity = 3;
            
            storablesInstances.Add(storable);
        }
        
        ItemRewarder.AddStorables(storablesInstances);
        
        ItemRewarder.ShowScreen();
    }
}

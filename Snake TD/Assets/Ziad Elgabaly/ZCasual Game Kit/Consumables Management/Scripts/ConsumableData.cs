using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Consumable", menuName = "ZCasual Game Kit/Consumables Management/Consumable Data")]
public class ConsumableData : ScriptableObject
{
    [Tooltip("The unique ID used to identify this consumable.")]
    public string ID;
    [Tooltip("The display name of this consumable, only used to visually represent it to players.")]
    public string DisplayName;
    [Tooltip("The icon to represent this consumable.")]
    public Sprite Icon;
    [Tooltip("The value players will have the first time they start the game.")]
    public float StartingValue;

    [Tooltip("Does this consumable increase over time?")]
    public bool IsRechargeable;
    [Tooltip("The duration in minutes between each charge.")]
    public float ChargingDuration;
    [Tooltip("The amount of units to add in each charge.")]
    public float ChargeAmount;

    [Tooltip("Does this consumable have a maximum units amount?")]
    public bool IsCapped;
    [Tooltip("The maximum amount of units this consumable can reach.")]
    public float MaximumAmount;
}

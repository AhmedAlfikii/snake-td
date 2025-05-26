using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZCasualGameKit;
using ZUtilities;

public class ConsumablesDemo : MonoBehaviour
{
    [SerializeField] private float coinsToAdd = 100;
    [SerializeField] private RectTransform coinStartPoint;
    [SerializeField] private ConsumableIncreaseAnimationData coinIncreaseAnim;

    public void AddCoins()
    {
        coinIncreaseAnim.ShowingUnitsCustomPosition = coinStartPoint.position;
        
        ConsumablesManager.Instance.AddUnits("coins", coinsToAdd);
    }
}

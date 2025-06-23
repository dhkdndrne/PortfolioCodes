using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UniRx;
using UnityEngine;

public class PlayerCurrency
{
    public ReactiveProperty<BigInteger> gold = new();
    
    public void IncreaseGold(double value)
    {
        double newValue = value * Player.Instance.OwnEffectDic[OwnEffectType.Gold];
        gold.Value += new BigInteger(value + newValue);
    }

    public void DecreaseGold(BigInteger value)
    {
        gold.Value -= value;
    }
    
    
}

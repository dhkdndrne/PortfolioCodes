using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BuffGroup
{
    private List<Buff> buffs = new List<Buff>();
    private Unit target;

    public BuffGroup(Unit target)
    {
        this.target = target;
    }

    public void AddBuff(Buff buff)
    {
        buffs.Add(buff);
        target.AddBuff(buff);
    }

    public async void Activate(float duration)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        Deactivate();
    }

    public void Deactivate()
    {
        foreach (var buff in buffs)
        {
            target.RemoveBuff(buff);
        }
        buffs.Clear();
    }
}

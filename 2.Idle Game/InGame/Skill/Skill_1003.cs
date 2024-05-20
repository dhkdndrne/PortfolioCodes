using System.Collections;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Skill_1003 : SkillBase
{
	private float atkSpeed;

    public override void Init(Dictionary<string, object> data)
    {
        base.Init(data);
        
        var effectType = (OwnEffectType)Enum.Parse(typeof(OwnEffectType), data["own_effectType"].ToString());
       // var baseEffect = Convert.ToDouble(data["own_effect_value"].ToString());
        var levelUpEffect = Convert.ToDouble(data["own_effect_up"].ToString());
        
        atkSpeed = Convert.ToSingle(data["value_2"]);
    }
    
    public async override UniTaskVoid UseSkill()
    {
	    base.UseSkill().Forget();

	    Player.Instance.PlayerUnit.unitBase.Stat.AtkSpeed += atkSpeed;
	    await UniTask.Delay(TimeSpan.FromSeconds(duration));
	    
	    Player.Instance.PlayerUnit.unitBase.Stat.AtkSpeed -= atkSpeed;
	    EndSkill();
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using Bam.Extensions;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Skill_1001 : SkillBase
{
   
   // 구체 개수
   // 구체 데미지
   // 구체 범위

   private int amount;
   private double damage;
   private float range;
   private float levelUpDmgVal;
   private readonly float SPEED = 3f;
   
   public override void Init(Dictionary<string, object> data)
   {
      base.Init(data);
      
      amount = Convert.ToInt32(data["value_1"]);
      damage = NumberTranslater.TranslateStringToDouble(data["value_2"].ToString());
      range = Convert.ToSingle(data["value_3"]);
      levelUpDmgVal = Convert.ToSingle(data["value_2_up"]);
   }
   
   public override void ApplyLevelUpValues(int level)
   {
      damage += levelUpDmgVal * level;
   }
   
   public async override UniTaskVoid UseSkill()
   {
      if (Player.Instance.PlayerUnit.GetTargetSqrDistance() > Extensions.Pow(range, 2))
         return;
      
      base.UseSkill().Forget();

      float interval = duration / amount;
      int amt = amount;

      while (amt > 0)
      {
         var bullet = ObjectPoolManager.Instance.Spawn("1001").GetComponent<Projectile>();
         bullet.transform.position = Player.Instance.PlayerUnit.transform.position;
         bullet.Init(Player.Instance.PlayerUnit,Player.Instance.PlayerUnit.Target,Player.Instance.GetTotalDamage() * (1 + (damage * 0.01f)), SPEED);

         await UniTask.Delay(TimeSpan.FromSeconds(interval));
         amt--;
      }

      EndSkill();
   }
   
   public override string GetDescription(string description)
   {
      return description.Replace("{duration}", $"{duration}").Replace("{value_1}", $"{amount}").Replace("{value_2 * level}", $"{damage}").Replace("{value_3}", $"{range}");
   }
}

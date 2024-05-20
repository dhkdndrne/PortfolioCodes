using UnityEngine;
public class Item_Skill : ItemBase
{
   public SkillBase Skill { get; private set; }

   public bool IsEquipped { get; private set; }
    
   public void EquipSkill()
   {
	   IsEquipped = true;
	   Skill.SetUseable();
   }

   public void UnEquipSkill()
   {
	   IsEquipped = false;
   }
   
   public void Init(int id,bool isLock, string itemName,ItemRankType rank, int level, int maxLevel, int piece, int maxPiece, OwnEffect ownEffect,string description,SkillBase skill)
   {
	   Skill = skill;
	   base.Init(id,isLock, itemName,rank, level, maxLevel, piece, maxPiece, ownEffect,description);
       
	   Skill.ApplyLevelUpValues(Level.Value);
   }
    
   public override string GetDescription()
   {
	   Skill.ApplyLevelUpValues(Level.Value);
	   return Skill.GetDescription(description);
   }
}

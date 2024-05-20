using System;
using System.Linq;
using UniRx;
using UnityEngine;

public class SkillSystem
{
   private bool isAutoMode;
   private Item_Skill[] equippedSkillList = new Item_Skill[6];
   
   public Action<int,int> onEquipSkillAction;

   public void Init()
   {
      isAutoMode = DataManager.Instance.PlayerData.isAutoSkill;
      
      var equipedSkillDatas = DataManager.Instance.PlayerData.equippedSkillIDs;
      var itemManager = ItemManager.Instance;
       
      for (int i = 0; i < equipedSkillDatas.Length; i++)
      {
         var skillData = equipedSkillDatas[i];
         if (skillData.id != 0)
         {
            equippedSkillList[i] = itemManager.GetSkillData(skillData.rank,skillData.id);
            equippedSkillList[i].EquipSkill();
         }
      }
   }

   public bool IsAutoMode
   {
      get { return isAutoMode;}
      set
      {
         isAutoMode = value;
         DataManager.Instance.PlayerData.isAutoSkill = value;
         DataManager.Instance.SaveData();
      }
   }
    
   public void EquipSkill(Item_Skill skill, int idx)
   {
      int index = idx != -1 ? idx : Array.FindIndex(equippedSkillList, slot => slot == null);
    
      if (index != -1)
      {
         equippedSkillList[index] = skill;
         equippedSkillList[index].EquipSkill();;
         onEquipSkillAction?.Invoke(skill.ID, index);

         DataManager.Instance.RefreshEquippedSkill(index,skill);
      }
   }

   public void UnEquipSkill(int id)
   {
      int idx = Array.FindIndex(equippedSkillList, skill => skill != null && skill.ID == id);

      if (idx == -1)
      {
         UtilClass.DebugLog("장착된 스킬중 같은 아이디가 없음",LogType.LogError);
         return;
      }

      if (equippedSkillList[idx].Skill.IsUsing)
      {
         UtilClass.DebugLog("사용중에는 해제 할 수 없습니다.");
         return;
      }
      
      equippedSkillList[idx].UnEquipSkill();
      equippedSkillList[idx] = null;
      DataManager.Instance.RefreshEquippedSkill(idx,null);
      onEquipSkillAction?.Invoke(0, idx);
   }
    
   public void UpdateSkillCoolTime(float deltaTime)
   {
      foreach (var itemSkill in equippedSkillList)
      {
         if(itemSkill == null)
            continue;

         if (IsAutoMode && itemSkill.Skill.CanUse)
         {
            itemSkill.Skill.UseSkill().Forget();
         }
         
         if(!itemSkill.Skill.IsUsing)
         {
            itemSkill.Skill.UpdateCoolTime(deltaTime);
         }
         else if (itemSkill.Skill.IsUsing)
         {
            itemSkill.Skill.UpdateElapsedTime(deltaTime);
         }
      }
   }

   public Item_Skill GetSkillItem(int index) => equippedSkillList[index];
}

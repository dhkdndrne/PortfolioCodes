using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.Mathematics;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
	[SerializeField] private Transform weaponHolder;
	private List<Weapon> equipedWeaponList = new List<Weapon>();
	
    private void Start()
    {
	    var objectPoolManager = ObjectPoolManager.Instance;
		
	    GameManager.Instance.Step.Where(step=>step is GameStep.Playing).Subscribe(_ =>
	    {
		    var weaponList = ItemManager.Instance.equipedItemList[ItemType.Weapon];
		    foreach (var weapon in weaponList)
		    {
			    objectPoolManager.CheckAlreadyExist(weapon.ItemSo.ID.ToString(),weapon.ItemSo.WeaponPrefab);
			    var obj = objectPoolManager.Spawn(weapon.ItemSo.ID.ToString()).GetComponent<Weapon>();

			    obj.transform.parent = weaponHolder;
			    obj.transform.SetLocalPositionAndRotation(Vector3.zero,quaternion.identity);
				
			    obj.Init(weapon.ItemSo);
			    equipedWeaponList.Add(obj);
		    }
			
	    }).AddTo(this);
	    
	    GameManager.Instance.Step.Where(step=>step is GameStep.UnLockSlot).Subscribe(_ =>
	    {
		    foreach (var weapon in equipedWeaponList)
		    {
			    weapon.DeSpawn();
		    }
	    }).AddTo(this);
    }

    public void UpdateCoolTime(float deltaTime)
    {
	    foreach (var weapon in equipedWeaponList)
	    {
		    weapon.UpdateCoolTime(deltaTime);
	    }
    }
}

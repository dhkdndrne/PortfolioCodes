using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/Item/NonAttack", fileName = "New NonAttack So")]
public class UnattackableItemSo : ItemSo
{
	[Header("능력치")]
	[SerializeField] private AbilityToken[] abilities;

	public IEnumerable<AbilityToken> GetAbility()
	{
		for (int i = 0; i < abilities.Length; i++)
			yield return abilities[i];
	}
}

[System.Serializable]
public class AbilityToken
{
	public AbilityType ability;
	public int value;
}
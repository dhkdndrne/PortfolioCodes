using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Stat/CharStat", fileName = "New Char Stat")]
public class CharacterStatData : ScriptableObject
{
	public List<InitalStat> InitalStats = new List<InitalStat>();
}

[System.Serializable]
public class InitalStat
{
	public AbilityType abilityType;
	public int value;
}
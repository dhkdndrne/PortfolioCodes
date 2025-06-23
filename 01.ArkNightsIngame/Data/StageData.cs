using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
	[SerializeField] private int characterLimit;
	[SerializeField] private int lifePoint;
	[SerializeField] private int initialCost;
	[SerializeField] private int maxCost = 99;
	[SerializeField] private float costIncreaseTime;
	
	[SerializeField] private Wave wave;
	
	public Wave Wave => wave;
	public int CharacterLimit => characterLimit;
	public int LifePoint => lifePoint;
	public int InitialCost => initialCost;
	public int MaxCost => maxCost;
	public float CostIncreaseTime => costIncreaseTime;
	public int GetTotalEnemyCount()
	{
		return wave.waveList.Sum(w => w.waveDatas.Sum(data => data.order));
	}
}
using System.Collections.Generic;
using Bam.Singleton;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class DataManager : DontDestroySingleton<DataManager>
{
	private OperatorData operatorData = new OperatorData();
	private EnemyData enemyData = new EnemyData();
	
	[SerializeField] private List<RangeData> rangeDatas = new List<RangeData>();
	public readonly Dictionary<int,RangeData> RangeDic = new Dictionary<int, RangeData>();
	
	public OperatorData OperatorData => operatorData;
	public EnemyData EnemyData => enemyData;
	
	
	protected override void Init()
	{
		foreach (var data in rangeDatas)
		{
			var split = data.name.Split('_');
			RangeDic.Add(int.Parse(split[1]),data);
		}
		
		Test().Forget();
	}

	private async UniTaskVoid Test()
	{
		await UniTask.WhenAll(operatorData.LoadData(),enemyData.LoadData());
		SceneManager.LoadScene("Stage_1-1");
	}
}
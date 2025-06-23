using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Bam.Extensions;
public class OperatorData
{
	#region URL

	private const string URL_OperatorLevelData = "https://docs.google.com/spreadsheets/d/1bV3EVBbS1NfVahKsUT4W1dVwksKTrQxQOjGhkrrAgAA/export?format=tsv";
	private const string URL_OperatorFixedData = "https://docs.google.com/spreadsheets/d/1bV3EVBbS1NfVahKsUT4W1dVwksKTrQxQOjGhkrrAgAA/export?format=tsv&gid=345562641";
	private const string URL_OperatorSkillData = "https://docs.google.com/spreadsheets/d/1bV3EVBbS1NfVahKsUT4W1dVwksKTrQxQOjGhkrrAgAA/export?format=tsv&gid=2091602837";

	#endregion

	private Dictionary<int, OperatorBaseData> opDataDic = new Dictionary<int, OperatorBaseData>();

	//오퍼레이터 id / 리스트(스킬 1 , 스킬2, 스킬3)
	private Dictionary<int, OperatorSkillData> opSkillDataDic = new Dictionary<int, OperatorSkillData>();

	public OperatorBaseData GetOperatorData(int opId)
	{
		return opDataDic.ContainsKey(opId) ? opDataDic[opId] : null;
	}

	public OperatorSkillData GetOperatorSkillData(int skillID)
	{
		return opSkillDataDic.ContainsKey(skillID) ? opSkillDataDic[skillID] : null;
	}

	public async UniTask LoadData()
	{
		await UniTask.WhenAll(LoadOperatorData(), LoadSkillData());

		Debug.LogSuccess("csv 로드 완료");
	}


	private async UniTask LoadOperatorData()
	{
		var fixedData = await SpreadSheetReader.LoadGoogleSheet(URL_OperatorFixedData);
		for (int i = 1; i < fixedData.Length; i++)
		{
			string[] column = fixedData[i].Split("\t");

			int id = int.Parse(column[0]);
			string name = column[1];
			int rank = int.Parse(column[2]);
			Operator_Class operatorClass = (Operator_Class)Enum.Parse(typeof(Operator_Class), column[3]);
			Operator_AtkType atkType = (Operator_AtkType)Enum.Parse(typeof(Operator_AtkType), column[4]);
			int rangeID = int.Parse(column[5]);
			var obd = new OperatorBaseData();
			obd.Init(id, name, rank, operatorClass, atkType,rangeID,column[6]);
			opDataDic.Add(id, obd);
		}

		Debug.LogSuccess("오퍼레이터 고정 데이터 로드 완료");

		// 오퍼레이터 레벨별 데이터 받아오기
		var levelData = await SpreadSheetReader.LoadGoogleSheet(URL_OperatorLevelData);
		for (int i = 1; i < levelData.Length; i++)
		{
			string[] column = levelData[i].Split("\t");

			int id = int.Parse(column[0]);
			opDataDic[id].SetStatData(column);
		}
		Debug.LogSuccess("오퍼레이터 레벨 별 데이터 로드 완료");
	}
	private async UniTask LoadSkillData()
	{
		var data = await SpreadSheetReader.LoadGoogleSheet(URL_OperatorSkillData);
		int curId = -9999;

		for (int i = 1; i < data.Length; i++)
		{
			string[] column = data[i].Split("\t");

			int id = int.Parse(column[0]);

			if (curId != id)
			{
				curId = id;
				var skillData = new OperatorSkillData();
				string name = column[1];

				skillData.Init(name);
				opSkillDataDic.Add(id, skillData);
			}

			opSkillDataDic[id].SetData(column);
		}

		Debug.LogSuccess("스킬 데이터 로드 완료");
	}
}

#region 오퍼레이터 데이터

/// <summary>
/// 오퍼레이터의 기본 데이터
/// </summary>
public class OperatorBaseData
{
	private int id;
	private string name;
	private int rangeID;
	private int rank;
	private Operator_Class operatorClass;
	private Operator_AtkType attackType;
	private List<OperatorLevelData> statList = new List<OperatorLevelData>();
	private int[] skillIDs;

	public int ID => id;
	public string Name => name;
	public int Rank => rank;
	public int[] SkillIDs => skillIDs;
	public int RangeID => rangeID;
	
	public Operator_AtkType OperatorAtkType => attackType;
	public Operator_Class OperatorClass => operatorClass;
	public OperatorLevelData GetLevelData(int level)
	{
		return level > statList.Count ? null : statList[level - 1];
	}

	public void Init(int id, string name, int rank, Operator_Class operatorClass, Operator_AtkType attackType,int rangeID ,string skillLists)
	{
		this.id = id;
		this.name = name;
		this.rank = rank;
		this.rangeID = rangeID;
		this.attackType = attackType;
		this.operatorClass = operatorClass;
		skillIDs = ConvertSkillIDs(skillLists);
	}

	public void SetStatData(string[] data)
	{
		int cost = int.Parse(data[3]);
		float maxHp = float.Parse(data[4]);
		float redeployTime = float.Parse(data[5]);
		int block = int.Parse(data[6]);
		float atkPower = float.Parse(data[7]);
		float defense = float.Parse(data[8]);
		float magicResistance = float.Parse(data[9]);
		float attackCoolTime = float.Parse(data[10]);

		statList.Add(new OperatorLevelData(cost, maxHp, redeployTime, block, atkPower, defense, magicResistance, attackCoolTime));
	}

	private int[] ConvertSkillIDs(string data)
	{
		return data.IsNullOrWhitespace() ? null : Array.ConvertAll(data.Split("/"), int.Parse);
	}
}
/// <summary>
/// 오퍼레이터의 레벨별 변화하는 데이터
/// </summary>
public class OperatorLevelData
{
	private int cost;
	private float maxHp;
	private float redeployTime;
	private int block;
	private int range_ID;
	private float atk_Power;
	private float defense;
	private float magic_Resistance;
	private float attackCoolTime;

	public int Cost => cost;
	public float MaxHp => maxHp;
	public float RedeployTime => redeployTime;
	public int Block => block;
	public float Atk_Power => atk_Power;
	public float Defense => defense;
	public float Magic_Resistance => magic_Resistance;
	public float AttackCoolTime => attackCoolTime;

	public OperatorLevelData(int cost,  float maxHp, float redeployTime, int block, float atkPower, float defense, float magicRes, float attackCoolTime)
	{
		this.cost = cost;

		this.maxHp = maxHp;
		this.redeployTime = redeployTime;
		this.block = block;
		this.atk_Power = atkPower;
		this.defense = defense;
		this.magic_Resistance = magicRes;
		this.attackCoolTime = attackCoolTime;
	}
}

#endregion

#region 스킬 데이터

public class OperatorSkillData
{
	private string name;
	private List<OperatorSkillLevelData> levelDataList = new List<OperatorSkillLevelData>();

	public string Name => name;

	public OperatorSkillLevelData GetSkillLevelData(int level)
	{
		return level > levelDataList.Count ? null : levelDataList[level - 1];
	}

	public void Init(string name)
	{
		this.name = name;
	}

	public void SetData(string[] data)
	{
		int initial_Sp = int.Parse(data[3]);
		int sp_cost = int.Parse(data[4]);

		SpChargeType spChargeType = (SpChargeType)Enum.Parse(typeof(SpChargeType), data[5]);
		SkillActiveType activeType = (SkillActiveType)Enum.Parse(typeof(SkillActiveType), data[6]);

		float duration = float.Parse(data[7]);
		int stackCount = int.Parse(data[8]);
		int rangeID = int.Parse(data[9]);
		levelDataList.Add(new OperatorSkillLevelData(initial_Sp, sp_cost, spChargeType, activeType, duration,stackCount ,rangeID, data[13], ConvertStringToAbility(data)));
	}

	/// <summary>
	/// 스킬 능력치 변화 데이터 Convert
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	private SkillAttribute[] ConvertStringToAbility(string[] data)
	{
		int cnt = 0;
		int index = 10; // csv에서 Ability카테고리가 처음 등장하는 인덱스
		while (true)
		{
			if (cnt >= 3 || data[index].IsNullOrWhitespace() || data[index].Equals("Description"))
				break;

			index++;
			cnt++;
		}

		SkillAttribute[] abilities = new SkillAttribute[cnt];
		index = 10;

		for (int i = 0; i < cnt; i++)
		{
			var split = data[index++].Split("_");

			AttributeType type = (AttributeType)Enum.Parse(typeof(AttributeType), split[0]);
			float value = float.Parse(split[1]);

			abilities[i] = new SkillAttribute(type, value);
		}

		return abilities;
	}
}
public class OperatorSkillLevelData
{
	public int Initial_Sp { get; private set; }
	public int Sp_cost { get; private set; }
	public SpChargeType SpChargeType { get; private set; }
	public SkillActiveType ActiveType { get; private set; }
	public float Duration { get; private set; }
	public int RangeID { get; private set; }
	public SkillAttribute[] Attributes { get; private set; }
	public string Description { get; private set; }
	public int StackCount { get; private set; }
	public OperatorSkillLevelData(int initial_Sp, int sp_cost, SpChargeType spChargeType, SkillActiveType activeType,
		float duration,int stackCount ,int rangeID, string description, params SkillAttribute[] attributes)
	{
		Initial_Sp = initial_Sp;
		Sp_cost = sp_cost;
		SpChargeType = spChargeType;
		ActiveType = activeType;
		Duration = duration;
		RangeID = rangeID;
		Description = description;
		Attributes = attributes;
		StackCount = stackCount;
	}
}
public class SkillAttribute
{
	public AttributeType AttributeType { get; private set; }
	public float Value { get; private set; }
	public SkillAttribute(AttributeType type, float value)
	{
		AttributeType = type;
		Value = value;
	}
}

#endregion
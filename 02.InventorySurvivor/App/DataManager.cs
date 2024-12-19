using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Bam.Extensions;
using Bam.Singleton;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public class DataManager : DontDestroySingleton<DataManager>
{

	#region URL

	private readonly string URL_SynergyData = "https://docs.google.com/spreadsheets/d/1RXiYA-hu1Z_vdz-dvHHoCkWOlgCQQBS-8EzbpI5IwYI/export?format=csv";
	private readonly string URL_WeaponData = "https://docs.google.com/spreadsheets/d/1RXiYA-hu1Z_vdz-dvHHoCkWOlgCQQBS-8EzbpI5IwYI/export?format=csv&gid=943203920";
	private readonly string URL_PassiveSkillData = "https://docs.google.com/spreadsheets/d/1RXiYA-hu1Z_vdz-dvHHoCkWOlgCQQBS-8EzbpI5IwYI/export?format=csv&gid=2060217760";
	private readonly string URL_StageData = "https://docs.google.com/spreadsheets/d/1RXiYA-hu1Z_vdz-dvHHoCkWOlgCQQBS-8EzbpI5IwYI/export?format=csv&gid=1648537001";
	private readonly string URL_PlayerExpData = "https://docs.google.com/spreadsheets/d/1RXiYA-hu1Z_vdz-dvHHoCkWOlgCQQBS-8EzbpI5IwYI/export?format=csv&gid=413978599";

    #endregion

	#region Data

	public List<int> PlayerExpList { get; private set; } = new List<int>();

    #endregion

	private static readonly string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	private readonly string LINE_SPLIT_REGEX = @"\r\n|\n\r|\n|\r"; // 줄 바꿈 기준 정규 표현식


	private struct StageInfo
	{
		public int level;
		public int time;
	}

	// private void Start()
	// {
	// 	LoadData().Forget();
	// }

	public async UniTask LoadData()
	{
		var (task_SynergyInfo, task_PlayerExp) =
			await UniTask.WhenAll(
				LoadGoogleSheet(URL_SynergyData),
				LoadGoogleSheet(URL_PlayerExpData));

		SetSynergyData(task_SynergyInfo);
		SetPlayerExpData(task_PlayerExp);
	}
	
	public async UniTask<string[]> GetWeaponData()
	{
		return await LoadGoogleSheet(URL_WeaponData);
	}
	/// <summary>
	/// 구글 스프레드 시트 받아서 string 배열로 변환하는 함수
	/// </summary>
	/// <param name="url"></param>
	/// <returns></returns>
	private async UniTask<string[]> LoadGoogleSheet(string url)
	{
		try
		{
			var www = UnityWebRequest.Get(url);
			var res = await www.SendWebRequest(); // Unity의 Async Operation 이라 await 가능하다.
			var data = res.downloadHandler.text;

			return Regex.Split(data, LINE_SPLIT_REGEX);
		}
		catch (System.Exception e)
		{
			UtilClass.DebugLog(e, LogType.LogError);
			return null;
		}
	}

	/// <summary>
	/// 시너지 데이터 저장하는 함수
	/// </summary>
	/// <param name="lines"></param>
	private void SetSynergyData(string[] lines)
	{
		List<SynergyBuff> tempBuffData = new List<SynergyBuff>();
		for (int i = 1; i < lines.Length; i++)
		{
			tempBuffData.Clear();

			string[] column = lines[i].Split(',');

			int synergy_Id = int.Parse(column[0]);
			var synergy_keyword = (SynergyKeyword)Enum.Parse(typeof(SynergyKeyword), column[1]);
			int[] synergy_Condition = Array.ConvertAll(column[2].Split('/'), int.Parse);

			int index = 3;
			while (column.Length > index && !column[index].IsNullOrWhitespace())
			{
				var abilityType = (AbilityType)Enum.Parse(typeof(AbilityType), column[index]);
				var values = column[index + 1].IsNullOrWhitespace() ? null : Array.ConvertAll(column[index + 1].Split('/'), int.Parse);

				tempBuffData.Add(new SynergyBuff(abilityType, values));
				index += 2;
			}

			Synergy synergy = new Synergy(synergy_Id, synergy_keyword, synergy_Condition, tempBuffData.ToArray());
			SynergyManager.Instance.Init(synergy);
		}
	}
	private void SetPlayerExpData(string[] lines)
	{
		for (int i = 1; i < lines.Length; i++)
		{
			string[] column = lines[i].Split(',');
			int exp = int.Parse(column[1]);
			PlayerExpList.Add(exp);
		}
	}
}
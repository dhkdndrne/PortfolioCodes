using System;
using System.Numerics;
using Bam.Singleton;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class OffLineRewardSystem : Singleton<OffLineRewardSystem>
{
	private readonly string URL = "www.google.com";

	[SerializeField] private UI_OffLineReward uiOffLineReward;
	
	private void Start()
	{
		CheckOffLineReward().Forget();
	}

	private async UniTaskVoid CheckOffLineReward()
	{
		if (Bam.Extensions.Extensions.IsNullOrWhitespace(DataManager.Instance.PlayerData.time))
			return;
        
		var savedTime = DateTime.Parse(DataManager.Instance.PlayerData.time).ToLocalTime();
        
		var time = await GetWebTime();
		var nowTime = DateTime.Parse(time).ToLocalTime();
		var timeSpan = nowTime - savedTime;

		if (timeSpan.Hours < 1)
			return;
		
		uiOffLineReward.gameObject.SetActive(true);
		int day = timeSpan.Days;
		int hour = timeSpan.Hours + (timeSpan.Minutes / 60);
      
		hour += day * 24;
        
		BigInteger gold = new BigInteger(hour * (Player.Instance.GetTotalDamage() * 0.5f));
		Player.Instance.Currency.gold.Value += gold;
		
		uiOffLineReward.ShowReward(hour,NumberTranslater.TranslateNumber(gold));
		
		DataManager.Instance.SaveData();
	}

	public async UniTask<string> GetWebTime()
	{
		var t = await CheckWebTime();
		return t.ToString();
	}

	private async UniTask<DateTime> CheckWebTime()
	{
		UnityWebRequest request = new UnityWebRequest();
		using (request = UnityWebRequest.Get(URL))
		{
			await request.SendWebRequest();

			if (request.isNetworkError)
			{
				Debug.Log(request.error);
				return default;
			}

			string date = request.GetResponseHeader("date"); // 이곳에서 반송된 데이터에 시간 데이터가 존재 
			return DateTime.Parse(date).ToLocalTime();       // ToLocalTime() 메소드로 한국시간으로 변환시켜 준다.
		}
	}
}
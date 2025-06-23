using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public static class SpreadSheetReader
{
	private const string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	private const string LINE_SPLIT_REGEX = @"\r\n|\n\r|\n|\r"; // 줄 바꿈 기준 정규 표현식
	
	
	public static async UniTask<string[][]> LoadDataMulti(params string[] urls)
	{
		List<UniTask<string[]>> taskList = new List<UniTask<string[]>>(urls.Length);
		
		foreach (string url in urls)
		{
			taskList.Add(LoadGoogleSheet(url));
		}
	
		string[][] results = await UniTask.WhenAll(taskList);
		return results;
	}
	public static async UniTask<string[]> LoadGoogleSheet(string url)
	{
		try
		{
			var www = UnityWebRequest.Get(url);
			var res = await www.SendWebRequest(); // Unity의 Async Operation 이라 await 가능하다.
			var data = res.downloadHandler.text;

			return Regex.Split(data, LINE_SPLIT_REGEX);
		}
		catch (Exception e)
		{
			Debug.LogError(e);
			return null;
		}
	}
}

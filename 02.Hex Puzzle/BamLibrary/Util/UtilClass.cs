using System.Reflection;
using System.Text;
using UnityEngine;
public enum LogType
{
	Log,
	LogError,
	Warning,
	Try,
	Success
}

public static class Debug
{
	//StringBuilder 꼭 초기화 하고 사용
	private static StringBuilder sb = new();
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void Log(object msg)
	{
		sb.Clear();
		sb.Append("<color=#C8C8C8>").Append("<b> [").Append(msg).Append("] </b></color>");;
		UnityEngine.Debug.Log(sb.ToString());
	}
	public static void LogError(object msg)
	{
		sb.Clear();
		sb.Append("<color=#a52a2aff>").Append("<b> [").Append(msg).Append("] </b></color>");;
		UnityEngine.Debug.Log(sb.ToString());
	}

	public static void LogWarning(object msg)
	{
		sb.Clear();
		sb.Append("<color=#C18E2B>").Append("<b> [").Append(msg).Append("] </b></color>");;
		UnityEngine.Debug.Log(sb.ToString());
	}
	public static void LogTry(object msg)
	{
		sb.Clear();
		sb.Append("<color=#3F92B5>").Append("<b> [").Append(msg).Append("] </b></color>");;
		UnityEngine.Debug.Log(sb.ToString());
	}	
	public static void LogSuccess(object msg)
	{
		sb.Clear();
		sb.Append("<color=#37D946>").Append("<b> [").Append(msg).Append("] </b></color>");;
		UnityEngine.Debug.Log(sb.ToString());
	}
	public static void LogFormat(string format, params object[] args)
	{
		sb.Clear();
		sb.Append("<color=#C8C8C8>").Append("<b> [").AppendFormat(format, args).Append("] </b></color>");
		UnityEngine.Debug.Log(sb.ToString());
	}
}
public static class Util
{
	public static ColorLayer GetRandomColorLayerExceptNone()
	{
		// ColorLayer enum의 값들을 배열로 변환
		ColorLayer[] allLayers = (ColorLayer[])System.Enum.GetValues(typeof(ColorLayer));
        
		// None을 제외한 배열을 필터링
		ColorLayer[] filteredLayers = System.Array.FindAll(allLayers, layer => layer != ColorLayer.None);

		// 무작위로 값 선택
		int randomIndex = Random.Range(0, filteredLayers.Length);
		return filteredLayers[randomIndex];
	}
	
	public static void CopyFields<T>(T source, T target)
	{
		var fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		foreach (var field in fields)
		{
			var value = field.GetValue(source);
			field.SetValue(target, value);
		}
	}
}
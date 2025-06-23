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
	// Assert 함수
	public static void Assert(bool condition, object msg)
	{
		if (condition)
		{
			sb.Clear();
			sb.Append("<color=#ff0000ff>").Append("<b> [Assert Failed: ").Append(msg).Append("] </b></color>");
			UnityEngine.Debug.LogError(sb.ToString());
		}
	}
}
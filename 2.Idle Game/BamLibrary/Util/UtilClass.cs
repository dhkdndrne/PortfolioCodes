using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Bam.Extensions;

public static class UtilClass
{
	//StringBuilder 꼭 초기화 하고 사용
	private static StringBuilder sb = new();
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void DebugLog(object msg, LogType logType = LogType.Log)
	{
		sb.Clear();

		switch (logType)
		{
			case LogType.Log:
				sb.Append($"<color=#C8C8C8>");
				break;

			case LogType.LogError:
				sb.Append($"<color=#a52a2aff>");
				break;

			case LogType.Warning:
				sb.Append($"<color=#C18E2B>");
				break;

			case LogType.Try:
				sb.Append($"<color=#3F92B5>");
				break;

			case LogType.Success:
				sb.Append($"<color=#37D946>");
				break;
		}

		sb.Append("<b> [").Append(msg).Append("] </b></color>");

		Debug.Log(sb.ToString());
	}
}
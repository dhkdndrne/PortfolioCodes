using System.Text;
using UnityEngine;

	public static class UtilClass
	{
		//StringBuilder 꼭 초기화 하고 사용
		private static StringBuilder sb = new();
		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void DebugLog(object msg, Define.LogType logType = Define.LogType.Log)
		{
			sb.Clear();

			switch (logType)
			{
				case Define.LogType.Log:
					sb.Append($"<color=#C8C8C8>");
					break;

				case Define.LogType.LogError:
					sb.Append($"<color=#a52a2aff>");
					break;

				case Define.LogType.Warning:
					sb.Append($"<color=#C18E2B>");
					break;
			}

			sb.Append("<b> [").Append(msg).Append("] </b></color>");

			Debug.Log(sb.ToString());
		}
	}

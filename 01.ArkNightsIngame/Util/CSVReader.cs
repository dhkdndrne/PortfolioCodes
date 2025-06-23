using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using Cysharp.Threading.Tasks;

public class CSVReader
{
	static readonly string SPLIT_REGEX = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))"; // 문자열 나눌 기준 정규 표현식
	static readonly string LINE_SPLIT_REGEX = @"\r\n|\n\r|\n|\r";                   // 줄 바꿈 기준 정규 표현식
	static readonly char[] TRIM_CHARS = { '\"' };                                   // 글자 분리 문자

	/// <summary>
	/// CSV 파싱
	/// </summary>
	/// <param name="_data">텍스트 에셋</param>
	/// <returns></returns>
	public static async UniTask<Dictionary<string, Dictionary<string, object>>> Read(string _file)
	{
		var dic = new Dictionary<string, Dictionary<string, object>>();
		TextAsset data = Resources.Load(_file) as TextAsset;

		var lines = Regex.Split(data.text, LINE_SPLIT_REGEX);

		if (lines.Length <= 1) return dic;

		var header = Regex.Split(lines[0], SPLIT_REGEX);
		for (var i = 1; i < lines.Length; i++)
		{
			var values = Regex.Split(lines[i], SPLIT_REGEX);
			if (values.Length == 0 || String.IsNullOrEmpty(values[0])) continue;

			var entry = new Dictionary<string, object>();
			for (var j = 1; j < header.Length && j < values.Length; j++)
			{
				string value = values[j];
				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

				object finalValue = value;
				if (int.TryParse(value, out int n))
				{
					finalValue = n;
				}
				else if (float.TryParse(value, out float f))
				{
					finalValue = f;
				}
				entry[header[j]] = finalValue;
			}
			dic.Add(values[0], entry);
		}
		return dic;
	}

	public static async UniTask<Dictionary<string, Dictionary<string, List<object>>>> ReadToDicList(string _file)
	{
		var dic = new Dictionary<string, Dictionary<string, List<object>>>();
		TextAsset data = Resources.Load(_file) as TextAsset;

		var lines = Regex.Split(data.text, LINE_SPLIT_REGEX);

		if (lines.Length <= 1) return dic;

		var header = Regex.Split(lines[0], SPLIT_REGEX);
		for (var i = 1; i < lines.Length; i++)
		{
			var values = Regex.Split(lines[i], SPLIT_REGEX);
			if (values.Length == 0 || String.IsNullOrEmpty(values[0])) continue;

			var entry = new Dictionary<string, List<object>>();
			for (var j = 1; j < header.Length && j < values.Length; j++)
			{
				string[] subKeys = values[j].Split(',');
				foreach (string subKey in subKeys)
				{
					string value = subKey.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

					object finalValue = value;
					if (int.TryParse(value, out int n))
					{
						finalValue = n;
					}
					else if (float.TryParse(value, out float f))
					{
						finalValue = f;
					}
					if (!entry.ContainsKey(header[j]))
					{
						entry[header[j]] = new List<object>();
					}
					entry[header[j]].Add(finalValue);
				}
			}
			dic.Add(values[0], entry);
		}
		return dic;
	}

}
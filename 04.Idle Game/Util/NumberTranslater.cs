using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using UnityEngine;
using Bam.Extensions;
using Unity.VisualScripting;

public static class NumberTranslater
{
	private static int numberLength = 27; //빈칸이 있어서 + 1
	private static int startIndex = 1;

	//처음에 저장되는 알파벳
	private static List<string> numberUnitList = new List<string>
	{
		"",
		"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
	};

	private static StringBuilder sb = new StringBuilder();

	public static string TranslateNumber(this BigInteger number)
	{
		sb.Clear();

		string zero = "0";

		//  패턴을 단순화 시키기 위해 무조건 지수 표현식으로 변경한 후 처리
		string[] partsSplit = number.ToString("E").Split('+');

		//  예외
		if (partsSplit.Length < 2)
		{
			return zero;
		}

		TranslateNumberInternal(number, partsSplit);

		return sb.ToString();
	}
	public static string TranslateNumber(this double number)
	{
		sb.Clear();

		string zero = "0";

		//  패턴을 단순화 시키기 위해 무조건 지수 표현식으로 변경한 후 처리
		string[] partsSplit = number.ToString("E").Split('+');

		//  예외
		if (partsSplit.Length < 2)
		{
			return zero;
		}
		TranslateNumberInternal(Math.Truncate(number), partsSplit);

        return sb.ToString();
	}

	private static void TranslateNumberInternal<T>(T number, string[] partsSplit)
	{
		//  지수 (자릿수 표현)
		if (!int.TryParse(partsSplit[1], out int exponent))
		{
			Debug.LogWarningFormat("Failed - ToCurrentString({0}) : partSplit[1] = {1}", number, partsSplit[1]);
			sb.Append("0");
			return;
		}

		//  몫은 문자열 인덱스
		int quotient = exponent / 3;

		//  나머지는 정수부 자릿수 계산에 사용(10의 거듭제곱을 사용)
		int remainder = exponent % 3;

		sb.Clear();
		//  1A 미만은 그냥 표현
		if (exponent < 3)
		{
			sb.Append(number);
		}
		else
		{
			//  10의 거듭제곱을 구해서 자릿수 표현값을 만들어 준다.
			var temp = double.Parse(partsSplit[0].Replace("E", "")) * Extensions.Pow(10, remainder);

			//  소수 둘째자리까지만 출력한다.
			if (temp < 10)
			{
				sb.Append(temp.ToString("F2"));
			}
			else if (temp < 100)
			{
				sb.Append(temp.ToString("F1"));
			}
			else
			{
				sb.Append(temp.ToString("0"));
			}
		}

		//몫이 자릿수보다 같거나 작을때까지 반복
		while(quotient > numberLength - 1)
		{
			AddNumberUnit();
		}

		sb.Append(numberUnitList[quotient]);
	}

	/// <summary>
	/// 새로운 단위 리스트에 추가
	/// </summary>
	public static void AddNumberUnit()
	{
		char startChar = (char)('A' - 1 + startIndex);

		for (int i = 1; i <= 26; i++)
		{
			string s = startChar + numberUnitList[i];
			numberUnitList.Add(s);
		}

		numberLength += 26;
		startIndex = startIndex + 1 > 26 ? startIndex = 1 : startIndex + 1;
	}

	public static double TranslateStringToDouble(string number)
	{
		sb.Clear();
        
		for (int i = number.Length - 1; i > 0; i--)
		{
			if (char.IsLetter(number[i]))
			{
				sb.Append(number[i]);
			}
		}
		
		if(sb.Length >0)
			number = number.Replace(sb.ToString()," ");
        
		int unitIndex = numberUnitList.IndexOf(sb.ToString());
        
		double num = Convert.ToDouble(number);

		num *= Extensions.Pow((double)1000, unitIndex);
		return num;
	}
}
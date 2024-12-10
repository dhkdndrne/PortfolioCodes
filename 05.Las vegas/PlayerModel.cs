using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[System.Serializable]
public class PlayerModel
{
	/*
	 *	두명이서 플레이하면 중립주사위 4개
	 *	3~4명이서 게임하면 중립 주사위 2개씩
	 *	3명이서 플레이하면 중립주사위 2개는 눈금에 맞는 카지노에 넣음
	 */

	public int PlayerNumber { get; private set; }
	
	public ReactiveProperty<int> Dice { get; private set; } = new();
	public ReactiveProperty<int> SpecialDice { get; private set; } = new();

	public ReactiveProperty<int> Money { get; private set; } = new();
	public ReactiveProperty<bool> IsMyTurn { get; private set; } = new();
	public ReactiveProperty<bool> IsBettingTime { get; private set; } = new();

	public void InitModel(int playerNumber,int diceAmount,int specialDiceAmount)
	{
		PlayerNumber = playerNumber;
		Dice.Value = diceAmount;
		SpecialDice.Value = specialDiceAmount;
	}

	public void UpdateDiceAmount(int diceAmount, int sDiceAmount)
	{
		Dice.Value -= diceAmount;
		SpecialDice.Value -= sDiceAmount;
	}

	public bool CheckHasDice()
	{
		return Dice.Value != 0 || SpecialDice.Value != 0;
	}
}
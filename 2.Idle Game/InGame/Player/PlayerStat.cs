using System.Numerics;
using Bam.Extensions;
using UniRx;

public class PlayerStat
{
	public ReactiveProperty<int> level = new();
	public ReactiveProperty<double> effect = new();
	public ReactiveProperty<BigInteger> upgradeCost = new();
    
	private double originEffect;
	private float effectIncrease;
	private float costIncrease;
	public int MaxLevel { get; private set; }

	private bool isPlus;
	
	public PlayerStat(int level, int maxLevel, float startEffect, float effectIncrease, int startCost, float costIncrease,bool isPlus)
	{
		this.level.Value = level;

		MaxLevel = maxLevel;
		this.effectIncrease = effectIncrease;
		this.costIncrease = costIncrease;

		originEffect = startEffect;
		this.isPlus = isPlus;
		
		effect.Value = isPlus ? effectIncrease * level : originEffect + (effectIncrease * level);
		upgradeCost.Value = (BigInteger)(startCost + ((level - 1) * costIncrease));
	}

	public void Upgrade()
	{
		if (level.Value == MaxLevel)
			return;

		if (Player.Instance.Currency.gold.Value < upgradeCost.Value)
			return;

		Player.Instance.Currency.DecreaseGold(upgradeCost.Value);

		level.Value++;
		effect.Value = isPlus ? effectIncrease * level.Value : originEffect + (effectIncrease * level.Value);
		upgradeCost.Value += new BigInteger(level.Value * costIncrease);
	}
}
using TMPro;
using UniRx;
using UnityEngine;

public class StatText : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI statText;
	[SerializeField] private TextMeshProUGUI statValueText;

	public void Init(AbilityType abilityType)
	{
		statText.text = UtilClass.AbilityTypeToString(abilityType);

		if (abilityType is AbilityType.Damage)
			statText.text += "%";
		
		var property = PlayerData.Instance.GetAbilityProperty(abilityType);
		property.Subscribe(value =>
		{
			statValueText.text = value.ToString();
			ChangeColor(value);
		}).AddTo(this);
	}

	/// <summary>
	/// 0미만 (빨강) 0(하양) 0이상(초록)
	/// </summary>
	/// <param name="val"></param>
	public void ChangeColor(int val)
	{
		Color color = val switch
		{
			0 => Color.white,
			> 0 => Color.green,
			< 0 => Color.red
		};

		statText.color = color;
		statValueText.color = color;
	}
}
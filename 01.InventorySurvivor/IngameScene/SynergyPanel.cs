using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UtilClass;

public class SynergyPanel : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI[] texts;

	private RectTransform rt;
	private StringBuilder sb;

	private void Awake()
	{
		rt = GetComponent<RectTransform>();
		sb = new StringBuilder();
	}

	public void UpdateUI(AttackItemSo item)
	{
		for (int i = 0; i < texts.Length; i++)
			texts[i].gameObject.SetActive(false);

		var colorManager = ColorManager.Instance;
		var idList = item.synergyIdList;

		for (int i = 0; i < idList.Count; i++)
		{
			texts[i].gameObject.SetActive(true);

			sb.Clear();

			if (i != 0)
				sb.AppendLine();

			var synergy = SynergyManager.Instance.GetSynergy(idList[i]);
			var buff = synergy.Buffs;

			//시너지 이름
			sb.Append($"<color=#{colorManager.HEX_SYNERGYNAME}>{synergy.Name}</color>");

			for (int j = 0; j < synergy.Conditions.Length; j++)
			{
				int condition = synergy.Conditions[j];
				string appliedColor = j == synergy.AppliedGrade ? colorManager.HEX_GREEN : colorManager.HEX_BLACK;
				
				sb.AppendLine();
				sb.Append($"<color=#{appliedColor}>({condition})</color>");

				for (int k = 0; k < buff.Length; k++)
				{
					bool isPlus = buff[k].Value[j] > 0;
					string plusSign = isPlus ? "+" : "";
					
					if (k is 0) sb.Append(" ");
					else sb.Append("     ");

					appliedColor = buff[k].Value[j] < 0 ? colorManager.HEX_RED : appliedColor;
					
					sb.Append($"<color=#{appliedColor}>{plusSign}{buff[k].Value[j]} {AbilityTypeToString(buff[k].AbilityType)}</color>");
					sb.AppendLine();
				}
			}
			//조건 및 능력
			texts[i].text = sb.ToString();
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
	}
}
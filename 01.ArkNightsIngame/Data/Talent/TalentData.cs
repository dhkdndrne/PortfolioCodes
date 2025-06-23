using System.Collections.Generic;
using UnityEngine;

public enum TalentTrigger { OnDeploy, OnAttack, onSquadDeployment }

[CreateAssetMenu(fileName = "TalentData", menuName = "Scriptable Objects/Talent Data")]
public class TalentData : ScriptableObject
{
	[SerializeField] private string displayName;
	[TextArea,SerializeField] private string description;
	
	[SerializeField] private TalentTrigger trigger;
	[SerializeField] private List<EffectToken> effects;
	
	public string DisplayName => displayName;
	public string Description => description;
	public TalentTrigger Trigger => trigger;
	public IReadOnlyList<EffectToken> Effects => effects;
}

[System.Serializable]
public class EffectToken
{
	public AttributeType type;
	public float value;
}

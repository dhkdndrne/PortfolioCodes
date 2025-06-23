using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class ExusiaiTalent2Handler : TalentHandler
{
	private Operator owner;
	private int maxTarget;
	private float hpBuff;
	private float damageBuff;
	
	private class BuffInfo
	{
		public AttributeModifier DmgMod;
		public AttributeModifier HpMod;
		public Action DeathHandler;
	}
	private readonly Dictionary<Operator, BuffInfo> buffedOperators
		= new Dictionary<Operator, BuffInfo>();
	
	public override void Initialize(Operator op)
	{
		var effects = data.Effects;
		
		owner = op;
		maxTarget = (int)effects.FirstOrDefault(x => x.type == AttributeType.Target).value;
		damageBuff = effects.FirstOrDefault(x => x.type == AttributeType.Damage).value;
		hpBuff = effects.FirstOrDefault(x => x.type == AttributeType.MaxHp).value;
		
		owner.onDeploy += HandleDeploy;
		owner.OnDeath += HandleExusiaiDeath;
	}

	private void HandleDeploy()
	{
		ApplyBuff(owner); // Exusiai 자신
		TryApplyBuffToExistingOperators();
		GameManager.Instance.OperatorManager.OnOperatorDeployed += OnOtherOperatorDeployed;
	}

	private void TryApplyBuffToExistingOperators()
	{
		int remaining = maxTarget - (buffedOperators.Count - 1); // 자기 자신꺼는 빼고
		if (remaining <= 0) return;

		var candidates = GameManager.Instance.OperatorManager
			.GetDeployedOperators()
			.Where(o => o != owner && !buffedOperators.ContainsKey(o))
			.OrderBy(_ => Random.value)
			.Take(remaining);

		foreach (var op in candidates)
			ApplyBuff(op);
	}

	private void OnOtherOperatorDeployed(Operator newOp)
	{
		if (buffedOperators.Count - 1 >= maxTarget) return;
		if (newOp == owner || buffedOperators.ContainsKey(newOp)) return;
		ApplyBuff(newOp);
	}

	private void ApplyBuff(Operator op)
	{
		if (op == null) return;

		// 1) Modifier 인스턴스 각각 생성
		var dmgMod = new AttributeModifier(
			AttributeType.Damage,
			damageBuff,
			AttributeModifierType.Add
		);
		var hpMod = new AttributeModifier(
			AttributeType.MaxHp,
			hpBuff,
			AttributeModifierType.Add
		);

		// 2) 적용
		op.SetAdditionalAttribute(dmgMod);
		op.SetAdditionalAttribute(hpMod);

		// 3) 사망 시 롤백 핸들러
		Action deathHandler = () => RemoveBuff(op);
		op.OnDeath += deathHandler;

		// 4) 사전 보관
		buffedOperators[op] = new BuffInfo
		{
			DmgMod = dmgMod,
			HpMod = hpMod,
			DeathHandler = deathHandler
		};
	}

	private void RemoveBuff(Operator op)
	{
		if (!buffedOperators.TryGetValue(op, out var info)) return;
		
		op.RemoveAdditionalAttribute(info.DmgMod);
		op.RemoveAdditionalAttribute(info.HpMod);
		op.OnDeath -= info.DeathHandler;

		buffedOperators.Remove(op);
	}

	private void HandleExusiaiDeath()
	{
		// 자신 버프 삭제
		RemoveBuff(owner);

		// 아군 전원 버프 삭제
		foreach (var op in buffedOperators.Keys.ToList())
			RemoveBuff(op);
		
		owner.onDeploy -= HandleDeploy;
		owner.OnDeath -= HandleExusiaiDeath;
		GameManager.Instance.OperatorManager.OnOperatorDeployed -= OnOtherOperatorDeployed;
	}
}
using System.Collections.Generic;
public abstract class Skill
{
    protected SpChargeType ChargeType;
    protected SkillActiveType activeType;
    
    protected int id;
    protected int index;
    protected string skillName;
    protected bool isActivating;
    protected int requireSp;
    protected int initialSp;
    protected string description;
    
    protected float elapsedTime;
    protected float duration;
    protected bool isEnd;
    protected Dictionary<AttributeType, float> deltaValueDic;

    protected Operator op;
    
    public ObservableValue<float> Sp { get; protected set; } = new ObservableValue<float>(0);
    public ObservableValue<float> SpRatio { get; } = new ObservableValue<float>(1);
    public ObservableValue<float> DurationRatio { get; protected set; } = new ObservableValue<float>(1);

    public float Duration => duration;
    public string SkillName => skillName;
    public int RequiredSp => requireSp;
    public bool IsActivating => isActivating;
    public int Index => index;
    public string Description => description;
    public SpChargeType SpChargeType => ChargeType;
    public SkillActiveType ActiveType => activeType;

    public void Init(Operator op, int index)
    {
        this.op = op;
        var opData = DataManager.Instance.OperatorData.GetOperatorData((int)op.OperatorID);
        this.index = index;
        id = opData.SkillIDs[index];

        var data = DataManager.Instance.OperatorData.GetOperatorSkillData(id);
        skillName = data.Name;

        // 공통 초기화
        var skillData = data.GetSkillLevelData(10);
        
        initialSp = skillData.Initial_Sp;
        requireSp = skillData.Sp_cost;
        ChargeType = skillData.SpChargeType;
        activeType = skillData.ActiveType;
        duration = skillData.Duration;
        description = skillData.Description;
        
        Sp.Subscribe(val =>
        {
            SpRatio.Value = val / requireSp;
        });

        op.onDeploy += () =>
        {
            Sp.Value = initialSp;
            isActivating = false;
        };
        
        deltaValueDic = new Dictionary<AttributeType, float>();
        foreach (var attribute in skillData.Attributes)
        {
            deltaValueDic.TryAdd(attribute.AttributeType, attribute.Value);
        }
        deltaValueDic.TryAdd(AttributeType.AtkSpeed, 0);

        // 하위 클래스에서 추가 초기화가 필요한 경우 오버라이드하여 처리
        PostInit(op, index);
    }

    //추가 초기화 작업을 이곳에 구현
    protected virtual void PostInit(Operator op, int index){}
    
    public virtual void SetSkill()
    {
        Sp.Value = 0;
        op.Controller.SetAnimationSpeed(deltaValueDic[AttributeType.AtkSpeed]);
    }

    public void ResetSp() => Sp.Value = initialSp;
    public virtual bool CanUse() => Sp.Value >= requireSp;
    public abstract void Use(Operator caster, HashSet<Unit> targetList);
    public abstract bool IsSkillEnd();
    public abstract bool UpdateSkill(HashSet<Unit> targetList);

    public virtual void EndSkillAnim() { }
    public virtual void ChargeSP(float value)
    {
        if (isActivating || Sp.Value >= requireSp)
            return;
        Sp.Value += value;
        if (Sp.Value >= requireSp)
            Sp.Value = requireSp;
    }
}

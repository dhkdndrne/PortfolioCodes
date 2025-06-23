public abstract class Buff
{
	public AttributeType Attribute { get; protected set; }
	public float Duration { get; protected set; }

	public float ElapsedTime { get; private set; }

	public bool IsExpired => ElapsedTime >= Duration; // 버프가 만료되었는지

	public bool IsGroup { get; private set; }    // 버프 그룹에 속해있는지
	public bool IsInfinite { get; private set; } // 무한히 지속되는 버프인지

	public Buff(float duration, bool isGroup)
	{
		IsGroup = isGroup;
		Duration = duration;

		if (duration < 0)
			IsInfinite = true;
	}

	public virtual void Update(float deltaTime)
	{
		ElapsedTime += deltaTime;
	}

	public abstract void Apply(Unit target);

	public abstract void Remove(Unit target);

}
public class State_Idle: State
{
	public override void BeginState()
	{

	}
	
	public override void UpdateState()
	{
		if (myAI.CheckTargetInAttackRange())
		{
			ChangeState<State_Attack>();
		}
		else
		{
			ChangeState<State_Move>();
		}
	}
	
	public override void EndState()
	{

	}
}

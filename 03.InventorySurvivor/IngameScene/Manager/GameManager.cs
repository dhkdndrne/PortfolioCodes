using UniRx;
using UnityEngine;

public class GameManager : Manager<GameManager>
{
	private SpawnSystem spawnSystem;
	[SerializeField] private CameraSwitchSystem cameraSwitchSystem = new CameraSwitchSystem();
	
	public ReactiveProperty<GameStep> Step { get; private set; }

	public override void OnStartManager()
	{
		spawnSystem = new SpawnSystem();
		Step = new ReactiveProperty<GameStep>();
		
		cameraSwitchSystem.Init();
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
			ChangeStep(GameStep.UnLockSlot);
	
		if (Input.GetKeyDown(KeyCode.E))
		{
			PlayerData.Instance.Gold.Value += 10000;
		}

		Inventory.Instance.UnLockCnt.Where(cnt => cnt == 0 && Step.Value is GameStep.UnLockSlot).Subscribe(_ =>
		{
			ChangeStep(GameStep.InventoryArrange);
		}).AddTo(this);

		if (Input.GetKeyDown(KeyCode.Keypad0))
		{
			PlayerData.Instance.UpdateExp(10);
		}
	}

	public void StartGame()
	{
		if (Inventory.Instance.UnLockCnt.Value > 0)
			return;
		
		ChangeStep(GameStep.Playing);
		spawnSystem.SpawnEnemy();
	}
	private void ChangeStep(GameStep step) =>Step.Value = step;
}
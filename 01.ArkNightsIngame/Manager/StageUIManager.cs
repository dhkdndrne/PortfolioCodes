using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour
{
	[Header("상단 UI")]
	[SerializeField] private TextMeshProUGUI killCountText;
	[SerializeField] private TextMeshProUGUI lifePointText;

	[Header("코스트")]
	[SerializeField] private TextMeshProUGUI costText;
	[SerializeField] private TextMeshProUGUI canDeployAmountText;
	[SerializeField] private Slider costSlider;
	[SerializeField] private GameObject maxCostImage;

	[Header("배속")]
	[SerializeField] private Button speedButton;
	[SerializeField] private Sprite[] speedSprites;

	[Header("일시정지, 플레이")]
	[SerializeField] private Button playButton;
	[SerializeField] private Sprite[] playPauseSprite;
	[SerializeField] private GameObject pausePanel;
	
	private void Start()
	{
		var timeManager = TimeManager.Instance;
		int pause = 0;
		playButton.onClick.AddListener(() =>
		{
			pause = pause == 0 ? 1 : 0;
			playButton.image.sprite = playPauseSprite[pause];

			pausePanel.SetActive(pause == 1);

			if (pause == 1) timeManager.Stop();
			else timeManager.Resume();
		});

		int speed = 1;
		speedButton.onClick.AddListener(() =>
		{
			if (pause == 1)
				return;
			
			speed = speed == 1 ? 2 : 1;
			speedButton.image.sprite = speedSprites[speed];
			
			timeManager.SetTimeScale(speed * 0.5f);
		});
		
		timeManager.OnTimeScaleChanged += timeScale =>
		{
			bool lowSpeed = timeScale < 0.5f;
			speedButton.interactable = !lowSpeed;
			
			if (AutoBattleManager.Instance.IsReplayMode.Value) return;
			if (lowSpeed) speedButton.image.sprite = speedSprites[0];
			else speedButton.image.sprite= speedSprites[(int)(timeScale * 2)];
		};
	}
	
	public void SubscribeEvent(ObservableValue<int> life,
		ObservableValue<int> cost,
		int maxCost, ObservableValue<int> charLimit,
		ObservableValue<int> killCount,
		int totalEnemyCount,
		ObservableValue<float> sliderValue)
	{
		life.Subscribe(val =>
		{
			lifePointText.text = val.ToString();
		});

		bool isCostMaxActivated = false;
		cost.Subscribe(val =>
		{
			costText.text = val.ToString();
			bool isMax = val == maxCost;
			if (isCostMaxActivated != isMax)
			{
				isCostMaxActivated = isMax;
				maxCostImage.SetActive(isMax);
			}
		});

		charLimit.Subscribe(val =>
		{
			canDeployAmountText.text = $"배치 가능 인원 : {val.ToString()}";
		});

		sliderValue.Subscribe(val =>
		{
			costSlider.value = val;
		});

		//todo 나중에 전체 적 유닛수도 표시해야함
		killCount.Subscribe(val =>
		{
			killCountText.text = $"{val} / {totalEnemyCount}";
		});
	}
}
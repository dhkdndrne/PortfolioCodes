using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Bam.Extensions;

public class OperatorSlot_UI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	[SerializeField] private Image characterImage;
	[SerializeField] private Image classImage;
	[SerializeField] private TextMeshProUGUI costText;
	[SerializeField] private GameObject shadow;
	[SerializeField] private GameObject border;

	[FormerlySerializedAs("coolTimePanel")]
	[Header("재배치 쿨타임 UI")]
	[SerializeField] private GameObject respawnPanel;
	[SerializeField] private Image respawnSlider;
	[SerializeField] private TextMeshProUGUI respawnText;

	public event Action<int> OnSlotSelect;
	public event Action<PointerEventData> OnSlotDragStart;
	public event Action OnSlotDragging;
	public event Action OnSlotDragEnd;

	private int slotIndex;
	private RectTransform rt;
	private bool isSelected;
	private bool isDragging;
	private bool isRespawning;
	private bool canDeploy;
	private float originYPosition;

	public RectTransform Rt
	{
		get
		{
			if (rt == null)
				rt = GetComponent<RectTransform>();

			return rt;
		}
	}
	private void Start()
	{
		originYPosition = rt.anchoredPosition.y;
	}

	private void OnEnable()
	{
		isSelected = false;
		border.SetActive(false);
	}

	public void Init(int index, Operator op)
	{
		slotIndex = index;
		op.Cost.Subscribe(val =>
		{
			costText.text = val.ToString();
		});
		
		characterImage.sprite = ImageManager.Instance.operatorImageDic[op.OperatorID].GetFaceIcon(0);
		classImage.sprite = ImageManager.Instance.battle_ClassSpriteDic[op.OperatorClass].battleCardClassIcon;
		
		var gameManager = GameManager.Instance;

		ObservableValue<bool> canDeployCombined = Extensions.CombineLatest(
			gameManager.Stage.Cost,
			gameManager.Stage.CharacterLimit,
			(cost, characterLimit) => op.Cost.Value <= cost && characterLimit > 0
		);

		canDeployCombined.Subscribe(result =>
		{
			canDeploy = result;
			shadow.gameObject.SetActive(!canDeploy);
		});

		AutoBattleManager.Instance.OnDeployAction += id =>
		{
			if (id == op.OperatorID)
				gameObject.SetActive(false);
		};
	}

	private void SelectSlot()
	{
		isSelected = true;
		border.SetActive(true);
		MoveSlot(true).Forget();
	}

	public void DeselectSlot()
	{
		isSelected = false;
		border.SetActive(false);
		MoveSlot(false).Forget();
	}

	private async UniTaskVoid MoveSlot(bool up)
	{
		rt.DOKill();
		var val = up ? 30 : 0;
		await rt.DOLocalMoveY(originYPosition + val, 0.1f);
	}

	public void StartUpdateRespawnTime(float coolTime)
	{
		respawnPanel.SetActive(true);
		UpdateRespawnTime(coolTime).Forget();
	}

	private async UniTaskVoid UpdateRespawnTime(float coolTime)
	{
		isRespawning = true;
		float timer = coolTime;
		respawnSlider.fillAmount = 0;

		while (timer > 0)
		{
			timer -= Time.deltaTime;   // 시간 감소
			if (timer <= 0) timer = 0; // 음수 방지

			float elapsedTime = coolTime - timer;      // 경과 시간 계산
			float sliderRate = elapsedTime / coolTime; // 경과 시간에 따른 비율 계산
			respawnSlider.fillAmount = sliderRate;     // 게이지 바 업데이트

			respawnText.text = timer.ToString("0.0");
			await UniTask.Yield();
		}
		isRespawning = false;
		respawnPanel.SetActive(false);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (isDragging) return;

		SelectSlot();
		OnSlotSelect?.Invoke(slotIndex);
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
		if (AutoBattleManager.Instance.IsReplayMode.Value)
		{
			eventData.pointerDrag = null;
			return;
		}
		
		if (!isSelected)
		{
			SelectSlot();
			OnSlotSelect?.Invoke(slotIndex);
		}

		if (isRespawning || !canDeploy)
		{
			eventData.pointerDrag = null;
			return;
		}

		isDragging = true;
		OnSlotDragStart?.Invoke(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		isDragging = false;
		OnSlotDragEnd?.Invoke();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}

	public void OnDrag(PointerEventData eventData)
	{
		OnSlotDragging?.Invoke();
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using Bam.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 슬롯에서의 오퍼레이터 드래그 앤 드랍 관리하는 클래스
/// </summary>
public class OperatorSlotController : MonoBehaviour
{
	[SerializeField] private List<OperatorSlot_UI> slots;

	[SerializeField] private float maxSize = 250;
	[SerializeField] private int slotSize = 10;
	[SerializeField] private OperatorDirectionUI operatorDirectionUI;

	private OperatorManager operatorManager;
	private OperatorDragController operatorDragController;

	private Operator selectedOperator; //선택된 오퍼레이터 오브젝트
	private OperatorSlot_UI selectedSlot;

	private const float DRAG_HEIGHT = 2f;

	private void Awake()
	{
		operatorDragController = GetComponent<OperatorDragController>();
	}

	public void Init(OperatorManager operatorManager)
	{
		this.operatorManager = operatorManager;
		InitEvent();
	}

	private void InitEvent()
	{
		var tileManager = GameManager.Instance.TileManager;
		var selectedOperatorUI = FindFirstObjectByType<SelectedOperatorUI>();
		var operatorInfoPanel = FindFirstObjectByType<OperatorInfoPanel>();
		
		// 전체 슬롯을 비활성화
		foreach (var slot in slots)
		{
			slot.gameObject.SetActive(false);
		}

		int index = 0;
		foreach (var op in operatorManager.GetAllOperators())
		{
			slots[index].gameObject.SetActive(true);
			slots[index].Init(index, op);

			slots[index].OnSlotSelect += slotIndex =>
			{
				// 누른슬롯이 이미 눌린 슬롯일때
				if (selectedSlot == slots[slotIndex])
				{
					ResetSelectedSlot();
					ChangeSlotSize();

					selectedOperator = null;

					//배치 가능타일 애니메이션 취소
					tileManager.DisablePlaceableTile();

					//유닛 상세정보 UI닫기
					operatorInfoPanel.Close();
					TimeManager.Instance.StopSlowMotion();
					return;
				}

				if (selectedSlot != null)
				{
					tileManager.DisablePlaceableTile();
					selectedSlot.DeselectSlot(); // 기존 선택된 슬롯 해제
				}

				selectedSlot = slots[slotIndex];
				selectedOperator = op;
				operatorInfoPanel.Show(op);
				tileManager.ShowPlaceableTile(selectedOperator);
				TimeManager.Instance.StartSlowMotion();
			};

			slots[index].OnSlotDragStart += val =>
			{
				if (AutoBattleManager.Instance.IsReplayMode.Value)
					return;
				
				HandleSlotDragStart(val);
				selectedOperatorUI.Close();
			};
			slots[index].OnSlotDragging += HandleSlotDragging;
			slots[index].OnSlotDragEnd += () =>
			{
				Action closeAction = operatorInfoPanel.Close;
				HandleSlotDragEnd(closeAction);
				tileManager.DisablePlaceableTile();
				AttackRangeIndicator.Instance.DisableAttackRange();
			};

			index++;
		}

		//슬롯 크기 변경
		ChangeSlotSize();

		operatorDirectionUI.OnChangedDirection += dir =>
		{
			var operatorSpriteHandler = selectedOperator.GetComponent<OperatorUIHandler>();
			if (dir == OperatorDirection.None)
			{
				operatorSpriteHandler?.SetDirectionArrowActivate(false);
				AttackRangeIndicator.Instance.DisableAttackRange();
			}
			else
			{
				operatorSpriteHandler?.SetDirectionArrowActivate(true);
				selectedOperator.Controller.Body.transform.rotation = Quaternion.Euler(0, ((int)dir) * 90, 0);
				selectedOperator.SetAttackDir(dir);
				AttackRangeIndicator.Instance.ShowUndeployedOperatorAttackRange(selectedOperator,operatorDragController.CurTile,dir);
			}
		};

		operatorDirectionUI.OnEndDragAction += isSuccess =>
		{
			if (isSuccess)
			{
				operatorDragController.CurTile?.SetOperator(selectedOperator);
				selectedOperator.Controller.DeployOperator();
				selectedSlot?.gameObject.SetActive(false);
				selectedSlot = null;
			}
			else //배치 취소했을때
			{
				operatorDragController.DisableObject();
				ResetSelectedSlot();
			}
			AttackRangeIndicator.Instance.DisableAttackRange();
			operatorInfoPanel.Close();
		};

		ClickChecker.Instance.onOperatorClicked += op =>
		{
			if (op is null)
			{
				if (selectedSlot != null)
				{
					selectedSlot.DeselectSlot();
					tileManager.DisablePlaceableTile();
				}
			}
		};
	}

	private void HandleSlotDragStart(PointerEventData eventData)
	{
		Vector3 startPosition = Camera.main.ScreenToWorldPoint(eventData.position);
		startPosition.z = 0;
		selectedOperator.SetActive(true);
		operatorDragController.StartDrag(selectedOperator, startPosition);

		selectedOperator.GetComponent<OperatorUIHandler>().SetDirectionArrowActivate(false);
	}

	private void HandleSlotDragging()
	{
		Vector3 objPos = Vector3.zero;
		float distance = Camera.main.WorldToScreenPoint(selectedOperator.transform.position).z;

#if UNITY_EDITOR || UNITY_STANDALONE
		// 데스크탑: 마우스 이동이 있을 때만 업데이트
		if (Input.GetAxis("Mouse X") == 0f && Input.GetAxis("Mouse Y") == 0f)
			return;

		Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
		objPos = Camera.main.ScreenToWorldPoint(mousePos);
#else
    // 모바일: 터치가 있고, 터치가 이동 중일 때만 업데이트
    if (Input.touchCount == 0)
        return;

    Touch touch = Input.GetTouch(0);
    if (touch.phase != TouchPhase.Moved)
        return;

    Vector3 touchPos = new Vector3(touch.position.x, touch.position.y, distance);
    objPos = Camera.main.ScreenToWorldPoint(touchPos);
#endif

		objPos.y = DRAG_HEIGHT;
		operatorDragController.UpdateDrag(objPos);
	}

	private void HandleSlotDragEnd(Action infoPanelAction)
	{
		// 적합한 타일이 아닌 다른곳에 올렸을때
		if (operatorDragController.CheckTileIsNull())
		{
			ResetSelectedSlot();
			infoPanelAction.Invoke();
			TimeManager.Instance.StopSlowMotion();
		}
		else // 오퍼레이터를 적합한 타일위에 올렸을때
		{
			selectedOperator.GetComponent<OperatorUIHandler>().SetDirectionArrowActivate(true);
			operatorDirectionUI.SetPositions(Camera.main.WorldToScreenPoint(selectedOperator.transform.position));
		}
	}

	// todo 나중에 수정
	private void ChangeSlotSize()
	{
		int activeCount = slots.Count(x => x.gameObject.activeInHierarchy);
		float size = maxSize; // 기본 슬롯 크기

		// 슬롯의 총 너비 계산 (슬롯 크기 + 간격 포함)
		float totalWidth = activeCount * size + (activeCount - 1);

		// 조건: 슬롯이 화면 너비를 초과하면 크기를 줄임
		if (totalWidth > Screen.width)
		{
			size = (Screen.width - (activeCount - 1)) / (float)activeCount; // 크기 조정
		}

		// 시작 위치를 오른쪽에서부터 계산
		float startX = (float)Screen.width / 2 - (size / 2);
		float currentX = startX;

		foreach (var slot in slots)
		{
			if (!slot.gameObject.activeInHierarchy) continue;

			RectTransform rt = slot.Rt;

			if (selectedSlot == slot)
			{
				selectedSlot.Rt.sizeDelta = new Vector2(maxSize, maxSize);
			}
			else rt.sizeDelta = new Vector2(size, size); // 슬롯 크기 설정

			//rt.anchoredPosition = new Vector2(currentX, 0); // 슬롯 위치 설정
			//currentX -= size; // 다음 슬롯으로 이동
		}
	}

	private void ResetSelectedSlot()
	{
		selectedSlot.DeselectSlot();
		selectedSlot = null;
		selectedOperator = null;
	}

	public void StartOperatorRespawn(int index, float respawnTime)
	{
		var slot = slots[index];

		slot.gameObject.SetActive(true);
		slot.StartUpdateRespawnTime(respawnTime);
	}
}
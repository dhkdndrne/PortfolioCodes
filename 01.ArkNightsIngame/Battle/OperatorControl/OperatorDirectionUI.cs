using System;
using Bam.Extensions;
using UnityEngine;

/// <summary>
/// 캐릭터를 타일에 배치 후 방향 및 관련 ui담당
/// handle의 정보를 바탕으로 ui표시
/// </summary>
public class OperatorDirectionUI : MonoBehaviour
{
	[SerializeField] private RectTransform maskParent;
	[SerializeField] private RectTransform secondSquare; // 유닛 방향 선택됐을때 표시할 모서리 UI
	[SerializeField] private GameObject[] edges;
	
	[SerializeField] private DirectionJoystick joystick;
	
	private GameObject currentEdge;

	public event Action<OperatorDirection> OnChangedDirection;
	public event Action<bool> OnEndDragAction;

	private void Init()
	{
		
		joystick.DRAG_ACTION += UpdateUI;
		
		joystick.END_DRAG_ACTION += hasDirection =>
		{
			if (hasDirection)
			{
				SetCurrentEdge(null);
				gameObject.SetActive(false);
				OnEndDragAction?.Invoke(true);
				TimeManager.Instance.StopSlowMotion();
			}
		};
	}
	private void Start()
	{
		Init();
	}

	private void OnEnable()
	{
		foreach (var edge in edges)
			edge.SetActive(false);
	}

	public void SetPositions(Vector2 pos)
	{
		gameObject.SetActive(true);
		maskParent.transform.position = pos;
		secondSquare.transform.position = pos;
	}
	
	private void UpdateUI(OperatorDirection dir, bool hasDir)
	{
		if (!hasDir)
		{
			OnChangedDirection?.Invoke(dir);
			SetCurrentEdge(null);
			return;
		}

		var newEdge = edges[(int)dir - 1];
		if (currentEdge != newEdge)
		{
			SetCurrentEdge(newEdge);
			OnChangedDirection?.Invoke(dir);
		}
	}

	private void SetCurrentEdge(GameObject edge)
	{
		currentEdge?.SetActive(false);
		currentEdge = edge;
		currentEdge?.SetActive(true);
	}

	public void Close()
	{
		OnChangedDirection?.Invoke(OperatorDirection.None);
		OnEndDragAction?.Invoke(false);
		SetCurrentEdge(null);
		gameObject.SetActive(false);
		TimeManager.Instance.StopSlowMotion();
	}
}
using System;
using Bam.Extensions;
using UnityEngine;

/// <summary>
/// 슬롯에서 드래그한 유닛의 오브젝트의 드래그를 관리하는 클래스
/// </summary>
public class OperatorDragController : MonoBehaviour
{
	[SerializeField] private LayerMask mask;

	private Operator dragOperator;
	private Tile curTile;

	public Tile CurTile => curTile;

	public void StartDrag(Operator draggingOperator, Vector3 startPosition)
	{
		if (draggingOperator == null)
		{
			Debug.LogWarning("DragPrefab이 설정되지 않았습니다.");
			return;
		}

		dragOperator = draggingOperator;
		dragOperator.SetActive(true);
		dragOperator.transform.position = startPosition;
	}

	public void UpdateDrag(Vector3 currentPosition)
	{
		if (dragOperator == null)
			return;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
		{
			dragOperator.transform.position = currentPosition;
			curTile = null;
			AttackRangeIndicator.Instance.DisableAttackRange();
			return;
		}

		GameObject hitObject = hit.collider.gameObject;
		Tile tile = hitObject.GetComponent<Tile>();

		// 같은 타일이면 리턴
		if (curTile != null && curTile == tile)
			return;
		
		// 오퍼레이터의 공격 유형에 따라 타일이 유효하지 않은지 확인
		// 그리고 배치 가능한 타일인지 체크
		bool invalidTile = !tile.TileType.HasFlag(TileType.Deployable) ||
		                   (dragOperator.AtkType == Operator_AtkType.Melee && tile.HeightType == HeightType.Highland) ||
		                   (dragOperator.AtkType == Operator_AtkType.Ranged && tile.HeightType == HeightType.Lowland);


		// 이미 유닛이 배치된 경우 드래그 객체의 위치를 업데이트
		if (invalidTile || tile.UnitOnTile != null)
		{
			dragOperator.transform.position = currentPosition;
			curTile = null;
			AttackRangeIndicator.Instance.DisableAttackRange();
			return;
		}

		curTile = tile;
		curTile.SetUnitPosition(dragOperator.transform);
		AttackRangeIndicator.Instance.ShowUndeployedOperatorAttackRange(dragOperator, curTile);
	}

	public bool CheckTileIsNull()
	{
		if (curTile == null)
		{
			DisableObject();
			return true;
		}

		return false;
	}

	public void DisableObject()
	{
		dragOperator.gameObject.SetActive(false);
		dragOperator = null;
	}
}
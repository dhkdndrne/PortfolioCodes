using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Bam.Extensions;

public class Tile : MonoBehaviour
{
	[SerializeField] private TileType tileType;
	[SerializeField] private HeightType heightType;
	[SerializeField] private GameObject attackRangeShader;
	
	public HeightType HeightType => heightType;
	public TileType TileType => tileType;

	private MeshRenderer meshRenderer;
	private MaterialPropertyBlock mpb;
	private bool isPlayingAnim;

	private Color originColor;
	private readonly Color ColorA = new Color32(33, 197, 21, 255);
	private readonly Color ColorB = new Color32(109, 255, 98, 255);


	public Vector2Int TileIndex { get; private set; }
	public Operator UnitOnTile { get; private set; }
	[field: SerializeField] public List<Enemy> Enemies { get; private set; }
	
	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		mpb = new MaterialPropertyBlock();
		Enemies = new List<Enemy>();
		
		originColor = meshRenderer.material.GetColor("_BaseColor");
	}
	public void SetAttackRangeShaderState(bool isActive) => attackRangeShader.gameObject.SetActive(isActive);

	public void SetOperator(Operator op)
	{
		UnitOnTile = op;
		op.SetTile(this);
	}

	public void SetUnitPosition(Transform op)
	{
		Vector3 targetPos = transform.position;

		targetPos.y = (transform.localScale.y * 0.5f) + 0.01f;

		op.position = targetPos;
	}

	public void RemoveUnit()
	{
		UnitOnTile = null;
	}

	public void SetTileIndex(int x, int y)
	{
		TileIndex = new Vector2Int(x, y);
	}

	public void StartPlaceableAnim()
	{
		StopCoroutine(nameof(ColorChangeAnim));
		Extensions.ChangeMeshColor(meshRenderer, mpb, ColorA);
		StartCoroutine(nameof(ColorChangeAnim));
	}

	public void StopPlaceableAnim()
	{
		StopCoroutine(nameof(ColorChangeAnim));
		Extensions.ChangeMeshColor(meshRenderer, mpb, originColor);
	}

	private IEnumerator ColorChangeAnim()
	{
		float t = 0;
		float duration = 1.5f;

		while (true)
		{
			Color currentColor = Color.Lerp(ColorA, ColorB, Mathf.PingPong(t / duration, 1));
			Extensions.ChangeMeshColor(meshRenderer, mpb, currentColor);

			t += Time.deltaTime;
			yield return null;
		}
	}
}
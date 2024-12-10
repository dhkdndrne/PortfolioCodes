using System;
using UnityEngine;

public class SpritePositionSortingOrder : MonoBehaviour
{
	private SpriteRenderer[] spriteRenderer;
	private IDisposable updateSubscription;

	private int[] originOrder;
	
	private void Awake()
	{
		spriteRenderer = GetComponentsInChildren<SpriteRenderer>();

		originOrder = new int[spriteRenderer.Length];
		for (int i = 0; i < spriteRenderer.Length; i++)
		{
			originOrder[i] = spriteRenderer[i].sortingOrder;
		}
	}
	private void OnEnable()
	{
		float precisionMultiplier = 5f;
        
		for (int i = 0; i < spriteRenderer.Length; i++)
		{
			spriteRenderer[i].sortingOrder = originOrder[i] + (int)(-transform.position.y * precisionMultiplier);
		}
	}
}
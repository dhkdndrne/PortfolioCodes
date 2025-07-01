using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : GridObject
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	public BlockData BlockData { get; private set; }
	public ColorLayer ColorLayer { get; private set; }

	public void SetData(BlockData data)
	{
		BlockData = data;
		SetHP(data.HP);

		if (TryGetComponent(out HpSpriteHandler hpSpriteHandler))
		{
			OnHpChanged += hpSpriteHandler.ChangeSprite;
			hpSpriteHandler.Init(data.HP);
		}
		
		spriteRenderer.sprite = data.Sprite;
		SetColor(data.ColorLayer);
	}

	public void SetColor(ColorLayer colorLayer)
	{
		if (colorLayer == ColorLayer.None)
			return;

		ColorLayer = colorLayer;

		if (Application.isPlaying)
			spriteRenderer.color = ColorManager.GetColor(colorLayer);
		else
		{
			spriteRenderer.color = ColorManager.GetColorInEditor(colorLayer);
		}
	}

	public bool CanMatchWith(Block other)
	{
		return
			BlockData.CanMatch && other.BlockData.CanMatch && ColorLayer == other.ColorLayer;
	}

	private void OnDisable()
	{
		if (HP <= 0 && TryGetComponent(out HpSpriteHandler hpSpriteHandler))
		{
			OnHpChanged -= hpSpriteHandler.ChangeSprite;
		}
	}
}
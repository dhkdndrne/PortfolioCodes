using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class BlockCatalogue : CatalogueItem
{
	private const string NORMAL_BLOCK_DATA_PATH = "Assets/09.Data/Block/Normal";
	private const string SPECIAL_BLOCK_DATA_PATH = "Assets/09.Data/Block/Special";
	private const string COLOR_DATA_PATH = "Assets/09.Data/";

	private Dictionary<ColorLayer, BlockData> blockDic = new();
	private Dictionary<SpecialBlockType, SpecialBlockData> specialBlockDic = new();
	private Dictionary<BlockData, Texture2D> textureCache = new();
	private Dictionary<ColorLayer, Color> colorDic = new();

	private (ColorLayer color,BlockData blockData) selectedBlockData;

	public override string CatalogueName => "블록";

	public override void Init()
	{
		LoadNormalBlocks();
		LoadSpecialBlocks();
		UpdateTextures();
	}

	public override void OnSelected()
	{
		selectedBlockData = (ColorLayer.None,null);
		SceneView.beforeSceneGui += SceneViewRaycast;
		HandleDrawer.SetEnable(true);
	}
	public override void OnDeSelected()
	{
		SceneView.beforeSceneGui -= SceneViewRaycast;
		HandleDrawer.SetEnable(false);
	}
	public override void DrawUI(Rect position)
	{
		EditorGUILayout.BeginVertical();
		DrawButtons(position);
		DrawSpecialBlockButtons(position);
		EditorGUILayout.EndVertical();
	}

	private void LoadNormalBlocks()
	{
		var blocks = LoadBlocks(NORMAL_BLOCK_DATA_PATH);
		var colorDataList = LoadColorData();

		if (colorDataList == null) return;

		colorDic = colorDataList.ColorList.ToDictionary(data => data.layer, data => data.color);
		blockDic = Enum.GetValues(typeof(ColorLayer))
			.Cast<ColorLayer>()
			.Where(layer => layer != ColorLayer.None)
			.ToDictionary(layer => layer, layer => blocks.FirstOrDefault(block => block.ColorLayer == layer));
	}

	private void LoadSpecialBlocks()
	{
		var specialBlocks = LoadBlocks(SPECIAL_BLOCK_DATA_PATH).OfType<SpecialBlockData>().ToList();
		specialBlockDic = Enum.GetValues(typeof(SpecialBlockType))
			.Cast<SpecialBlockType>()
			.ToDictionary(type => type, type => specialBlocks.FirstOrDefault(block => block.SBlockType == type));
	}

	#region 버튼 그리기

	protected override void DrawButtons(Rect position)
	{
		DrawColorLayerButtons(position);
	}

	private void DrawSpecialBlockButtons(Rect position)
	{
		if (selectedBlockData.color == ColorLayer.None) return;

		DrawSpecialBlockLayerButtons(position);
	}

	private void DrawColorLayerButtons(Rect position)
	{
		const float spacing = 10f;
		const float buttonSize = 60f;

		float availableWidth = position.width - (spacing * 2);
		float buttonWidth = Mathf.Min(buttonSize, availableWidth / Mathf.Ceil(Mathf.Sqrt(blockDic.Count + 1))); // +1 for "None" button

		int buttonsPerRow = Mathf.FloorToInt(availableWidth / (buttonWidth + spacing));
		int rowCount = 0;

		EditorGUILayout.BeginHorizontal();

		// Add the "None" button
		if (GUILayout.Button("X", GUILayout.Width(buttonWidth), GUILayout.Height(buttonWidth)))
		{
			selectedBlockData = ( ColorLayer.None,null);
			Debug.Log("None block selected.");
		}

		rowCount++;
		if (rowCount >= buttonsPerRow)
		{
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			rowCount = 0;
		}

		foreach (var (colorLayer, blockData) in blockDic)
		{
			if (blockData == null) continue;

			Texture2D texture = GetColoredTexture(blockData);
			if (texture == null) continue;

			GUI.backgroundColor = selectedBlockData.color == colorLayer ? Color.green : Color.white;
				
			var buttonContent = new GUIContent { image = texture };
			GUIStyle buttonStyle = new(GUI.skin.button)
			{
				fixedWidth = buttonWidth,
				fixedHeight = buttonWidth,
				imagePosition = ImagePosition.ImageAbove
			};

			if (GUILayout.Button(buttonContent, buttonStyle))
			{
				selectedBlockData.blockData = blockData;
				selectedBlockData.color = colorLayer;
			}

			rowCount++;
			if (rowCount >= buttonsPerRow)
			{
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				rowCount = 0;
			}
		}

		EditorGUILayout.EndHorizontal();
	}

	private void DrawSpecialBlockLayerButtons(Rect position)
	{
		const float spacing = 10f;
		const float buttonSize = 60f;

		float availableWidth = position.width - (spacing * 2);
		float buttonWidth = Mathf.Min(buttonSize, availableWidth / Mathf.Ceil(Mathf.Sqrt(specialBlockDic.Count)));

		int buttonsPerRow = Mathf.FloorToInt(availableWidth / (buttonWidth + spacing));
		int rowCount = 0;

		EditorGUILayout.BeginHorizontal();

		foreach (var (specialBlockType, specialBlockData) in specialBlockDic)
		{
			if (specialBlockData == null) continue;

			Texture2D texture = specialBlockData.Sprite.texture;
			if (texture == null) continue;
			
			GUI.backgroundColor = selectedBlockData.blockData == specialBlockData ? Color.green : Color.white;
			
			var buttonContent = new GUIContent { image = texture };
			GUIStyle buttonStyle = new(GUI.skin.button)
			{
				fixedWidth = buttonWidth,
				fixedHeight = buttonWidth,
				imagePosition = ImagePosition.ImageAbove
			};

			if (GUILayout.Button(buttonContent, buttonStyle))
			{
				selectedBlockData.blockData = specialBlockData;
				Debug.Log($"Color: {selectedBlockData.color}, Name: {selectedBlockData.blockData.name}");
			}

			rowCount++;
			if (rowCount >= buttonsPerRow)
			{
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				rowCount = 0;
			}
		}

		EditorGUILayout.EndHorizontal();
	}

	private List<BlockData> LoadBlocks(string path)
	{
		return AssetDatabase.FindAssets("t:BlockData", new[] { path })
			.Select(AssetDatabase.GUIDToAssetPath)
			.Select(AssetDatabase.LoadAssetAtPath<BlockData>)
			.Where(block => block != null)
			.ToList();
	}
	private ColorDataList LoadColorData()
	{
		string[] guids = AssetDatabase.FindAssets("t:ColorDataList", new[] { COLOR_DATA_PATH });
		if (guids.Length == 0)
		{
			Debug.LogError($"No ColorDataList found at {COLOR_DATA_PATH}");
			return null;
		}

		return AssetDatabase.LoadAssetAtPath<ColorDataList>(AssetDatabase.GUIDToAssetPath(guids[0]));
	}
	
	private Texture2D GetColoredTexture(BlockData blockData)
	{
		if (textureCache.TryGetValue(blockData, out var cachedTexture)) return cachedTexture;

		if (blockData.Sprite?.texture == null || !blockData.Sprite.texture.isReadable)
		{
			Debug.LogWarning("Texture is not readable.");
			return null;
		}

		Color color = blockData.ColorLayer != ColorLayer.None
			? colorDic[blockData.ColorLayer]
			: colorDic[selectedBlockData.color];

		Texture2D texture = new(blockData.Sprite.texture.width, blockData.Sprite.texture.height);
		Color[] pixels = blockData.Sprite.texture.GetPixels();

		for (int i = 0; i < pixels.Length; i++) pixels[i] *= color;

		texture.SetPixels(pixels);
		texture.Apply();

		textureCache[blockData] = texture;
		return texture;
	}

	private void UpdateTextures()
	{
		foreach (var block in blockDic.Values)
		{
			GetColoredTexture(block);
		}
	}

	#endregion

	private void SceneViewRaycast(SceneView view)
	{
		var board = Board_Edit.Instance;
		if (board?.stageData is null) return;

		Vector3 worldPos = Bam.Extensions.Extensions.ScreenToWorldPoint(view);
		Hex hex = board.WorldPosToHex(worldPos);

		if (board.IsIndexOutOfRange(hex) || board.CanNotSetBlock(hex)) return;

		var e = Event.current;
		if (e.type is EventType.MouseDown or EventType.MouseDrag && e.button == 0)
		{
			board.SetBlock(selectedBlockData, hex);
			EditorUtility.SetDirty(board.stageData);
			e.Use();
		}

		HandleDrawer.SetHandleIndex(hex, 0);
	}
}
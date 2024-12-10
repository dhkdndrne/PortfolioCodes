using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CellCatalogue : CatalogueItem
{
    private const string CELL_DATA_PATH = "Assets/09.Data/Cell";
    private const float BUTTON_SPACING = 10f;
    private const float BUTTON_SIZE = 60f;

    private readonly Dictionary<CellType, CellData> cellDic = new();
    private CellData selectedCellData;

    public override string CatalogueName => "셀";

    public override void Init()
    {
        var cellList = LoadCells(CELL_DATA_PATH);
        foreach (CellType cellType in Enum.GetValues(typeof(CellType)))
        {
            cellDic[cellType] = cellList.FirstOrDefault(b => b.cellType == cellType);
        }
    }

    public override void OnSelected()
    {
        selectedCellData = null;
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
        EditorGUILayout.EndVertical();
    }

    protected override void DrawButtons(Rect position)
    {
        var buttonDimensions = CalculateButtonDimensions(position.width, position.height, cellDic.Count);
        int rowCount = 0;

        EditorGUILayout.BeginHorizontal();
        foreach (var (cellType, cellData) in cellDic)
        {
            if (cellData == null) continue;

            var buttonContent = CreateButtonContent(cellType, cellData.sprite);
            SetButtonBackgroundColor(cellData);

            if (GUILayout.Button(buttonContent, CreateButtonStyle(buttonDimensions.width, buttonDimensions.height)))
            {
                selectedCellData = cellData;
                Debug.Log(selectedCellData.name);
            }

            rowCount = HandleRowLayout(rowCount, buttonDimensions.buttonsPerRow);
        }
        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
    }

    private (float width, float height, int buttonsPerRow) CalculateButtonDimensions(float availableWidth, float availableHeight, int itemCount)
    {
        float buttonWidth = Mathf.Min(BUTTON_SIZE, availableWidth / Mathf.Ceil(Mathf.Sqrt(itemCount)));
        float buttonHeight = Mathf.Min(BUTTON_SIZE, availableHeight / Mathf.Ceil(Mathf.Sqrt(itemCount)));
        int buttonsPerRow = Mathf.FloorToInt(availableWidth / (buttonWidth + BUTTON_SPACING));

        return (buttonWidth, buttonHeight, buttonsPerRow);
    }

    private GUIContent CreateButtonContent(CellType cellType, Sprite sprite)
    {
        return cellType == CellType.None
            ? new GUIContent { text = "X" }
            : new GUIContent { image = sprite.texture };
    }

    private void SetButtonBackgroundColor(CellData cellData)
    {
        GUI.backgroundColor = selectedCellData == cellData ? Color.green : Color.white;
    }

    private GUIStyle CreateButtonStyle(float width, float height)
    {
        return new GUIStyle(GUI.skin.button)
        {
            fixedWidth = width,
            fixedHeight = height,
            imagePosition = ImagePosition.ImageAbove
        };
    }

    private int HandleRowLayout(int rowCount, int buttonsPerRow)
    {
        rowCount++;
        if (rowCount >= buttonsPerRow)
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            return 0;
        }
        return rowCount;
    }

    private List<CellData> LoadCells(string path)
    {
        var guids = AssetDatabase.FindAssets("t:CellData", new[] { path });
        return guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<CellData>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(cell => cell != null)
            .ToList();
    }

    private void SceneViewRaycast(SceneView view)
    {
        var board = Board_Edit.Instance;

        if (!IsRaycastValid(board)) return;

        Vector3 worldPos = Bam.Extensions.Extensions.ScreenToWorldPoint(view);
        Hex hex = board.WorldPosToHex(worldPos);
        if (board.IsIndexOutOfRange(hex)) return;

        HandleMouseInput(board, hex);
        HandleDrawer.SetHandleIndex(hex, 0);
    }

    private bool IsRaycastValid(Board_Edit board)
    {
        return board?.stageData != null && selectedCellData != null;
    }

    private void HandleMouseInput(Board_Edit board, Hex hex)
    {
        var e = Event.current;
        if (e.type is EventType.MouseDown or EventType.MouseDrag && e.button == 0)
        {
            board.SetCell(selectedCellData, hex);
            EditorUtility.SetDirty(board.stageData);
            e.Use();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Bam.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AttackRangeUI : MonoBehaviour
{
    [SerializeField] private Sprite standCellSprite;
    [SerializeField] private Sprite attackRangeSprite;
    
    [SerializeField] private Image rageCellPrefab;
    [SerializeField] private RectTransform container;
    [SerializeField] private List<Image> rangeCellList = new List<Image>();
    private List<int> outOfRangeList = new List<int>();
    
    private GridLayoutGroup gridLayoutGroup;
    private ContentSizeFitter contentSizeFitter;
    
    private void Awake()
    {
        gridLayoutGroup = container.GetComponent<GridLayoutGroup>();
        contentSizeFitter = container.GetComponent<ContentSizeFitter>();
        
        rangeCellList.Add(rageCellPrefab);

        for (int i = 0; i < 24; i++)
        {
            var cell = Instantiate(rageCellPrefab,container);
            rangeCellList.Add(cell);
        }
    }
    
    public async UniTaskVoid ShowAttackRange(GridType[,] rangeGrid)
    {
        contentSizeFitter.enabled = true;
        gridLayoutGroup.enabled = true;
        
        ResetAllCells();
        outOfRangeList.Clear();
        
        int row = rangeGrid.GetLength(0);
        int col = rangeGrid.GetLength(1);
        int index = 0;

        gridLayoutGroup.constraintCount = col;
        
        if (rangeCellList.Count< col * row)
        {
            int count = (col * row) - (rangeCellList.Count);
            for (int i = 0; i < count; i++)
            {
                var cell = Instantiate(rageCellPrefab,container);
                rangeCellList.Add(cell);
            }
        }

        int standCellIndex = 0;
        for (int y = 0; y < row; y++)
        {
            for (int x = 0; x < col; x++)
            {
                switch (rangeGrid[y,x])
                {
                    case GridType.Pivot:
                        standCellIndex = Extensions.Get1DIndex(x, y, col);
                        break;
                    
                    case GridType.None:
                        outOfRangeList.Add(Extensions.Get1DIndex(x,y,col));
                        break;
                }
                
                rangeCellList[index++].gameObject.SetActive(true);
            }
        }
        
        await UniTask.Yield();

        float scaleFactor = 1f;
        
        // 크기 조정
        if (container.rect.height >= 76f)
            scaleFactor = 76f / container.rect.height;
        
        container.localScale = Vector3.one * scaleFactor;
        contentSizeFitter.enabled = false;
        gridLayoutGroup.enabled = false;

        rangeCellList[standCellIndex].sprite = standCellSprite;
        foreach (var idx in outOfRangeList)
        {
            rangeCellList[idx].gameObject.SetActive(false);
        }
    }

    private void ResetAllCells()
    {
        rangeCellList.ForEach(cell =>
        {
            cell.sprite = attackRangeSprite;
            cell.gameObject.SetActive(false);
        });
    }
}

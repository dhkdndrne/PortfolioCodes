using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UI_SummonResult : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Button exitButton;
    private ResultSlot[] slots;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        var container = resultPanel.transform.GetChild(1).GetChild(0).GetChild(0);
        slots = new ResultSlot[container.childCount];
        
        for (int i = 0; i < slots.Length; i++)
        {
            ResultSlot slot = new ResultSlot();
            slot.obj = container.GetChild(i).gameObject;
            slot.rankBG = container.GetChild(i).GetComponent<Image>();
            slot.itemIcon = container.GetChild(i).GetChild(0).GetComponent<Image>();

            slots[i] = slot;
        }
        
        exitButton.onClick.AddListener(()=>resultPanel.SetActive(false));
    }
    public async UniTaskVoid ShowResult(ItemType itemType,List<(int id,int rank)> itemList)
    {
        resultPanel.SetActive(true);
        
        foreach (var slot in slots)
            slot.obj.SetActive(false);

        int speed = itemList.Count < 100 ? 50 : 10;
        
        for (int i = 0; i < itemList.Count; i++)
        {
            slots[i].obj.SetActive(true);

            int id = itemList[i].id;
            int rank = itemList[i].rank;

            slots[i].rankBG.sprite = ImageManager.Instance.GetItemRankBg((ItemRankType)Enum.ToObject(typeof(ItemRankType), rank));
            slots[i].itemIcon.sprite = itemType switch
            {
                ItemType.Equipment => ImageManager.Instance.GetItemIcon(ItemType.Equipment,id),
                ItemType.Skill => ImageManager.Instance.GetItemIcon(ItemType.Skill,id),
                ItemType.Pet => ImageManager.Instance.GetItemIcon(ItemType.Pet,id)
            };
            await UniTask.Delay(speed);
        }
    }
}

public class ResultSlot
{
    public GameObject obj;
    public Image rankBG;
    public Image itemIcon;
}
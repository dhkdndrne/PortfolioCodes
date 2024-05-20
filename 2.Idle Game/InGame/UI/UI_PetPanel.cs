using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_PetPanel : MonoBehaviour, IPanel
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private Button[] equippedPetBtns;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private UI_PetPopUp petInfoPopUp;
    
    private Action<ItemBase> onClickSlotAction;
    
    private float originPosY;
    private readonly float TARGET_POSITION_Y = 180f;

    private void Awake()
    {
        originPosY = panel.transform.position.y;
        
        var prefab = slotContainer.GetChild(0).gameObject;
        var playerData = DataManager.Instance.PlayerData;

        var itemManager = ItemManager.Instance;
        var list = itemManager.GetItemIDList(ItemType.Pet);

        onClickSlotAction += petInfoPopUp.OpenPopUp;
        
        foreach (var pet in list)
        {
            var obj = Instantiate(prefab, slotContainer);
            var slot = obj.GetComponent<UI_PetSlot>();

            slot.InitSlotUI(itemManager.GetPetData(pet.Rank,pet.ID), onClickSlotAction);
        }
        
        Destroy(prefab);
        
        //장착 펫 아이콘 초기화
        for (int i = 0; i < equippedPetBtns.Length; i++)
        {
            int index = i;

            int id = playerData.equippedPetIDs[index].id;
            ChangeEquippedPetSlot(id, index);

            equippedPetBtns[index].onClick.AddListener(() =>
            {
                var pet = Player.Instance.EquippedPet[index];

                if (pet == null)
                    return;

                onClickSlotAction?.Invoke(pet);
            });
        }

       Player.Instance.onEquipPetAction += ChangeEquippedPetSlot;
    }

    public void OpenPanel()
    {
        gameObject.SetActive(true);
        
        UIManager.Instance.ClosePanel();
        UIManager.Instance.OpenPanel(this);
        
        panel.DOAnchorPosY(TARGET_POSITION_Y, 0.4f).SetEase(Ease.OutBack);
    }
    public void ClosePanel()
    {
        panel.DOAnchorPosY(originPosY, 0.4f).SetEase(Ease.OutBack);
        gameObject.SetActive(false);
    }
    
    private void ChangeEquippedPetSlot(int id, int index)
    {
        var child = equippedPetBtns[index].transform.GetChild(0).GetComponent<Image>();

        if (id == 0)
        {
            child.gameObject.SetActive(false);
        }
        else
        {
            child.gameObject.SetActive(true);
            child.sprite = ImageManager.Instance.GetItemIcon(ItemType.Pet, id);
        }
        equippedPetBtns[index].image.sprite = id == 0 ? ImageManager.Instance.EquippedSkillSlotBaseSprite : ImageManager.Instance.GetItemRankBg(Player.Instance.EquippedPet[index].Rank);
    }
}

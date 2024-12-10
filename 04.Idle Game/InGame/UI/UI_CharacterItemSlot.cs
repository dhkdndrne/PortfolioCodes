using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterItemSlot : UI_ItemSlot
{
    [SerializeField] private Button button;
    
    public override void InitSlotUI(ItemBase item, Action<ItemBase> action)
    {
        var imageManager = ImageManager.Instance;
        
        button.onClick.AddListener(() =>
        {
            transform.DOScale(Vector3.one * 1.1f, 0.05f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InFlash);
            action?.Invoke(item);
        });
        
        item.IsLock.DistinctUntilChanged().Subscribe(isLock =>
        {
            lockIcon.gameObject.SetActive(isLock);
            itemIcon.material = isLock ? imageManager.GrayScaleMaterial : null;
            slotBG.material = isLock ? imageManager.GrayScaleMaterial : null;
        }).AddTo(gameObject);

        item.Level.Subscribe(value =>
        {
            levelText.text = $"LV{value}";
        }).AddTo(this);

        item.piece.Subscribe(value =>
        {
            slider.value = (float)value / item.MaxPiece;
            amountText.text = $"{value}/{item.MaxPiece}";
        }).AddTo(this);
        
        itemIcon.sprite = imageManager.GetItemIcon(ItemType.Character,item.ID);
        slotBG.sprite = imageManager.GetItemRankBg(item.Rank);
    }
}

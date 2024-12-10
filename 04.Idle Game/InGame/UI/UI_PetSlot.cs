using System;
using UniRx;
using UnityEngine.UI;

public class UI_PetSlot : UI_ItemSlot
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }
    
    public override void InitSlotUI(ItemBase item, Action<ItemBase> action)
    {
        var imageManager = ImageManager.Instance;

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

        itemIcon.sprite = imageManager.GetItemIcon(ItemType.Pet,item.ID);
        slotBG.sprite = imageManager.GetItemRankBg(item.Rank);
        button.onClick.AddListener(() => action?.Invoke(item));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName = "SO/SlotRankImageData",fileName = "Slot Rank Image Data")]
public class SlotRankImageData : ScriptableObject
{
    [SerializeField] private Sprite[] bgSprites;

    public Sprite GetBGSprite(int index) => bgSprites[index];
}

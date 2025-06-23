using System.Collections.Generic;
using Bam.Singleton;
using UnityEngine;

public class ImageManager : DontDestroySingleton<ImageManager>
{
    [Header("임시 오퍼레이터 이미지 SO")]
    [SerializeField] private List<OperatorImageData> tempImageList = new List<OperatorImageData>();
    public readonly Dictionary<OperatorID, OperatorImageData> operatorImageDic = new Dictionary<OperatorID, OperatorImageData>();
    
    [SerializeField] private List<BattleClassSprite> classSpriteList = new List<BattleClassSprite>();
    public readonly Dictionary<Operator_Class, BattleClassSprite> battle_ClassSpriteDic = new Dictionary<Operator_Class, BattleClassSprite>();
    protected override void Init()
    {
        foreach (var img in tempImageList)
        {
            operatorImageDic[img.OperatorID] = img;
        }

        for (int i = 0; i < classSpriteList.Count; i++)
        {
            battle_ClassSpriteDic[classSpriteList[i].opClass] = classSpriteList[i];
        }
    }
}

[System.Serializable]
public class BattleClassSprite
{
    public Operator_Class opClass;
    public Sprite classIcon;
    public Sprite battleCardClassIcon;
}

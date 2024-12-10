using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterSpriteSO",fileName = "CharacterSpriteSO")]
public class CharacterSpriteSO : ScriptableObject
{
    [Header("머리")]
    public Sprite head, face, hood;

    [Header("왼 어깨")]
    public Sprite weapon_l, wrist_l, elbow_l, shoulder_l;
    
    [Header("오른 어깨")]
    public Sprite weapon_r, wrist_r, elbow_r, shoulder_r;

    [Header("가슴")]
    public Sprite torso;

    [Header("왼다리")]
    public Sprite boot_l, leg_l;
    
    [Header("오른 다리")]
    public Sprite boot_r, leg_r;

    [Header("허리")]
    public Sprite pelvis;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprites : MonoBehaviour
{
   [Header("머리")]
   public SpriteRenderer head, face, hood;

   [Header("왼 어깨")]
   public SpriteRenderer weapon_l, wrist_l, elbow_l, shoulder_l;
    
   [Header("오른 어깨")]
   public SpriteRenderer weapon_r, wrist_r, elbow_r, shoulder_r;

   [Header("가슴")]
   public SpriteRenderer torso;

   [Header("왼다리")]
   public SpriteRenderer boot_l, leg_l;
    
   [Header("오른 다리")]
   public SpriteRenderer boot_r, leg_r;

   [Header("허리")]
   public SpriteRenderer pelvis;

   public void ChangeSprite(CharacterSpriteSO SO)
   {
      head.sprite = SO.head;
      face.sprite = SO.face;
      hood.sprite = SO.hood;
      weapon_l.sprite = SO.weapon_l;
      wrist_l.sprite = SO.wrist_l;
      elbow_l.sprite = SO.elbow_l;
      shoulder_l.sprite = SO.shoulder_l;
      weapon_r.sprite = SO.weapon_r;
      wrist_r.sprite = SO.wrist_r;
      elbow_r.sprite = SO.elbow_r;
      shoulder_r.sprite = SO.shoulder_r;
      torso.sprite = SO.torso;
      boot_l.sprite = SO.boot_l;
      leg_l.sprite = SO.leg_l;
      boot_r.sprite = SO.boot_r;
      leg_r.sprite = SO.leg_r;
      pelvis.sprite = SO.pelvis;
   }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "OperatorImageSo", menuName = "Scriptable Objects/OperatorImageSo")]
public class OperatorImageData : ScriptableObject
{
	[SerializeField] private OperatorID operatorID;
	[SerializeField] private List<Sprite> operatorSpriteList = new List<Sprite>();
	[SerializeField] private List<Sprite> faceIconList = new List<Sprite>();
	[SerializeField] private List<Sprite> portraitList = new List<Sprite>();
	[SerializeField] private List<Sprite> skillIconList = new List<Sprite>();

	public OperatorID OperatorID => operatorID;
	public Sprite GetSprite(int index) => operatorSpriteList[index];
	public Sprite GetFaceIcon(int index) => faceIconList[index];
	public Sprite GetPortrait(int index) => portraitList[index];
	public Sprite GetSkillIcon(int index) => skillIconList[index];
}
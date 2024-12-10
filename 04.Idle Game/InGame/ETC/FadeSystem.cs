using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class FadeSystem
{
	[FormerlySerializedAs("fadeObject")] [SerializeField] private Image fadeImage;
	
	public async UniTask FadeIn()
	{
		fadeImage.raycastTarget = true;
		await fadeImage.DOColor(new Color(0,0,0,1), 0.3f);
	}
	
	public async UniTask FadeOut()
	{
		await fadeImage.DOColor(new Color(0,0,0,0), 0.3f);
		fadeImage.raycastTarget = false;
	}
}

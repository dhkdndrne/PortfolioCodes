using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


public class UI_UnitHp : MonoBehaviour
{
	[SerializeField] private Slider hpSlider;
	[SerializeField] private RectTransform whiteSlider;

	private void Start()
	{
		hpSlider.OnValueChangedAsObservable().Subscribe(value =>
		{
			// 0.2초 후에 이벤트를 실행하도록 설정
			Observable.Timer(TimeSpan.FromSeconds(0.2f))
				.Subscribe(_ =>
				{
					float targetValue = value; // 목표 값은 Slider의 값
					float duration = 0.5f;     // Lerp가 진행될 시간

					float startTime = Time.time;
					Vector2 startAnchorMax = whiteSlider.anchorMax;

					Observable.EveryUpdate()
						.TakeWhile(_ => Time.time - startTime <= duration) // Lerp를 duration 동안 진행
						.Subscribe(_ =>
						{
							float lerpT = (Time.time - startTime) / duration;
							whiteSlider.anchorMax = Vector2.Lerp(startAnchorMax, new Vector2(targetValue, 1), lerpT);
						})
						.AddTo(this);
				})
				.AddTo(this);
		}).AddTo(this);
	}

	public void Init()
	{
		hpSlider.gameObject.SetActive(true);
		hpSlider.value = 1;
		whiteSlider.anchorMax = Vector2.one;
	}
	
	public void UpdateSlider(float value)
	{
		hpSlider.value = value;
	}
}
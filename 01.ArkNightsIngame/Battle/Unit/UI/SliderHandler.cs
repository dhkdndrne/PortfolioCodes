using System;
using UnityEngine;
using UnityEngine.UI;

public class SliderHandler : MonoBehaviour
{
	[SerializeField] protected Slider sliderBar;
	protected float initialValue;
	
	public virtual void ChainEvent(ObservableValue<float> ov,float initialValue)
	{
		ov.Subscribe(SetSliderValue);
		this.initialValue = initialValue;
	}
	public void UnChainEvent(ObservableValue<float> ov)
	{
		ov.Unsubscribe(SetSliderValue);	
	}
	
	public virtual void SetSliderActive(bool active)
	{
		gameObject.SetActive(active);
		sliderBar.value = initialValue;
	}
	protected virtual void SetSliderValue(float value)
	{
		sliderBar.value = value;
	}
}

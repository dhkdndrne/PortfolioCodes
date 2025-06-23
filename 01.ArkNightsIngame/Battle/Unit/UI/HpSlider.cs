using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HpSlider : SliderHandler
{
	[SerializeField] private Slider ghostBar;
	
	public override void ChainEvent(ObservableValue<float> ov,float initialValue)
	{
		base.ChainEvent(ov, initialValue);
		ghostBar.value = initialValue;
	}

	protected override void SetSliderValue(float value)
	{
		base.SetSliderValue(value);
		ShowGhostBarAnim().Forget();
	}

	public override void SetSliderActive(bool active)
	{
		base.SetSliderActive(active);
		ghostBar.gameObject.SetActive(active);
	}

	private async UniTaskVoid ShowGhostBarAnim()
	{
		while (true)
		{
			float x = Mathf.Lerp(ghostBar.value, sliderBar.value, Time.deltaTime * 5f);
			ghostBar.value = x;
			
			if (sliderBar.value >= ghostBar.value - 0.01f)
			{
				ghostBar.value = sliderBar.value;
				break;
			}
			await UniTask.Yield();
		}
	}
	

}
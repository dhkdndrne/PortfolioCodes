using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour
{
	[SerializeField] SliderHandler sliderHandler;
	
	private ObservableValue<float> s = new ObservableValue<float>(0);
	private ObservableValue<float> ratio = new ObservableValue<float>(0);
	
	private float max = 516f;

	private void Awake()
	{
		s.Subscribe(val =>
		{
			ratio.Value = val / max;
		});
	}

	private void Start()
	{
		sliderHandler.ChainEvent(ratio,0);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			s.Value += 100;
		}
	}
}

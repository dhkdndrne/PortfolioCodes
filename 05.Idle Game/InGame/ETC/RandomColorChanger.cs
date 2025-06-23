using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.SimpleSpinner
{
	[RequireComponent(typeof(Image))]
	public class RandomColorChanger : MonoBehaviour
	{
		[Range(-10, 10), Tooltip("Value in Hz (revolutions per second).")]
		public float RainbowSpeed = 0.5f;
		[Range(0, 1)]
		public float RainbowSaturation = 1f;
		public AnimationCurve RainbowAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);

		[Header("Options")]
		public bool RandomPeriod = true;

		private Image image;
		private float period;

		private void Awake()
		{
			image = GetComponent<Image>();
			period = RandomPeriod ? Random.Range(0f, 1f) : 0;
		}
        
		private void Update()
		{
			image.color = Color.HSVToRGB(RainbowAnimationCurve.Evaluate((RainbowSpeed * Time.time + period) % 1), RainbowSaturation, 1);
		}
	}
}
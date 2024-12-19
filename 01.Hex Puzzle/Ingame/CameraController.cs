using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Puzzle.Ingame
{
	public class CameraController : MonoBehaviour
	{
		public float padding = 0.2f;

		public void FitCamera(int row,float rowGap)
		{
			var cam = Camera.main;
			cam.orthographicSize = row * rowGap * (1 + padding);
		}
	}
}

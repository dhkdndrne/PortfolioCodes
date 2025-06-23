using UnityEngine;
using UnityEngine.UI;

public class InfiniteScroll : MonoBehaviour
{
	public RawImage image;
	public float scrollSpeed = 0.1f;

	private void Update()
	{
		Rect uv = image.uvRect;
		uv.x += scrollSpeed * Time.deltaTime; // 좌우 스크롤
		image.uvRect = uv;
	}
}

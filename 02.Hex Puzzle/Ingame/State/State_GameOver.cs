using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;

public class State_GameOver : State
{
	[SerializeField] RectTransform gameOverPanel;
	private void Awake()
	{
		OnBeginStream.Subscribe(async _ =>
		{
			gameOverPanel.anchoredPosition = new Vector2(0, Screen.height);

			gameOverPanel.gameObject.SetActive(true);
			await gameOverPanel.DOAnchorPosY(Screen.height * 0.5f, 0.5f).SetEase(Ease.OutBounce);

			await UniTask.Delay(2000);

			SceneManager.LoadScene("LobbyScene");
		}).AddTo(this);
	}
}
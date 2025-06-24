using System;
using UnityEngine;
using Bam.Singleton;
using TMPro;
using UnityEditor;


public class TimeManager : Singleton<TimeManager>
{
	private float timeScale;
	private float frameTimer;
	private float frameDuration;
	private float prevTimeScale;
	
	private bool isPaused;
	private bool skipOnce;	
	
	public int Frame { get; private set; }

	public event Action<float> OnTimeScaleChanged;
	
	protected override void Init()
	{
		SetTimeScale(0.5f);
		prevTimeScale = 0.5f;
		Application.targetFrameRate = 60;
		frameDuration = 1f / Application.targetFrameRate; 	//프레임 보정
	}
	private void OnApplicationPause(bool pause)
	{
		skipOnce = pause;
	}
	
	private void Update()
	{
		float unscaled = Time.unscaledDeltaTime;
		
		if (skipOnce)
		{
			// 첫 복귀 프레임의 dt를 0으로 건너뛴다
			unscaled = 0f;
			skipOnce = false;
		}
    
		// 추가로 이상치 dt 방지를 위해 클램프
		if (unscaled > 0.2f)
			unscaled = 0f;
		
		float dt = isPaused ? 0f : unscaled * timeScale;
		
		CustomTime.UpdateDeltaTime(dt);

		// 프레임 카운터 로직
		frameTimer += CustomTime.deltaTime;
		while (frameTimer >= frameDuration)
		{
			Frame++;
			frameTimer -= frameDuration;
		}
	}
	
	public void Stop()
	{
		isPaused = true;
		SetTimeScale(0);
	}

	public void Resume()
	{
		isPaused = false;
		SetTimeScale(prevTimeScale);
	}

	public void SetTimeScale(float t)
	{
		if (Mathf.Approximately(timeScale, t)) return;
		prevTimeScale = timeScale;
		timeScale = t;
		OnTimeScaleChanged?.Invoke(timeScale);
	}

	public void StartSlowMotion(float slowValue = 0.1f)
	{
		if (AutoBattleManager.Instance.IsReplayMode.Value)
			return;
		if (isPaused) return;
		
		SetTimeScale(slowValue);
	}

	public void StopSlowMotion()
	{
		if (isPaused) return;
		if (timeScale >= 0.5f) return;
		
		SetTimeScale(prevTimeScale);
	}
}

public static class CustomTime
{
	public static float deltaTime { get; private set; }
	
	public static void UpdateDeltaTime(float dt)
	{
		deltaTime = dt;
	}
}
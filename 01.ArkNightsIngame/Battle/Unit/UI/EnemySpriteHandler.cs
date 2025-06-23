using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemySpriteHandler : MonoBehaviour
{
    private SpriteRenderer[] spriteRenders;
    private CancellationTokenSource hitToken;
    
    private void Awake()
    {
        Transform body = transform.Find("body");
        spriteRenders = body.GetComponentsInChildren<SpriteRenderer>();
    }

    public void FlipSprite(bool isFacingRight)
    {
        int val = isFacingRight ? -1 : 1;
        transform.localScale = new Vector3(transform.localScale.x * val, transform.localScale.y, transform.localScale.z);
    }
    
    public async UniTaskVoid HitSpriteEffect()
    {
        hitToken?.Cancel();
        hitToken = new CancellationTokenSource();
        var token = hitToken.Token;

        // 즉시 빨간색으로 변경
        foreach (var sr in spriteRenders)
        {
            sr.color = Color.red;
        }

        float elapsed = 0f;
        Color from = Color.red;
        Color to = Color.white;

        try
        {
            while (elapsed < 1f)
            {
                if (token.IsCancellationRequested) return;

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / 0.5f);
                Color lerp = Color.Lerp(from, to, t);

                foreach (var sr in spriteRenders)
                {
                    sr.color = lerp;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        catch (OperationCanceledException)
        {
            return;
        }

        // 마지막 색 보정
        foreach (var sr in spriteRenders)
        {
            sr.color = to;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class PlayerWorldUI : MonoBehaviour
{
    [SerializeField] private Vector3 worldOffset = new Vector3(1f, 0f, 0f);

    [SerializeField] private Image staminaFillImage;
    [SerializeField] private CanvasGroup staminaCanvasGroup;

    private Camera cam;
    private Transform targetPlayer;

    private CancellationTokenSource staminaFadeCts;

    private void Start()
    {
        cam = Camera.main;
    }

    public void Init(Transform target)
    {
        targetPlayer = target;

        staminaCanvasGroup.alpha = 0f;
    }

    private void LateUpdate()
    {
        if(cam != null && targetPlayer != null)
        {
            Vector3 targetWorldPos = targetPlayer.position
                                 + (cam.transform.right * worldOffset.x)
                                 + (cam.transform.up * worldOffset.y);

            Vector3 screenPos = cam.WorldToScreenPoint(targetWorldPos);

            if(screenPos.z > 0)
            {
                screenPos.z = 0;
                transform.position = screenPos; 
            }
        }
    }

    public void UpdateStamina(float currentStamina, float maxStamina)
    {
        staminaFillImage.fillAmount = currentStamina / maxStamina;

        if (currentStamina >= maxStamina)
        {
            StartStaminaFadeOut().Forget();
        }
        else
        {
            staminaFadeCts?.Cancel();
            staminaCanvasGroup.alpha = 1f;
        }
    }
        
    private async UniTaskVoid StartStaminaFadeOut()
    {
        staminaFadeCts?.Cancel();
        staminaFadeCts = new CancellationTokenSource();
        var token = staminaFadeCts.Token;

        try
        {
            await UniTask.WaitForSeconds(1.0f, cancellationToken: token);

            float duration = 0.5f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                staminaCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
            staminaCanvasGroup.alpha = 0f;
        }
        catch (OperationCanceledException)
        {

        }
    }

    private void OnDestroy()
    {
        staminaFadeCts?.Cancel();
        staminaFadeCts?.Dispose();
    }
}

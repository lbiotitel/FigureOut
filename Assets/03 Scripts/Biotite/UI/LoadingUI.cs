using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float fadeOutTime = 1f;

    public async UniTask FadeIn()
    {
        await FadeTo(1, fadeInTime);
    }

    public async UniTask FadeOut()
    {
        await FadeTo(0, fadeOutTime);
    }

    private async UniTask FadeTo(float targetA, float duration)
    {
        float startA = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(startA, targetA , t);
            await UniTask.Yield();
        }
        
        canvasGroup.alpha = targetA;
    }
}

using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ElDoradoDoorway : MonoBehaviour, IInteractable
{
    [SerializeField] private Image fadeImage;
    private bool isTransitioning = false;
    [SerializeField] private Transform targetTrans;
    private void OnTriggerEnter(Collider other)
    {
        if (isTransitioning) return;
        Utils.CheckPlayerInteract(other, this, (playerUnit) => {
            Managers.Event.NotifyEvent<PlayerTriggerEnter>(
                new PlayerTriggerEnter
                {
                    position = transform.position,
                    interactableObject = this,
                    interactText = "황금향으로 들어가기"
                });
        });
    }


    private void OnTriggerStay(Collider other)
    {
        if (Managers.Input.InteractButton)
        {
            Utils.CheckPlayerInteract(other, this, Interact);
        }
    }

    public async void Interact(PlayerUnit unit)
    {
        if (isTransitioning) return;
        isTransitioning = true;

        var cts = this.GetCancellationTokenOnDestroy();

        try
        {
            await PlayFadeInAsync(0.5f, cts);

            unit.GetComponent<PlayerMovement>().Teleport(targetTrans.position);
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: cts); // ��� ���

            await PlayFadeOutAsync(0.5f, cts);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("OperationCanceledException");
        }
        finally
        {
            isTransitioning = false;
        }
    }



    private void OnTriggerExit(Collider other)
    {

        Managers.Event.NotifyEvent<PlayerTriggerExit>(new PlayerTriggerExit());
    }


    public async UniTask PlayFadeInAsync(float duration, CancellationToken cancellationToken = default)
    {
        fadeImage.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / duration);
            fadeImage.color = color;

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
    }

    public async UniTask PlayFadeOutAsync(float duration, CancellationToken cancellationToken = default)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (elapsedTime / duration));
            fadeImage.color = color;

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        fadeImage.gameObject.SetActive(false);
    }
}

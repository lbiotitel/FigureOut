using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadUI : MonoBehaviour
{
    [SerializeField] private Image reloadImage;
    private Camera cam;

    private void Start()
    {
        Hide();
        cam = Camera.main;
    }

    private void Update() => transform.LookAt(cam.transform);


    public void Reload(float time)
    {
        Show();
        ReloadAsync(time);
    }

    private async void ReloadAsync(float time)
    {
        reloadImage.fillAmount = 0;
        var t = 0f;
        while (t < time)
        { 
            reloadImage.fillAmount = t / time;
            t += Time.deltaTime;
            await UniTask.Yield();
        }
        Hide();
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DOTween 네임스페이스 추가 필수!

public class UnitWorldHP : MonoBehaviour
{
    [SerializeField] private Image hpSlider;
    [SerializeField] private Image hpSliderBG;
    [SerializeField] private float animDuration;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        Vector3 targetDir = cam.transform.position - transform.position;
        targetDir.x = 0f;
        transform.rotation = Quaternion.LookRotation(-targetDir);
    }



    public void DrawHpUI(float hp, float newHp, float maxHp)
    {
        float currentRatio = Mathf.Clamp01(newHp / maxHp);
        hpSlider.fillAmount = currentRatio;

        if (newHp < hp)
        {
            hpSliderBG.DOFillAmount(hpSlider.fillAmount, animDuration).SetEase(Ease.OutQuad);
        }
    }

}
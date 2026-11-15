using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using DG.Tweening;


public class DaytimeLightManager : MonoBehaviour
{
    [SerializeField] private SpriteAtlas uiSpriteAtlas;
    // 0: bg, 1: day, 2: night
    [SerializeField] private Sprite[] spriteBundle;

    [Header("Timer")]
    [SerializeField] private DayCycleTimer dayCycle;
    [SerializeField] private int updateTick;
    [SerializeField] private float animateTime;

    [Header("Environment")]
    [SerializeField] private Transform objSun;
    private int sunAngle = 0;
    // 디버그용
    [SerializeField] private Vector3 sunTransform;

    [Header("UI")]
    [SerializeField] private Image timeGaugeBg;
    [SerializeField] private Image timeGaugeColor;
    [SerializeField] private Slider uiDayTimeSlider;

    private void Awake()
    {
        dayCycle = gameObject.GetComponent<DayCycleTimer>();

        spriteBundle = new Sprite[uiSpriteAtlas.spriteCount];
        uiSpriteAtlas.GetSprites(spriteBundle);

        timeGaugeBg.sprite = spriteBundle[0];
        timeGaugeColor.sprite = spriteBundle[1];
        uiDayTimeSlider.value = uiDayTimeSlider.maxValue;

        updateTick = 20;
        animateTime = 1f;
    }

    private void Start()
    {
        dayCycle.onTimeChanged += ChangeHalfDay;
        dayCycle.onIntervalTimeChanged += UpdateGraphic;
    }

    private void UpdateGraphic(int elapsedTime)
    {
        // 디버그용
        sunTransform = objSun.gameObject.transform.eulerAngles;

        int rotationAngle = 360 / (int)(dayCycle.TotalDayTime / dayCycle.IntervalChangeUIValue);
        sunAngle += rotationAngle;

        /*
        Debug.Log($"회전할 각도: {rotationAngle}" +
                  $"\n고정 회전 각도는 {360 / (dayCycle.TotalDayTime / dayCycle.IntervalChangeUIValue)}!" +
                  $"\n회전 횟수가 몇 번 남았냐면~ {(100 / (100 - elapsedTime))}!");
        */

        // DoTween이 중복되지 않게 기존에 실행된 트윈을 종료
        objSun.DOKill();
        uiDayTimeSlider.DOKill();

        objSun.DOLocalRotate(new Vector3(sunAngle, 0f, 0f), animateTime).SetEase(Ease.InOutQuad);
        uiDayTimeSlider.DOValue((100f - elapsedTime) * 0.01f, animateTime).SetEase(Ease.InOutQuad);

        /*
        // 태양 회전
        sunAngle += rotationAngle;
        //Debug.Log(sunAngle);
        objSun.localRotation = Quaternion.Euler(sunAngle, 0f, 0f);

        // UI 갱신
        uiDayTimeSlider.value = (100f - elapsedTime) * 0.01f;
        //uiDayTimeSlider.value -= (uiDayTimeSlider.maxValue / dayCycle.TotalDayTime * 2) * 0.01f;
        */
    }

    private void ChangeHalfDay(bool isNight)
    {
        uiDayTimeSlider.value = uiDayTimeSlider.maxValue;
        timeGaugeColor.sprite = isNight == false ? spriteBundle[1] : spriteBundle[2];

        if (isNight == false) sunAngle = 0;
    }


    private void OnDisable()
    {
        dayCycle.onTimeChanged -= ChangeHalfDay;
        dayCycle.onIntervalTimeChanged += UpdateGraphic;
    }
}

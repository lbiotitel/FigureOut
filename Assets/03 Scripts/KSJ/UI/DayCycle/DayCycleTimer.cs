using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline;
using UnityEngine;

public class DayCycleTimer : MonoBehaviour
{
    // 현재 지나가는 시간
    [SerializeField] private float timer;

    // 하루 = 6분(360f)  반나절 = 3분(180f)
    [SerializeField] private float totalDaytime = 360f;

    // 밤 or 낮인지 확인하는 bool 변수
    [SerializeField] private bool isNight = false;

    // UI 슬라이더 값 조정에 사용할 이벤트
    public event Action<bool> onTimeChanged;

    [SerializeField] private float timeProgress = 0f;

    [Header("Flag")]
    public bool isRunning = false;

    [Header("Interval")]
    // 그래픽이 갱신되는 간격
    [SerializeField] private int intervalChangeUIValue = 10;
    // timer를 정수로 표기하는 변수
    [SerializeField] private int intervalChangeUI;
    public event Action<int> onIntervalTimeChanged;
    private bool toggleEvent = false;

    private void Start()
    {
        onTimeChanged?.Invoke(isNight);
    }
    public void StartTimer() => isRunning = true;

    private void Update()
    {
        if   (isRunning == false) return;
        else TimeElapse();
    }


    private void TimeElapse()
    {
        timer += Time.deltaTime;
        timeProgress = (timer / totalDaytime) * 2 * 100f;

        intervalChangeUI = (int)timer;

        // 밤낮 제어
        // DaytimeLightManager.cs >>> ChangeHalfDay()
        if (timer > totalDaytime / 2 && isNight == true)
        {
            isNight = false;
            onTimeChanged?.Invoke(isNight);
            timeProgress = 0f;
            timer = 0f;
        }

        else if (timer > totalDaytime / 2 && isNight == false)
        {
            isNight = true;
            onTimeChanged?.Invoke(isNight);
            timeProgress = 0f;
            timer = 0f;
        }

        // UI 및 맵 빛을 특정 간격마다 제어
        // DaytimeLightManager.cs >>> UpdateGraphic()
        if (intervalChangeUI != 0 && intervalChangeUI % intervalChangeUIValue == 0)
        {
            if(toggleEvent == false)
            {
                onIntervalTimeChanged?.Invoke((int)timeProgress);
                toggleEvent = true;
            }
        }

        else
        {
            toggleEvent = false;
        }

    }

    public float Timer
    {
        get { return timer; }
    }

    public bool IsNight
    {
        get { return isNight; }
    }

    public float TimeProgress
    {
        get { return timeProgress; }
    }

    public float TotalDayTime
    {
        get { return totalDaytime; }
    }

    public int IntervalChangeUIValue
    {
        get { return intervalChangeUIValue; }
    }

    public int IntervalChangeUI
    {
        get { return intervalChangeUI; }
    }
}

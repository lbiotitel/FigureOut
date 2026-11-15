using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelecopterAnim : MonoBehaviour
{
    [Header("Rotor Anim")]
    [SerializeField] private GameObject rotor;
    [SerializeField] private float rotorSpeed;  

    [Header("Floating Anim")]
    [SerializeField] private float floatingScale;

    [Header("Move Anim")]
    [SerializeField] private float moveDuration;
    [SerializeField] private Transform wayPoint;
    public event Action OnAnimFinished;
    
    private bool isMove = false;

    private void Update()
    {
        RotorAnim();
        FloatingAnim();
    }


    public void MoveWayPoint()
    {
        isMove = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = wayPoint.position;

        Vector3 midPos = Vector3.Lerp(startPos, endPos, 0.5f) + Vector3.up * 2f + Vector3.right * 1f;

        Vector3[] path = new Vector3[] { midPos, endPos };

        transform.DOPath(path, moveDuration, PathType.CatmullRom).SetEase(Ease.InQuad)
            .OnComplete( () => { OnAnimFinished?.Invoke(); });
    }

    private void RotorAnim()
    {
        rotor.transform.Rotate(0, rotorSpeed * Time.deltaTime, 0);
    }


    private void FloatingAnim()
    {
        if (isMove) return;
        transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(Time.time) * floatingScale, transform.position.z);
    }
}

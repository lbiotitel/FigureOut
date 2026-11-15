using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingParticle : MonoBehaviour,IPoolingObject
{
    public GameObject OriginPrefab { get; set; }
    private ParticleSystem particle;
    private bool isPooling = false;
    private Transform targetTransform = null;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void Init(Transform targetTransform)
    { 
        this.targetTransform = targetTransform;
    }

    private void Update()
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position;
        }
    }

    private void OnEnable()
    {
        particle.Play();
        isPooling = false;
    }

    public void ResetObject()
    {
        particle.Stop();
    }

    public void ReturnObject()
    {
        if (isPooling) return;
        isPooling = true;
        targetTransform = null;
        ResetObject();
        Managers.Object.PoolingSystem.ReturnObject(gameObject);
    }

    private void OnParticleSystemStopped()
    {
        ReturnObject();
    }
}

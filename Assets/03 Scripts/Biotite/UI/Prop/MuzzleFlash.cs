using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class MuzzleFlash : MonoBehaviour
{
    private GameObject pointLight;
    private ParticleSystem particle;

    private void Awake()
    {
        pointLight = transform.Find("Point Light").gameObject;
        particle = GetComponentInChildren<ParticleSystem>();
        pointLight.SetActive(false);
    }


    public void PlayEffect()
    {
        pointLight.SetActive(true);
        particle.Play();
        LightOff();
    }

    private async void LightOff()
    {
        await UniTask.Yield();
        pointLight?.SetActive(false);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashLight : MonoBehaviour
{
    [SerializeField] private GameObject muzzleLight;

    private void OnParticleSystemStopped()
    {
        muzzleLight.SetActive(false);
    }
}

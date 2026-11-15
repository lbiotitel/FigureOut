using DG.Tweening;
using System.Collections;
using UnityEngine;

public class RecoilController : MonoBehaviour
{

    private Transform originTransform;

    public void Init(Transform originTransform)
    { 
        this.originTransform = originTransform;
    }

    public void AddRecoil(float pitch, float yaw, float kickBack, float time)
    {
        DoRecoil(time, kickBack);
    }

    private void DoRecoil(float time, float kickBack)
    {
        originTransform.DOComplete();

        originTransform.DOPunchPosition(Vector3.forward * -kickBack, time, vibrato: 5, elasticity: 0.5f);

        float upwardForce = kickBack * 10f;
        originTransform.DOPunchRotation(new Vector3(-upwardForce, 0, 0), time, vibrato: 5, elasticity: 0.5f);
    }


}

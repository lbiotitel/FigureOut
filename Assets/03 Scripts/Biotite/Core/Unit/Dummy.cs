using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Unit, IDmgAble
{
    

    public override void Init(UnitData unitData)
    {
        
    }

    public void TakeDamage(DmgContext context)
    {
        Managers.Audio.PlaySfx(KeyDefines.SoundKey.HitSfx);
        
        //히트 포인트 ui도 오브젝트 매니저로 위임
        //GameObject hitPoint = Managers.Data.GetData<GameObject>("Hit Point");

        //나중에 오브젝트 매니저로 위임
        Managers.Object.CreateDamageText(10,EHitType.Head,transform.position + new Vector3(0,1,0),Quaternion.identity);

        transform.DOComplete();
        transform.DOPunchScale(Vector3.one * 0.1f,0.2f, 2);
    }

    protected override void Die(string killerName)
    {
        throw new System.NotImplementedException();
    }
}

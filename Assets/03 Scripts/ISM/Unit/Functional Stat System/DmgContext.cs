using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgContext 
{
    public float atk;
    public float ctiDmg;
    public float dmgCor;
    public Unit ownUnit;
    public Vector3 hitPoint;

    public Dictionary<EElementType, float> elementDamages;

    public DmgContext(Unit unit) 
    {
        ownUnit=unit;

        atk=unit.unitStat.GetStat(EStatType.Atk);
        ctiDmg=unit.unitStat.GetStat(EStatType.CriDamage);
        dmgCor=unit.unitStat.GetStat(EStatType.DamageCorrection);
        SetElementStack(unit);
    }

    private void SetElementStack(Unit unit)
    {

        elementDamages = new Dictionary<EElementType, float>();

        float igniteVal = unit.unitStat.GetStat(EStatType.IgniteAtkCount);
       // Debug.Log($"[DmgContext] 공격자({unit.gameObject.name})의 화상 공격력 수치: {igniteVal}");
        if (igniteVal > 0f) elementDamages.Add(EElementType.Ignite, igniteVal);

        float freezeVal = unit.unitStat.GetStat(EStatType.FreezeAtkCount);
        if (freezeVal > 0f) elementDamages.Add(EElementType.Freeze, freezeVal);

        float corrosionVal = unit.unitStat.GetStat(EStatType.CorrosionAtkCount);
        if (corrosionVal > 0f) elementDamages.Add(EElementType.Corrosion, corrosionVal);
        
        float irradiationVal = unit.unitStat.GetStat(EStatType.IrradiationAtkCount);
        if (irradiationVal > 0f) elementDamages.Add(EElementType.Irradiation, irradiationVal);

        float markedVal = unit.unitStat.GetStat(EStatType.MarkedAtkCount);
        if (markedVal > 0f) elementDamages.Add(EElementType.Marked, markedVal);

        float injuryVal = unit.unitStat.GetStat(EStatType.InjuryAtkCount);
        if (injuryVal > 0f) elementDamages.Add(EElementType.Injury, injuryVal);

        // (나머지 부상, 쇠약 등도 똑같이 추가...)
    }

}

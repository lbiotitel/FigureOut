using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Player,
    Enemy
}

public abstract class Unit : MonoBehaviour
{
    [SerializeField] public UnitStat unitStat;
    private List<object> equipments = new List<object>();
    public bool IsDead { get; protected set; } = false;
    public UnitType unitType { get; protected set; }

    public EquipmentController EquipmentController { get; private set; }
    protected UnitWorldHP worldHpUI;
    private ElementStat elementStat;
    public UnitData UnitData { get; private set; }
    Collider[] childColliders;

    private bool isMarked;

    private bool igniteImmunity = false;
    private bool freezeImmunity = false;
    private bool irradiationImmunity = false;
    private bool corrosionImmunity = false;
    private bool markedImmunity = false;
    private bool injuryImmunity = false;

    public virtual void Init(UnitData unitData)
    {
        unitStat = new UnitStat();
        UnitData = unitData;
        unitStat.Init(unitData);
        unitType = unitData.UnitType;
        worldHpUI = GetComponentInChildren<UnitWorldHP>();
        unitStat.OnHpChanged += worldHpUI.DrawHpUI;

        EquipmentController = GetComponent<EquipmentController>();
        EquipmentController.Init(this);

        elementStat = new ElementStat(this);



        igniteImmunity = unitData.IgniteImmunity;
        freezeImmunity = unitData.FreezeImmunity;
        irradiationImmunity = unitData.IrradiationImmunity;
        corrosionImmunity = unitData.CorrosionImmunity;
        markedImmunity = unitData.MarkedImmunity;
        injuryImmunity = unitData.InjuryImmunity;

        childColliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in childColliders)
        {
            if (col.gameObject == gameObject) continue;
            HitPoint point = col.GetComponent<HitPoint>();
            if (point != null)
            {
                point.Setup(this.TakeDamage);
            }
            else
            {
              //  Debug.LogWarning($"{col.gameObject.name}에 EnemyHitPoint 스크립트가 누락되었습니다!");
            }
        }


    }

    public virtual void Equip()
    {

    }

    public virtual void UnEquip(object source)
    {
        if (equipments.Contains(source)) return;//�ش��ϴ� ������Ʈ�� ���ٸ� �ѱ��

        unitStat.RemoveAllModifiresFromSource(source);//�ش� source�� ���� ��� StatModifire�� ����

        equipments.Remove(source);//�ش� ������Ʈ�� ����Ʈ���� ����

    }

    public virtual void UnEquipAll()//�ϰ� ��� ����
    {
        for (int i = equipments.Count - 1; i >= 0; i--)
        {
            UnEquip(equipments[i]);
        }
    }

    public void SetMarked(bool value)
    {
        // 죽은 놈한테는 표식 안 찍힘!
        if (IsDead) return;

        isMarked = value;
    }
    
    public virtual void OnDamage(float damage, bool crit, bool isDot = false) { }

    public virtual void TakeDamage(DmgContext context, bool crit = false)
    {
        if (IsDead) return;
        Enemy enemy = context.ownUnit as Enemy;
        Managers.Audio.PlaySfx(KeyDefines.SoundKey.HitSfx);

        //날아온 속성 데미지(게이지) 싹 다 적용하기
        if (context.elementDamages != null)
        {
          //  Debug.Log("속성 데미지 들어옴");
            // 딕셔너리에 들어있는 모든 속성을 순회하며 게이지를 올림!
            foreach (var element in context.elementDamages)
            {
                EElementType type = element.Key;   // 예: Fire
                float amount = element.Value;      // 예: 20f
                // 우리가 아까 만들었던 상태이상 관리자에게 일을 넘김!
                elementStat.ApplyElementGauge(type, amount);
            }
        }

        if (isMarked && !markedImmunity)
            crit = true;
        EHitType hitType = crit == true ? EHitType.Head : EHitType.Body;
        
        float finalDamage = DamageCalculator.Calculate(context.atk, unitStat, crit, context.ctiDmg);

        OnDamage(finalDamage, crit);

        Managers.Object.CreateDamageText(finalDamage, hitType,
            new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);

        StatModifier hpModifier = new StatModifier(-finalDamage, EStatModifireType.Add, context.ownUnit);
        unitStat.AddModifire(EStatType.Hp, hpModifier); 

        transform.DOComplete();
        transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 2);

        if (unitStat.GetStat(EStatType.Hp) <= 0)
        {
            IsDead = true;
            Die(context.ownUnit.UnitData.unitName);
        }
    }
    public virtual void ApplySpeed() { }//그냥 현재 유닛의 속도를 스피드 속도로 갱신해주는 내용

    public virtual void TakeDotDamage(float damage, EElementType elementType)
    {
        if (IsDead) return;

        // 1. 데미지 텍스트 띄우기 (화상/부상 등 속성에 따라 색상을 다르게 줄 수도 있습니다)
        Managers.Object.CreateDamageText(damage, EHitType.Body,
            new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);

        // 2.모디파이어 대신 ConsumeStat으로 깔끔하게 체력 차감 (-damage)
        ResourceStat hpStat = unitStat.GetResourceStat(EStatType.Hp);
        hpStat.ConsumeStat(-damage);

        OnDamage(damage, false, true);

        // 3. 사망 체크
        if (hpStat.Value <= 0)
        {
            IsDead = true;
            Die(Utils.ElementTypeToKor.GetKor(elementType));
        }

    }

    private void OnDestroy()
    {
        transform.DOKill();

    }


    protected abstract void Die(string killerName);


}

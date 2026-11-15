using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EStatModifireType
{
    Add,
    Percent,
}

public class StatModifier//스텟수정의 클래스. 얘 또한 스텟과 같이 어떠한 종류의 스텟을 바꾸는지 모른다
{

    public float Value;//변동 데이터 수치 값
    public EStatModifireType Type;//수정될 방식
    public object source;//수정하는 객체의 대상

    public StatModifier(float value, EStatModifireType type, object source)//생성자 객체에서 이 인스턴스를 생성해 유닛에게 전달
                                                                           //이때 어떤 스텟의 수정치인지 모르기 때문에
                                                                           //KeyValuePair<EStatType, StatModifier>의 형태로
                                                                           //묶어서 전달해 줘야만 한다                                                                       
    {
        Value = value;
        Type = type;
        this.source = source;
    }
    //딕셔너리가 아니라 왜 KeyValuePair를 쓰는가?
    //1. 한 종류(단일 데이터)의 전달 이기 때문
    //딕셔너리는 여러 데이터를 담아두고 검색하기 위한 저장소에 가깝다.
    //2. 중복 허용을 위해서
    //만약 아이템 하나에 "공격력 +10" 효과와 "공격력 +5%" 효과가 동시에 들어있다면?
    //딕셔너리의 경우 EStatType.Damage라는 키가 겹쳐서 데이터가 씹히거나 에러가 난다.
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : StatBase
{
    private float lastValue;//기본 스텟에서 수정된 값
    private List<StatModifier> statModifiers;//스텟값 보정 클래스를 리스트의 형태로 저장.
                                             //해당 클래스에 있는 정보로 스텟에서 값을 조절
    private bool isDirty = true; //데이터가 변경되었으니 다시 계산(갱신) 할 필요가 있다 라는 것을 표시하는 상태 깃발
                                 //평소 (isDirty = false): 이전에 계산해둔 결과값(lastValue)을 그대로 반환합니다. (매우 빠름)


    public Stat(float value) : base(value)//스텟 생성자. 유닛이 조립될때 유닛 데이터의 값을 받게 될 것이다.
    {
        statModifiers = new List<StatModifier>();
    }


    public override float Value//스텟에 담긴 값 set부분이 없어 기본값을 바꾸지 못하며 get 으로 계산된 값을 반환한다
    {
        get
        {
            if (isDirty) // 1. 값이 오염(변경)되었다면?
            {
                lastValue = CalcModifier(); // 2. 다시 계산하고
                isDirty = false;         // 3. 깨끗한 상태로 변경
            }
            return lastValue; // 4. 보관된 값 반환
        }
    }

    //어떤 추가 장비나 이런거 했을 때  아무튼 스텟의 수정사항이 있을때 호출
    //추가는 하나씩!=>그래서 관리자(UnitStat)에서 반복해야함
    public override void AddModifier(StatModifier statModifier)
    {
        statModifiers.Add(statModifier);
        isDirty = true;
    }

    public override void RemoveModifier(StatModifier modifier)
    {
        statModifiers.Remove(modifier);
        isDirty = true;
    }

    //삭제는 한번에!(리스트의 기능을 이용)
    public override void RemoveAllModifier(object source)
    {
        statModifiers.RemoveAll(x => x.source == source);
        isDirty = true;
    }
    //x: 리스트 안에 들어있는 각각의 StatModifire객체를 가리키는 임시 이름(foreach문의 변수와 같다)
    //=> 조건을 말할게 라는 화살표
    // x.source == value 조건 true false 가 나오면된다
    // 검사 결과 조건에 맞는 모든 리스트를 삭제한다

    public override void ConsumeStat(float value) { return; }

    //스텟의 변화값을 계산! 곱연산의 경우 곱연산 수치들을 짜로 더하고 합연산의 경우 기존 수치에 바로 더한다!
    //최종적으로 그 둘을 서로 곱함!
    //ex:데미지 스텟을 예시로 기본 데미지 1 데미지 증가 4,5 ,1  데미지 배율 0.5, 0.2, 0.1
    //합연산 1+ 4+ 5+ 1  곱연산 1+0.5+0.2+0.1
    //최종 11*1.8 =19,8 의 데미지를 가짐
    protected float CalcModifier()
    {
        float result = value;
        float percent = 1f;

        foreach (var modifire in statModifiers)
        {
            if (modifire.Type == EStatModifireType.Add)
            {
                result += modifire.Value;
            }
            else if (modifire.Type == EStatModifireType.Percent)
            {
                percent += modifire.Value;
            }
        }

        return result * percent>0?result*percent:0;
    }
}

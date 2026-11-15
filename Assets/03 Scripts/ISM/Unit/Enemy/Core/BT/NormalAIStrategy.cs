using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Strategy/Normal")]
public class NormalAIStrategy : EnemyAIStrategy
{
    public override Node SetupTree(Enemy enemy)
    {
        // 유틸리티 셀렉터에 가중치 로직을 담은 노드들을 주입 
        return new UtilitySelector(new List<Node> {
            
            // 엄폐 노드: 체력이 낮거나 재장전이 필요할 때 점수 상승
            new ActionCover(enemy)//생성자
            {//생성자로 생성한 녀석을 변수에 담기 전에 내부 수정 해주기 위해 영역{} 열기 "객체초기화자"
             //(다양한 방식으로 데이터 주입이 가능하지만 아래 방식이 가장 보기 좋음)
             //변수안에 넣은뒤에 바꾸는건 .(~의)를 써서 넣으면 됨
                ScoreModifier = () => //영역을 열어서 그 안에 변수 ScoreModifier 에다가 값 넣기 () => {}는 아래에 후술
                {
                    
                     //0 points if there is nowhere to hide
                     if (!enemy.IsCoverNearby(10f, out Vector3 pos))
                        return 0f;

                    // 1. 기본적으로 피가 적으면 점수 상승 (최대 50점)
                    float hpPercent = enemy.unitStat.GetStat(EStatType.Hp) / enemy.unitStat.GetStat(EStatType.MaxHp);

                    float score = 0f;
                    score += (1f - hpPercent) * 200f;

                    return score; // 총합이 높을수록 다른 행동(사격 등)을 이길 확률이 높음
                }
            },

            // 사격 노드: 적이 보이면 기본 점수, 거리가 적절하면 보너스
            new ActionShoot(enemy)
            {
                ScoreModifier = () =>
                {
                    if (enemy.CanSeePlayer) return 80f;
                    return 0;
                }
            },

           
                // 정찰 노드: 아무것도 할 게 없을 때의 기본값 그리고 적 마지막 위치 추적
            new ActionPatrol(enemy)
            {
                ScoreModifier = () =>
                {
                if (!enemy.CanSeePlayer)
                    {
                        return 40f; // 일반 정찰(10점)보다 높은 점수로 수색 유도
                    }
                return 10f;
                }
            }

        });//new UtilitySelector(new List<Node> { 의 끝 UtilitySelector 생성자의 끝

    }
    /*
    * 문법 파헤치기: () => { ... }
    이 기호들을 하나씩 뜯어보면 의미가 명확해진다
    ScoreModifier = : ScoreModifier에다가 오른쪽에서 만든 함수를 저장한다
    ():매게변수의 종류를 넣는 곳 지금은 아무것도 없기에 매게변수가 필요 없다 라는 뜻.
    =>: 이름하여 '로켓 연산자' 또는 'Goes to'라고 읽는다. 왼쪼그이 ()의 매게변수를 가지고 (없으면 없는데로) 오른쪽 {} 의 내용을 실행하라 
    { ... }: 실제 실행될 함수의 코드 이는 만들어진 곳이 아닌 가지고 있는 쪽에서 실행됨 여기서 리턴되는 값은 원본 델리게이트의 Func<>의 <>안에 있는 데이터 형을 리턴함
    */
}
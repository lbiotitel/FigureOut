using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    //노드의 실행 결과 현황
    public enum State { Running, Success, Failure }
    public State CurrentState { get; protected set; } =State.Failure;


    // 델리게이트를 부모 레벨로 올립니다. (모든 행동 노드가 점수 계산식을 가질 수 있도록)
    public System.Func<float> ScoreModifier { get; set; }
    //이 노드가 선택되어 실행될 행동들에 대한 실행부 코드를 담을 추상코드
    public abstract State Evaluate(); //추상화로 상속받으면 이거 꼭 만들어야해!

    // 가중치 점수를 계산하는 식을 넣어 해당 노드의 가중치 계산의 기준으로 쓰임
    // 식을 넣어주는 곳은 EnemyAIStrategy 의 자식들
    public virtual float GetScore()
    {
        // 1. Strategy(람다식)에서 짜준 순수한 전술 점수를 가져옵니다.
        float baseScore = ScoreModifier != null ? ScoreModifier() : 0f;
        return baseScore;
    }
    public virtual void Abort()//노드가 강제 종료될때 청소를 위한 가상함수
    {
        CurrentState = State.Failure;
    }
}
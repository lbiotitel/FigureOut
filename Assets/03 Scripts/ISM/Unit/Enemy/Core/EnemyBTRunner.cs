using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyBTRunner : MonoBehaviour//몬스터 종류별로 가중치를 주입해주면서 각 노드를 생성하는 스크립트
                                          //BTRunner의 종류= 몬스터의 종류 라고 할수 있겠다.
{
    private Node rootNode;
    private Enemy enemy;

    [Header("AI Settings")]
    [SerializeField] private float tickRate = 0.1f; // 0.1초마다 의사결정 (성능 최적화)
    private float tickTimer;

    public void Init(Enemy enemy, EnemyAIStrategy strategy)
    {
        this.enemy = enemy;
        // 주입받은 전략(Strategy)을 통해 트리를 조립
        rootNode = strategy.SetupTree(enemy);
    }

    void Update()
    {
        tickTimer += Time.deltaTime;

        //일정 주기마다 가장 점수 높은 행동을 다시 계산
        if (tickTimer >= tickRate)
        {
            tickTimer = 0f;
            rootNode.Evaluate();//UtilitySelector의 노드들의 가중치 계산 실행 및 
        }
    }
    public void ResetRunner(EnemyAIStrategy strategy)
    {
        // 1. 타이머를 0으로 초기화해서 깨어나자마자 바로 의사결정을 하도록 만듭니다.
        // (안 그러면 이전 생(?)에서 남은 애매한 시간 뒤에 작동할 수 있습니다)
        tickTimer = 0f;

        // 2. 주입받은 전략을 통해 노드들을 완전히 새로 조립하여 덮어씌웁니다.
        // (기존의 ActionHp70Event 같은 노드들의 hasRolledDice 플래그가 전부 날아가고 새것이 됨)
        rootNode = strategy.SetupTree(enemy);
        Debug.Log("러너 리셋됨");
    }
}


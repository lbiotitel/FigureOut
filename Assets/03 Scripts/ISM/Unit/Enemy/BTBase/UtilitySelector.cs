using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UtilitySelector : Node//이 클래스가 BT의 관리자 라고 볼수 있다. 또한 rootNode 이기도 하다
{
    protected List<Node> nodes = new List<Node>();
    //생성한 행동노드들을 모아두는 인스턴스를 만드는 설계도 이 인스턴스는 행동 노드 인스턴스들중 가장 높은 가중치를 가진
    //노드의 실행부를 실행시키기도 한다. UtilitySelector.Evaluate() 한다면 가중치가 가장 높은 노드가 실행될 것이다.
   
    public UtilitySelector(List<Node> nodes) => this.nodes = nodes;//생성자로 해당 기능을 통해 노드가 구성된다.
                                                                   //생성한 인스턴스에 어떤 노드들이 어떠한 값으로
                                                                   //들어 왔는가에 따라서 BT가 달라진다.
                                                                   //구성하는 과정은 EnemyAIStrategy의 자식들이 한다.
    
    public override State Evaluate()//노드들 중에서 가장 가중치가 높은 노드를 선택해주는 함수
    {
        
        //점수 순으로 내림차순 정렬하여 가장 높은 점수 노드 선택
        Node bestNode = nodes.OrderByDescending(n => n.GetScore())//리스트를 내림차순으로 정렬하는데 그 기준을 Node.GetScore 함수 실행 결과 값으로 해
                                       .FirstOrDefault();//그렇게 정렬된 리스트의 0번 인덱스의 요소를 반환해 그런데 결과가 null이면 null또는0을 반환해

        if (bestNode != null && bestNode.GetScore() > 0)
        {
            
            return bestNode.Evaluate();//Execute the node with the highest score
        }
        return State.Failure;
    }
}
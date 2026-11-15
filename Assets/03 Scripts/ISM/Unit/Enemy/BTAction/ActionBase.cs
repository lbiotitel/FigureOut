using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBase : Node//클래스 이름 바꾸고
{
    private Enemy enemy;
   

    public ActionBase(Enemy enemy)//생성자 이름도 맘에 드는걸로 바꿔라!
    {
        this.enemy = enemy;
    }

    //기능 해당 노드가 가중치 충족되어 사용될때 기동할 기능을 적으면 된다!
    public override State Evaluate()
    {
        
        return State.Running; //상태가 실행 중인지, 실패했는지, 성공했는지 반환
    }
}
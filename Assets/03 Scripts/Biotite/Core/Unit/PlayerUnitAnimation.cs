using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAnimType
{ 
    IDLE,
    WALK,
    SPRINT,
    DASH
}

public class PlayerUnitAnimation : MonoBehaviour
{
    private Animator ac;

    private void Awake()
    {
        ac = GetComponent<Animator>();
    }


    public void PlayAnimation(EAnimType animType)
    {
        StopAnim();

        if (animType == EAnimType.IDLE)
        {
            ac.SetBool("Idle", true);
        }
        else if (animType == EAnimType.WALK)
        {
            ac.SetBool("Walk", true);
        }
        else if (animType == EAnimType.SPRINT)
        {
            ac.SetBool("Sprint", true);
        }
        else if (animType == EAnimType.DASH)
        {
            ac.SetTrigger("Dash");
        }
    }


    private void StopAnim()
    {
        ac.SetBool("Walk",false);
        ac.SetBool("Idle",false);
        ac.SetBool("Sprint",false);
        ac.SetBool("Dash", false);
    }
}

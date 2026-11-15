public class PlayerStateMachine 
{
    public PlayerStateBase CurrentState { get; private set; }

    public void Init(PlayerStateBase stateMachine)
    {
        CurrentState = stateMachine;
        CurrentState.Enter();
    }

    public void ChangeMachine(PlayerStateBase newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}

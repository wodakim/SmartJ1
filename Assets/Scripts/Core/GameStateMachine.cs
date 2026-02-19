using System;
using EntropySyndicate.Data;

namespace EntropySyndicate.Core
{
    public class GameStateMachine
    {
        public GameFlowState CurrentState { get; private set; }
        public event Action<GameFlowState> OnStateChanged;

        public void SetState(GameFlowState nextState)
        {
            if (CurrentState == nextState)
            {
                return;
            }

            CurrentState = nextState;
            OnStateChanged?.Invoke(CurrentState);
        }
    }
}

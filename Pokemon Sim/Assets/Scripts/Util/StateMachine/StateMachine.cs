using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils.StateMachine
{
    public class StateMachine<T>
    {
        public State<T> CurrentState { get; private set; }

        public Stack<State<T>> StateStack { get; private set; }

        T owner;
        public StateMachine(T owner)
        {
            this.owner = owner;
            StateStack = new Stack<State<T>>();
        }

        public void Push(State<T> newState)
        {
            StateStack.Push(newState);
            CurrentState = newState;
            CurrentState.EnterState(owner);
        }

        public void Pop()
        {
            CurrentState.ExitState();
            StateStack.Pop();
            CurrentState = StateStack.Peek();
        }

        public void Execute()
        {
            CurrentState?.Execute();
        }

        public void ChangeState(State<T> newState)
        {
            if (CurrentState != null)
            {
                StateStack.Pop();
                CurrentState.ExitState();
            }
            StateStack.Push(newState);
            CurrentState = StateStack.Peek();
            CurrentState.EnterState(owner);
        }

        public IEnumerator PushAndWait(State<T> newState)
        {
            var currState = CurrentState;
            Push(newState);
            yield return new WaitUntil(() => currState == CurrentState);
        }

        public State<T> GetPreviousState()
        {
            return StateStack.ElementAt(1);
        }

    }
}

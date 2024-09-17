using UnityEngine;

namespace Utils.StateMachine
{
    public class State<T> : MonoBehaviour
    {

        public virtual void EnterState(T owner) { }
        public virtual void Execute() { }
        public virtual void ExitState() { }
    }

}


namespace SCOM.Utils
{
	/**
	 * Finite state machine inspired in http://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/
	 */
	public class FiniteStateMachine<T> {

		public FSMState<T> CurrentState
		{
			get {
				return m_currentState;
			}
		}
        public FSMState<T> PreviousState
        {
            get
            {
                return m_previousState;
            }
        }

        private T m_owner;
		private FSMState<T> m_currentState;
		private FSMState<T> m_previousState;
		private FSMState<T> m_globalState;

		public FiniteStateMachine(T owner, FSMState<T> initialState)
		{
			m_owner = owner;
			m_currentState = initialState;
			m_previousState = null;
			m_globalState = null;
		}

		public void ProcessState() {
			if (m_globalState != null) {
				m_globalState.Execute (m_owner);
			}
			if (m_currentState != null) {
				m_currentState.Execute (m_owner);
			}
		}

		public void ChangeState(FSMState<T> newState) {
			if (m_currentState == null) {
				return;
			}

			m_currentState.Exit (m_owner);
            m_previousState = m_currentState;
			m_currentState = newState;

			if (m_currentState != null) {
				m_currentState.Enter (m_owner);
			}
		}

		public void RevertToPreviousState() {
			if (m_previousState != null) {
				ChangeState (m_previousState);
			}
		}
	}
}
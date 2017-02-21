namespace SCOM.Utils
{
	/**
	 * Interface for the finite state machine state
	 *
	 * It defines the enter, execute and exit methods that will
	 * be called by the FSM
	 */
	public interface FSMState<T> {
		void Enter (T entity);
		void Execute (T entity);
		void Exit (T entity);
	}
}

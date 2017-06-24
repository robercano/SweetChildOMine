using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;

public class Floater : MonoBehaviour {

    private StateStorage m_state = null;
    private IStateStorageHandler[] m_stateHandlers;

    // Use this for initialization
    void Start () {
        m_stateHandlers = GetComponents<IStateStorageHandler>();

        m_state = new StateStorage();
        m_state["movementType"] = FloatMovement.MovementType.WanderFreely;
        m_state["targetPosition"] = Vector2.zero;
    }
	
	// Update is called once per frame
	void Update () {
        ProcessState();
    }

    void ProcessState()
    {
        foreach (IStateStorageHandler handler in m_stateHandlers)
        {
            handler.ProcessState(m_state);
        }
    }
}

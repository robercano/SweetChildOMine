using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;
using com.kleberswf.lib.core;

public sealed class FloatMovementStateIdle : Singleton<FloatMovementStateIdle>, FSMState<FloatMovement> {

	public void Enter(FloatMovement floatMovement)
	{
        floatMovement.StopMovement();
        floatMovement.PlayAnimation("");
	}

	public void Execute(FloatMovement floatMovement)
	{
	}

	public void Exit(FloatMovement floatMovement)
	{
	}
}
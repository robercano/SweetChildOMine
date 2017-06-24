using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;
using com.kleberswf.lib.core;

public sealed class FloatMovementStateWanderRight : Singleton<FloatMovementStateWanderRight>, FSMState<FloatMovement> {

	public void Enter(FloatMovement floatMovement)
	{
        floatMovement.StartWanderingRight();
        floatMovement.PlayAnimation("monsterWalk");
	}

	public void Execute(FloatMovement floatMovement)
	{
        floatMovement.CheckWanderingDirectionChange();
	}

	public void Exit(FloatMovement floatMovement)
	{
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;
using com.kleberswf.lib.core;

public sealed class FloatMovementStateWanderLeft : Singleton<FloatMovementStateWanderLeft>, FSMState<FloatMovement> {

	public void Enter(FloatMovement floatMovement)
	{
        floatMovement.StartWanderingLeft();
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
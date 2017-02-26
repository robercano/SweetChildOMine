using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;
using com.kleberswf.lib.core;

public sealed class MinerStateDig : Singleton<MinerStateDig>, FSMState<Miner> {

	public void Enter(Miner miner)
	{
		miner.Stop ();
		miner.PlayAnimation("minerDig");
	}

	public void Execute(Miner miner)
	{
        if (miner.HasReachedMovementTarget())
        {
            miner.ChangeState(MinerStateIdle.Instance);
        }
        else
        {
            miner.FallDown();
        }
	}

	public void Exit(Miner miner)
	{
		miner.StopDigging ();
	}
}
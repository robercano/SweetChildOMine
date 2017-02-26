using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;
using com.kleberswf.lib.core;

public sealed class MinerStateBuild : Singleton<MinerStateBuild>, FSMState<Miner> {
	public void Enter(Miner miner)
	{
		miner.Stop ();
		miner.DisableVisibleTarget ();
		miner.OnActionStarted ();
		miner.PlayAnimation("minerBuild");
	}

	public void Execute(Miner miner)
	{
		if (miner.BuildableTarget == null) {
			miner.ChangeState(MinerStateIdle.Instance);
		}
	}

	public void Exit(Miner miner)
	{
        miner.OnActionTerminated();
    }
}
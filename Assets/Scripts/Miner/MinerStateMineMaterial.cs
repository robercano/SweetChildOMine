using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;
using com.kleberswf.lib.core;

public sealed class MinerStateMineMaterial : Singleton<MinerStateMineMaterial>, FSMState<Miner> {

	public void Enter(Miner miner)
	{
		miner.Stop ();
		miner.DisableVisibleTarget ();
		miner.OnActionStarted ();
		miner.PlayAnimation("minerMine");
	}

	public void Execute(Miner miner)
	{
		if (miner.MineableTarget == null) {
			miner.ChangeState(MinerStateIdle.Instance);
		}
	}

	public void Exit(Miner miner)
	{
		miner.OnActionTerminated ();
	}
}
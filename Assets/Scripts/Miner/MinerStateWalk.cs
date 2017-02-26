using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;
using com.kleberswf.lib.core;

public class MinerStateWalk : Singleton<MinerStateWalk>, FSMState<Miner> {

	public virtual void Enter(Miner miner)
	{
		miner.PlayAnimation("minerWalk");
	}

	public virtual void Execute(Miner miner)
	{
		if (miner.HasReachedMovementTarget()) {
			miner.ChangeState(MinerStateIdle.Instance);
		} else {
			miner.Walk ();
		}
	}

	public virtual void Exit(Miner miner)
	{
		miner.Stop ();
	}
}
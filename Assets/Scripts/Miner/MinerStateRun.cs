using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;
using com.kleberswf.lib.core;

public class MinerStateRun : Singleton<MinerStateRun>, FSMState<Miner> {

	public virtual void Enter(Miner miner)
	{
        bool showTarget = miner.MineableTarget == null &&
                          miner.BuildableTarget == null;
		miner.SetMovementTarget (Camera.main.ScreenToWorldPoint (Input.mousePosition), showTarget);
		miner.PlayAnimation("minerRun");
	}

	public virtual void Execute(Miner miner)
	{
		if (miner.HasReachedMovementTarget()) {
			miner.Stop ();
			miner.ChangeState(MinerStateIdle.Instance);
		}
		else
		{
			miner.Run ();
		}
	}

	public virtual void Exit(Miner miner)
	{
	}
}
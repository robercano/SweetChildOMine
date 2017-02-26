using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;
using com.kleberswf.lib.core;

public class MinerStateRun : Singleton<MinerStateRun>, FSMState<Miner> {

	public virtual void Enter(Miner miner)
	{
		miner.SetMovementTarget (Camera.main.ScreenToWorldPoint (Input.mousePosition));
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
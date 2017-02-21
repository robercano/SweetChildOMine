using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;

public sealed class MinerStateRun : FSMState<Miner> {

	static readonly MinerStateRun instance = new MinerStateRun();
	public static MinerStateRun Instance {
		get {
			return instance;
		}
	}

	static MinerStateRun() {}
	private MinerStateRun() {}

	public void Enter(Miner miner)
	{
		miner.SetMovementTarget (Camera.main.ScreenToWorldPoint (Input.mousePosition));
		miner.PlayAnimation("minerRun");
	}

	public void Execute(Miner miner)
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

	public void Exit(Miner miner)
	{
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;

public sealed class MinerStateBuild : FSMState<Miner> {

	static readonly MinerStateBuild instance = new MinerStateBuild();
	public static MinerStateBuild Instance {
		get {
			return instance;
		}
	}

	static MinerStateBuild() {}
	private MinerStateBuild() {}

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
	}
}
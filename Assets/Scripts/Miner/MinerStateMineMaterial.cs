using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;

public sealed class MinerStateMineMaterial : FSMState<Miner> {

	static readonly MinerStateMineMaterial instance = new MinerStateMineMaterial();
	public static MinerStateMineMaterial Instance {
		get {
			return instance;
		}
	}

	static MinerStateMineMaterial() {}
	private MinerStateMineMaterial() {}

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
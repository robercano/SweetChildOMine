using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;

public sealed class MinerStateIdle : FSMState<Miner> {

	static readonly MinerStateIdle instance = new MinerStateIdle();
	public static MinerStateIdle Instance {
		get {
			return instance;
		}
	}

	static MinerStateIdle() {}
	private MinerStateIdle() {}

	public void Enter(Miner miner)
	{
		miner.DisableVisibleTarget ();
		miner.PlayAnimation("minerIdle");
	}

	public void Execute(Miner miner)
	{
		if (miner.PeakInputEvent() == Miner.InputEvent.LeftClick) {
			miner.ConsumeInputEvent ();
			miner.ChangeState (MinerStateWalk.Instance);
		}
	}

	public void Exit(Miner miner)
	{
	}
}
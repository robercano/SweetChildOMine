using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;

public sealed class MinerStateWalk : FSMState<Miner> {

	static readonly MinerStateWalk instance = new MinerStateWalk();
	public static MinerStateWalk Instance {
		get {
			return instance;
		}
	}

	static MinerStateWalk() {}
	private MinerStateWalk() {}

	public void Enter(Miner miner)
	{
		miner.SetMovementTarget (Camera.main.ScreenToWorldPoint (Input.mousePosition));
		miner.PlayAnimation("minerWalk");
	}

	public void Execute(Miner miner)
	{
		if (miner.HasReachedMovementTarget()) {
			miner.Stop ();
			miner.ChangeState(MinerStateIdle.Instance);
		}
		else
		{
			miner.Walk ();
		}
	}

	public void Exit(Miner miner)
	{
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;

public class MinerStateWalk : FSMState<Miner> {

	static readonly MinerStateWalk instance = new MinerStateWalk();
	public static MinerStateWalk Instance {
		get {
			return instance;
		}
	}

	static MinerStateWalk() {}
	protected MinerStateWalk() {}

	public virtual void Enter(Miner miner)
	{
		miner.SetMovementTarget (Camera.main.ScreenToWorldPoint (Input.mousePosition));
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;

public sealed class MinerStateDig : FSMState<Miner> {

	static readonly MinerStateDig instance = new MinerStateDig();
	public static MinerStateDig Instance {
		get {
			return instance;
		}
	}

	static MinerStateDig() {}
	private MinerStateDig() {}

	public void Enter(Miner miner)
	{
		miner.Stop ();
		miner.PlayAnimation("minerDig");
	}

	public void Execute(Miner miner)
	{
        if (miner.HasReachedMovementTarget())
        {
            miner.ChangeState(MinerStateIdle.Instance);
        }
        else
        {
            miner.FallDown();
        }
	}

	public void Exit(Miner miner)
	{
		miner.StopDigging ();
	}
}
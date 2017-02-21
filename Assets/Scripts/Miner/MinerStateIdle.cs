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
		miner.Stop ();
		miner.DisableVisibleTarget ();
		miner.PlayAnimation("minerIdle");
	}

	public void Execute(Miner miner)
	{
	}

	public void Exit(Miner miner)
	{
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;
using com.kleberswf.lib.core;

public sealed class MinerStateIdle : Singleton<MinerStateIdle>, FSMState<Miner> {

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
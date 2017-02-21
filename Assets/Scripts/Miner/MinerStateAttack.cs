using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;

public sealed class MinerStateAttack : FSMState<Miner> {

	static readonly MinerStateAttack instance = new MinerStateAttack();
	public static MinerStateAttack Instance {
		get {
			return instance;
		}
	}

	static MinerStateAttack() {}
	private MinerStateAttack() {}

	public void Enter(Miner miner)
	{
        if (miner.GetPreviousState() != MinerStateAttack.Instance)
        {
            miner.Stop();
            miner.PlayAnimation("minerAttack");
        }
	}

	public void Execute(Miner miner)
	{
	}

	public void Exit(Miner miner)
	{
	}
}
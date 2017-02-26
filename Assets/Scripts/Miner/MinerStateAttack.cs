using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;
using com.kleberswf.lib.core;

#pragma warning disable CS0252
public sealed class MinerStateAttack : Singleton<MinerStateAttack>, FSMState<Miner>  {

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
#pragma warning restore
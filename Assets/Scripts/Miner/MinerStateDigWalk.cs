using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCOM.Utils;

public sealed class MinerStateDigWalk : MinerStateRun {

	static readonly MinerStateDigWalk instance = new MinerStateDigWalk();
	new public static MinerStateDigWalk Instance {
		get {
			return instance;
		}
	}

	static MinerStateDigWalk() {}
	private MinerStateDigWalk() {}

	public override void Enter(Miner miner)
	{
		base.Enter (miner);
	}

	public override void Execute(Miner miner)
	{
		base.Execute (miner);
	}

	public override void Exit(Miner miner)
	{
		base.Exit (miner);
	}
}
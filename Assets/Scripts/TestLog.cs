using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

public class TestLog : MonoBehaviour {

	private bool called = false;

	public PlayMakerFSM fsm;

	public void CallLog(string vtff, int petibatard){

		if(!this.called){

			this.fsm.SendEvent("C est la mierda");

		}

		this.called = true;

		Debug.Log("TestLog.CallLog - ALLOOWWW !"+vtff+" "+petibatard);

	}
}

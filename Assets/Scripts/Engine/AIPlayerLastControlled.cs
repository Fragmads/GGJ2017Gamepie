using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerLastControlled : GamePlayer {


	#region Properties 

	private float timeUntilNextMove;

	private float lastChargeTime = 0f;

	[Header ("AIPlayerLastControlled")]
	[SerializeField]
	private float minAdditionnalWait = 1.5f;
	[SerializeField]
	private float maxAdditionnalWait = 5f;

	#endregion

	public void Start(){

		this.timeUntilNextMove = this.rndChargeLevel() + Random.Range(this.minAdditionnalWait, this.maxAdditionnalWait);

		// Recursive call
		this.Invoke("RandomMove", this.timeUntilNextMove);
	}

	private void RandomMove(){

		if(this.ControlledSpots.Count > 0){
			int rnd = Random.Range(0,100);

			int index = this.ControlledSpots.Count - 1;

			if(rnd < 30 && this.ControlledSpots.Count > 1){
				index = this.ControlledSpots.Count - 2;
			}

			this.ControlledSpots[index].ReleaseWave(this.lastChargeTime);

			this.timeUntilNextMove = this.rndChargeLevel() + Random.Range(this.minAdditionnalWait, this.maxAdditionnalWait);

			// Recursive call
			this.Invoke("RandomMove", this.timeUntilNextMove);

		}

	}


	public float rndChargeLevel(){

		int rnd = Random.Range(0,100);

		// 20%, 40%, 40%

		if(rnd < 20){
			this.lastChargeTime =GameManager.Instance.Level1ChargeTime;
			return GameManager.Instance.Level1ChargeTime;
		}
		else if(rnd < 60){
			this.lastChargeTime =GameManager.Instance.Level2ChargeTime;
			return GameManager.Instance.Level2ChargeTime;
		}
		else if(rnd >= 60){
			this.lastChargeTime =GameManager.Instance.Level3ChargeTime;
			return GameManager.Instance.Level3ChargeTime;
		}

		// Default case
		return GameManager.Instance.Level1ChargeTime;

	}


}

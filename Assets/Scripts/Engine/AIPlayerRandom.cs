using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerRandom : GamePlayer {

	#region Properties

	private float timeUntilNextMove;

	private float lastChargeTime = 0f;

	[Header ("AIPlayerRandom")]
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
			int rnd = Random.Range(0, this.ControlledSpots.Count -1);

			this.ControlledSpots[rnd].ReleaseWave(this.lastChargeTime);

			this.timeUntilNextMove = this.rndChargeLevel() + Random.Range(this.minAdditionnalWait, this.maxAdditionnalWait);

			// Recursive call
			this.Invoke("RandomMove", this.timeUntilNextMove);

		}

	}


	public float rndChargeLevel(){

		int rnd = Random.Range(0,2);

		switch (rnd){
			case 0:
				this.lastChargeTime =GameManager.Instance.Level1ChargeTime;
				return GameManager.Instance.Level1ChargeTime;
			break;
			case 1:
				this.lastChargeTime =GameManager.Instance.Level2ChargeTime;
				return GameManager.Instance.Level2ChargeTime;
			break;
			case 2:
				this.lastChargeTime =GameManager.Instance.Level3ChargeTime;
				return GameManager.Instance.Level3ChargeTime;
			break;

		}

		// Default case
		return GameManager.Instance.Level1ChargeTime;

	}

}

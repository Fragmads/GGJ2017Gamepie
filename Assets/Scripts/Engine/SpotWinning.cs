using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotWinning : ControlSpot {

	public override void OnConquered (bool releaseWave = true)
	{
		base.OnConquered (releaseWave);

		if(this.CurrentOwner == GamePlayer.UserPlayer){

			GameManager.Instance.Win();

		}

	}

}

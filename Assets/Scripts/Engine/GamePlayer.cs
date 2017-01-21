using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : MonoBehaviour {

	#region Properties

	public static GamePlayer UserPlayer;

	[Header ("GamePlayer")]
	[SerializeField]
	private bool isUser = false;
	public bool IsUser{
		get {
			return this.isUser;
		}
	}

	[SerializeField]
	public Color PlayerMainColor;


	public List<ControlSpot> ControlledSpots = new List<ControlSpot>();

	#endregion

	public void Awake(){

		if(this.isUser){

			if(GamePlayer.UserPlayer != null){
				GameObject.Destroy(GamePlayer.UserPlayer);
				Debug.LogWarning("GamePlayer.Awake - Another User player was used, it will be removed by this one");
			}

			GamePlayer.UserPlayer = this;
		}

	}

	public void OnCaptureSpot(ControlSpot spot){
		this.ControlledSpots.Add(spot);
	}

	public void OnLooseSpot(ControlSpot spot){
		this.ControlledSpots.Remove(spot);

		if(this.ControlledSpots.Count == 0){
			this.PlayerLost();

		}

	}

	public void PlayerLost(){

		// Game over
		if(this.isUser){

			// TODO Game over

		}
		else{
			// Remove this player from the game, he should stop playing and his wave should be removed
			GameManager.Instance.CurrentLevel.players.Remove(this);

		}

	}

}

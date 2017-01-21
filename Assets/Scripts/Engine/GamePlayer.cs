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

	private bool isClicking = false;
	public bool IsClicking{
		get {
			return this.isClicking;
		}
	}

	private ControlSpot chargingSpot;

	// Current playthrough
	private int currentLevel;

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
			GameManager.Instance.GameOver();

		}
		else{
			// Remove this player from the game, he should stop playing and his wave should be removed
			GameManager.Instance.CurrentLevel.players.Remove(this);

			// Basic win condition
			if(GameManager.Instance.CurrentLevel.players.Count < 2 && GameManager.Instance.CurrentLevel.players.Contains(GamePlayer.UserPlayer)){
				GameManager.Instance.Win();
			}

		}

	}

	#region ClickLock Handle

	public void ClickingOnSpot(ControlSpot spot){

		// If this spot can be used by the player
		if(spot != null && spot.CurrentOwner != null && spot.CurrentOwner == this && spot.IsTaken) {

			if(this.isClicking){

				if(this.chargingSpot.CurrentOwner == this && this.chargingSpot.IsTaken){
					this.chargingSpot.ReleaseWave(this.chargingSpot.ClickedTime);
				}

				if(spot != this.chargingSpot && spot != null && spot.CurrentOwner != null && spot.CurrentOwner == this && spot.IsTaken){
					this.chargingSpot = spot;
				}
				else{
					this.chargingSpot = null;
					this.isClicking = false;
				}

			}
			else {
				this.chargingSpot = spot;
				this.isClicking = true;
			}
		}

	}

	#endregion

}

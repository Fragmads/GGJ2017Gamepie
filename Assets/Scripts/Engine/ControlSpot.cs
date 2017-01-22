using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSpot : MonoBehaviour {


	#region Properties
	[Header ("ControlSpot")]
	public GamePlayer CurrentOwner = null;

	private bool isTaken = false;
	public bool IsTaken{
		get {
			return this.isTaken;
		}
	}

	private float currentHP;
	[SerializeField]
	private float maxHP;

	[SerializeField]
	private float decayOvercharge = 10f;
	[SerializeField]
	private float MaxOvercharge = 100f;

	private float overcharge = 0f;

	[Space (15)]

	private float clickedTime = 0f;
	public float ClickedTime{
		get {
			return this.clickedTime;
		}
	}

	private bool clicked = false;

	[SerializeField]
	private Wave wavePrefab;

	[SerializeField]
	private Transform WaveParent;

	[SerializeField]
	private SpriteRenderer spotSprite;

	[SerializeField]
	private SpriteRenderer OverchargeFeedback;

	#endregion

	private void Start(){

		this.currentHP = this.maxHP;

		this.OverchargeFeedback.enabled = false;

		if(this.CurrentOwner != null){
			// Conquered, but don't release a wave yet
			this.OnConquered(false);
		}
		else {
			this.OnGoesNeutral(null);
		}
	}


	public void OnTakeDamage(float damageTaken, GamePlayer sender){

		Debug.Log("ControlSpot.OnTakeDamage : "+damageTaken+", is sender player : "+sender.IsUser);

		// If this is a neutral wave, take the claim
		if(this.CurrentOwner == null){
			// The sender start it's claim
			this.CurrentOwner = sender;
			this.currentHP = damageTaken;
			this.isTaken = false;

		}
		// If this is an ennemy wave
		else if(sender != this.CurrentOwner){

			this.currentHP -= damageTaken;

			// If this spot reach 0hp
			if(this.currentHP <= 0f){
				this.OnGoesNeutral(sender);
			}
		}
		// If this is an ally wave
		else if(sender == this.CurrentOwner){

			this.currentHP += damageTaken;

			// If this hit is over heal, store it as overcharge
			if(this.currentHP > this.maxHP){
				this.overcharge += this.currentHP - this.maxHP;

				// Clamp the overcharge value
				this.overcharge = Mathf.Clamp(this.overcharge, 0f, this.MaxOvercharge);

				this.currentHP = this.maxHP;

				// TODO feedback on overcharge

			}

			this.currentHP = Mathf.Min(this.currentHP + damageTaken, this.maxHP);

			// If the player conquered this point
			if(!this.isTaken && this.currentHP >= GameManager.Instance.NeutralSpotConquerPoint) {

				this.OnConquered();

			}

		}

	}



	public virtual void OnConquered(bool releaseWave = true){
		// Change the color
		this.isTaken = true;
		this.clicked = false;
		this.clickedTime = 0f;
		this.spotSprite.color = this.CurrentOwner.PlayerMainColor;
		this.OverchargeFeedback.color = this.CurrentOwner.PlayerMainColor;

		// Release a wave
		if(releaseWave){
			this.ReleaseWave(GameManager.Instance.Level1ChargeTime);
		}

		this.CurrentOwner.OnCaptureSpot(this);

	}

	public void OnGoesNeutral(GamePlayer contestant){

		if(this.CurrentOwner != null){

			// If this is a user spot that is lost
			if(this.CurrentOwner == GamePlayer.UserPlayer){
				SoundManager.Instance.PlaySpotTaken();
			}

			this.CurrentOwner.OnLooseSpot(this);
		}
		this.currentHP = Mathf.Abs(this.currentHP);

		this.overcharge = 0f;

		this.CurrentOwner = contestant;
		this.isTaken = false;

		// Change the color
		this.spotSprite.color = GameManager.Instance.NeutralMainColor;
		this.OverchargeFeedback.color = GameManager.Instance.NeutralMainColor;

	}


	public void Update(){

		// If this is owned by the user
		if(this.CurrentOwner == GamePlayer.UserPlayer && this.isTaken && GamePlayer.UserPlayer.IsClicking) {
			// Handle click time
			if(this.clicked){

				this.clickedTime += Time.deltaTime;
			
				FeedbackManager.Instance.UpdateJaugeBar(this.ClickedTime);

			}


		}
		else{
			this.clickedTime = 0f;
		}

		// Overcharge 
		if(this.overcharge > 0f){
			// Decay the overcharge value

			this.OverchargeFeedback.enabled = true;

			this.overcharge = Mathf.MoveTowards(this.overcharge, 0f, this.decayOvercharge * Time.deltaTime);

			Color oldCol = this.OverchargeFeedback.color;
			float alphaVal = (this.overcharge/this.MaxOvercharge) * 1f ;
			this.OverchargeFeedback.color = new Color(oldCol.r, oldCol.g, oldCol.b, alphaVal);


		}
		else{
			this.OverchargeFeedback.enabled = false;
		}

	}

	#region SendWave

	public void OnMouseDown(){
		
		if(this.CurrentOwner != null && this.CurrentOwner == GamePlayer.UserPlayer){
			this.CurrentOwner.ClickingOnSpot(this);
			this.clicked = true;
		}

	}

	public void NotClickedAnymore(){
		this.clicked = false;
		this.clickedTime = 0f;
	}

	public virtual void ReleaseWave(float holdTime){

		this.clicked = false;
		this.clickedTime = 0f;

		if(!this.isTaken){
			return;
		}

		float waveValue = 0f;

		// If we have at least one level of charge
		if(holdTime >= GameManager.Instance.Level1ChargeTime){

			// Charge level
			if(holdTime >= GameManager.Instance.Level3ChargeTime){
				waveValue = GameManager.Instance.Level3Value;
			}
			else if(holdTime >= GameManager.Instance.Level2ChargeTime){
				waveValue = GameManager.Instance.Level2Value;
			}
			else if(holdTime >= GameManager.Instance.Level1ChargeTime){
				waveValue = GameManager.Instance.Level1Value;
			}


			Debug.Log("ControlSpot.ReleaseWave - HoldTime : "+holdTime);

			// PLay the wave launch sound
			if(this.CurrentOwner != null && this.CurrentOwner == GamePlayer.UserPlayer){

				if(this.overcharge > 0f){
					SoundManager.Instance.PlayOverchargeWave();
					//SoundManager.Instance.PlayWaveLaunch();
				}

				SoundManager.Instance.PlayWaveLaunch();
			}

			//

			List<Wave> waves = WavePool.Instance.GetWaveImpule();

			foreach(Wave w in waves){
				w.SetDirection(this.CurrentOwner, this, waveValue, this.overcharge/2f);
			}

			this.overcharge = 0f;

		}
		else{
			Debug.Log("ControlSpot.ReleaseWave - Not enough charge time, do nothing : holdTime : "+holdTime);
		}

	}



	#endregion

}

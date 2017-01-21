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

	private float ClickedTime = 0f;

	private bool clicked = false;

	[SerializeField]
	private Wave wavePrefab;

	[SerializeField]
	private Transform WaveParent;

	[SerializeField]
	private SpriteRenderer spotSprite;

	#endregion

	private void Start(){

		this.currentHP = this.maxHP;

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
			
			this.currentHP = Mathf.Min(this.currentHP+ damageTaken, this.maxHP);

			// If the player conquered this point
			if(!this.isTaken && this.currentHP >= GameManager.Instance.NeutralSpotConquerPoint) {

				this.OnConquered();

			}

		}

	}



	public void OnConquered(bool releaseWave = true){
		// Change the color
		this.isTaken = true;
		this.clicked = false;
		this.spotSprite.color = this.CurrentOwner.PlayerMainColor;

		// Release a wave
		if(releaseWave){
			this.ReleaseWave(GameManager.Instance.Level1ChargeTime);
		}

		this.CurrentOwner.OnCaptureSpot(this);

	}

	public void OnGoesNeutral(GamePlayer contestant){

		if(this.CurrentOwner != null){
			this.CurrentOwner.OnLooseSpot(this);
		}
		this.currentHP = Mathf.Abs(this.currentHP);

		this.CurrentOwner = contestant;
		this.isTaken = false;

		// Change the color
		this.spotSprite.color = GameManager.Instance.NeutralMainColor;

	}


	public void FixedUpdate(){

		// If this is owned by the user
		if(this.CurrentOwner == GamePlayer.UserPlayer && this.isTaken && this.clicked){

			if(Input.GetMouseButton(0)){
				this.ClickedTime += Time.fixedDeltaTime;
			}
			else{
				this.clicked = false;

				this.ReleaseWave(this.ClickedTime);

				this.ClickedTime = 0f;

			}

		}


	}

	#region SendWave

	public void OnMouseDown(){

		this.clicked = true;

	}


	public virtual void ReleaseWave(float holdTime){

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

			List<Wave> waves = new List<Wave>(); 

			Vector3 posSpot = this.transform.position;

			// Make 4 wave, to look like a circle
			for(int i=0; i<8; ++i){

				Wave w = GameObject.Instantiate<Wave>(this.wavePrefab);
				w.transform.position = posSpot;

				w.transform.SetParent(this.WaveParent, true);

				w.transform.Rotate(new Vector3(0f, 0f, (i/8f))*360);

				float angle = (i/8f)* (Mathf.PI * 2);

				w.SetDirection(this.CurrentOwner, this, 10000f*(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0)), waveValue); 

				waves.Add(w);
			}

			foreach(Wave w in waves){
				w.SetSiblings(waves);
			}
		}
		else{
			Debug.Log("ControlSpot.ReleaseWave - Not enough charge time, do nothing : holdTime : "+holdTime);

		}


	}



	#endregion

}

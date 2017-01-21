using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSpot : MonoBehaviour {


	#region Properties
	[Header ("ControlSpot")]
	public GamePlayer CurrentOwner = null;

	private bool isTaken = false;

	private float currentHP;
	[SerializeField]
	private float maxHP;

	private float ClickedTime = 0f;

	private bool clicked = false;

	[SerializeField]
	private Wave wavePrefab;

	#endregion

	private void Awake(){

		this.currentHP = this.maxHP;

		if(this.CurrentOwner != null){
			this.isTaken = true;
		}

	}


	public void OnTakeDamage(float damageTaken, GamePlayer sender){

		// If this is an ennemy wave
		if(sender != this.CurrentOwner){

			this.currentHP -= damageTaken;

			// If this spot reach 0hp
			if(this.currentHP >= 0f){

				this.OnGoesNeutral();

			}

		}
		else{
			
			this.currentHP = Mathf.Min(this.currentHP+ damageTaken, this.maxHP);

			// If the player conquered this point
			if(this.currentHP >= GameManager.Instance.NeutralSpotConquerPoint) {

				this.OnConquered();

			}

		}

	}



	public void OnConquered(){


	}

	public void OnGoesNeutral(){

		this.CurrentOwner = null;
		this.isTaken = false;

	}



	public void FixedUpdate(){

		// If this is owned by the user
		if(this.CurrentOwner == GamePlayer.UserPlayer && this.clicked){

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

		Debug.Log("ControlSpot.ReleaseWave - HoldTime : "+holdTime);

		List<Wave> waves = new List<Wave>(); 

		Vector3 posSpot = this.transform.position;

		// Make 16 wave, to look like a circle
		for(int i=0; i<16; ++i){

			Wave w = GameObject.Instantiate<Wave>(this.wavePrefab);
			w.transform.position = posSpot;

			w.transform.Rotate(new Vector3(0f, 0f, (i/16f))*360);

			float angle = (i/16f)* (Mathf.PI * 2);

			w.SetDirection(this.CurrentOwner, this, 10000f*(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f))); 

			waves.Add(w);
		}

		foreach(Wave w in waves){
			w.SetSiblings(waves);
		}


	}

	#endregion

}

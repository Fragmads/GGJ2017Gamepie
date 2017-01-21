using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {

	#region Properties

	private GamePlayer owner;
	private ControlSpot startSpot;
	private ControlSpot arrivalStop;

	private float MovementSpeed =10f;

	private Transform tr;

	private float Value = 30f;

	private List<Wave> collidedList = new List<Wave>();

	private WaveGenerator Generator;

	private bool destroyed = false;

	#endregion

	private void Awake(){
		this.tr = this.transform;

	}


	public void SetDirection(GamePlayer owner, ControlSpot startSpot, ControlSpot arrivalSpot){

		this.owner = owner;
		this.startSpot = startSpot;
		this.arrivalStop = arrivalSpot;

	}


	public void FixedUpdate(){

		// TODO
		// Check if this wave owner still exist, else destroy it

		if(!this.destroyed){

			// Move this wave
			this.tr.position = Vector3.MoveTowards(this.tr.position, this.arrivalStop.transform.position, this.MovementSpeed * Time.fixedDeltaTime);

			// If we arrived
			if(this.tr.position == this.arrivalStop.transform.position){

				this.arrivalStop.OnTakeDamage(this.Value, this.owner);

			}
		}

	}

	public void OnTriggerEnter2D(Collider2D col){

		if(col.tag == "wave" && col.GetComponent<Wave>() != null){

			Wave w = col.GetComponent<Wave>();

			if(!this.collidedList.Contains(w)){
				this.OnCollideWave(w);
				this.collidedList.Add(w);
			}

		}

	}

	public void OnCollideWave(Wave other){

		float otherValue = other.Value;
		other.OnCollidedByWave(this);

		// If those are ennemies wave
		if(this.owner != other.owner){

			this.Value -= otherValue;

			// Destroy this wave
			if(this.Value >= 0f){
				this.OnDestroyWave();
			}

		}
		// Else
		else {
			
		}

	}

	/// <summary>
	/// Received 
	/// </summary>
	/// <param name="other">Other.</param>
	public void OnCollidedByWave(Wave other){
		float otherValue = other.Value;
		this.collidedList.Add(other);

		// If those are ennemies wave
		if(this.owner != other.owner){

			this.Value -= otherValue;

			// Destroy this wave
			if(this.Value >= 0f){
				this.OnDestroyWave();
			}

		}
		// Else
		else {

		}


	}

	public void OnDestroyWave(){
		this.GetComponent<Collider2D>().enabled = false;

		this.Value = 0f;
		this.destroyed = true;

	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {

	#region Properties

	[Header ("Wave")]

	private GamePlayer owner;
	private ControlSpot startSpot;
	private Vector3 targetPos;

	[SerializeField]
	private float MovementSpeed =10f;

	private Transform tr;

	[SerializeField]
	private float Value = 30f;

	private List<Wave> collidedList = new List<Wave>();

	private WaveGenerator Generator;

	private bool destroyed = false;

	[SerializeField]
	private GameObject colliderObject;

	private float distanceTraveled = 0f;

	[SerializeField]
	private float decaySpeed = 15f;

	#endregion

	private void Awake(){
		this.tr = this.transform;

	}

	public void SetSiblings(List<Wave> siblings){

		this.collidedList.AddRange(siblings);

	}

	public void SetDirection(GamePlayer owner, ControlSpot startSpot, Vector3 targetPos){

		this.owner = owner;
		this.startSpot = startSpot;
		this.targetPos = targetPos;

	}


	public void FixedUpdate(){

		// TODO
		// Check if this wave owner still exist, else destroy it

		if(!this.destroyed){

			// Move this wave
			this.tr.position = Vector3.MoveTowards(this.tr.position, this.targetPos, this.MovementSpeed * Time.fixedDeltaTime);

			this.distanceTraveled += this.MovementSpeed * Time.fixedDeltaTime;

			this.colliderObject.transform.localScale = new Vector3(this.distanceTraveled * 0.5f, 1f, 1f);

			// Decay
			this.Value -= this.decaySpeed * Time.fixedDeltaTime;

			// if this wave have decayed
			if(this.Value <= 0f){
				this.OnDestroyWave();
			}

		}

	}

	public void OnTriggerEnter2D(Collider2D col){
			

		if(col.tag == "wave" && col.GetComponentInChildren<Wave>() != null){

			Wave w = col.GetComponentInChildren<Wave>();

			if(!this.collidedList.Contains(w)){
				this.OnCollideWave(w);
				this.collidedList.Add(w);
			}

		}
		else if(col.tag == "spot" && col.GetComponentInChildren<ControlSpot>() != null){

			ControlSpot spot = col.GetComponentInChildren<ControlSpot>();

			spot.OnTakeDamage(this.Value, this.owner);

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
	/// Used by receiving wave
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
		this.GetComponentInChildren<Collider2D>().enabled = false;

		this.Value = 0f;
		this.destroyed = true;

		// 
		GameObject.Destroy(this.gameObject, 0.5f);
	}

}

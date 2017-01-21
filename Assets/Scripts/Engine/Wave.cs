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

	private List<object> collidedList = new List<object>();
	private List<Wave> siblings = new List<Wave>();

	private WaveGenerator Generator;

	private bool destroyed = false;

	[SerializeField]
	private GameObject colliderObject;

	private float distanceTraveled = 0f;

	[SerializeField]
	private float decaySpeed = 15f;

	[SerializeField]
	private SpriteRenderer WaveRenderer;

	[SerializeField]
	private Collider2D collider;

	#endregion

	private void Awake(){
		this.tr = this.transform;

		this.collider.enabled = false;

		this.Invoke("AwakeCollider", 0.15f);

	}

	private void AwakeCollider(){
		this.collider.enabled = true;
	}

	public void SetSiblings(List<Wave> siblings){

		this.siblings.AddRange(siblings);
		foreach(Wave w in siblings){
			this.collidedList.Add(w);
		}
	}

	public void SetDirection(GamePlayer owner, ControlSpot startSpot, Vector3 targetPos, float baseValue){

		this.owner = owner;
		this.startSpot = startSpot;
		this.targetPos = targetPos;
		this.Value = baseValue;

		this.WaveRenderer.color = this.owner.PlayerMainColor;
	}


	public void FixedUpdate(){

		// TODO
		// Check if this wave owner still exist, else destroy it

		if(!this.destroyed){

			// Move this wave
			this.tr.position = Vector3.MoveTowards(this.tr.position, this.targetPos, this.MovementSpeed * Time.fixedDeltaTime);

			this.distanceTraveled += this.MovementSpeed * Time.fixedDeltaTime;

			this.colliderObject.transform.localScale = new Vector3(this.distanceTraveled * 1f, 1f, 1f);

			// Decay
			this.Value -= this.decaySpeed * Time.fixedDeltaTime;

			// if this wave have decayed
			if(this.Value <= 0f){
				this.OnDestroyWave();
			}

		}

	}

	public void OnTriggerEnter2D(Collider2D col){
			

		if(col.tag == "wave" && col.GetComponentInParent<Wave>() != null){

			Wave w = col.GetComponentInParent<Wave>();

			if(!this.collidedList.Contains(w)){
				this.OnCollideWave(w);
				//this.collidedList.Add(w);
			}

		}
		else if(col.tag == "spot" && col.GetComponentInChildren<ControlSpot>() != null ){

			ControlSpot spot = col.GetComponentInChildren<ControlSpot>();

			if(spot != this.startSpot && !this.collidedList.Contains(spot)){
				spot.OnTakeDamage(this.Value, this.owner);

				// Add this spot to the collided list of all siblings
				foreach(Wave w in this.siblings){
					w.AddToCollidedList(spot);
				}

				/*
				// Destroy the wave, except if the spot is owned & already taken
				if(spot.CurrentOwner != this.owner || !spot.IsTaken){
					this.OnDestroyWave();
				}
				*/

			}

			if(spot != this.startSpot){
				// Destroy the wave, except if the spot is owned & already taken
				if(spot.CurrentOwner != this.owner || !spot.IsTaken){
					this.OnDestroyWave();
				}
			}

		}

	}

	public void OnCollideWave(Wave other){

		Debug.Log("Wave.OnCollideWave - SelfValue "+this.Value+", other value : "+other.Value);

		float otherValue = other.Value;
		other.OnCollidedByWave(this);

		foreach(Wave w in this.siblings){
			this.collidedList.Add(other);
		}

		// If those are ennemies wave
		if(this.owner != other.owner){

			this.Value -= otherValue;

			// Destroy this wave
			if(this.Value <= 0f){
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
		//this.collidedList.Add(other);
		foreach(Wave w in this.siblings){
			this.collidedList.Add(other);
		}

		// If those are ennemies wave
		if(this.owner != other.owner){

			this.Value -= otherValue;

			// Destroy this wave
			if(this.Value <= 0f){
				this.OnDestroyWave();
			}

		}
		// Else
		else {

		}

	}

	private void AddToCollidedList(object o){
		this.collidedList.Add(o);
	}

	public void OnDestroyWave(){
		this.collider.enabled = false;

		this.Value = 0f;
		this.destroyed = true;

		// 
		GameObject.Destroy(this.gameObject, 0.5f);
	}

}

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

	[SerializeField]
	private float decaySpeed = 15f;

	[Space(15)]

	private Transform tr;

	private float Value = 30f;

	private List<object> collidedList = new List<object>();
	private List<Wave> siblings = new List<Wave>();

	private WaveGenerator Generator;

	private bool destroyed = false;

	[SerializeField]
	private GameObject colliderObject;

	private float distanceTraveled = 0f;

	[SerializeField]
	private SpriteRenderer WaveRenderer;

	[SerializeField]
	private Collider2D collider;

	private int orientation = 0;

	private bool used = false;
	public bool isUsed {
		get {
			return this.used;
		}
	}

	#endregion

	private void Awake(){
		this.tr = this.transform;

		this.collider.enabled = false;

		//this.Invoke("AwakeCollider", 0.15f);

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

	public void SetOrientation(int orientation){

		this.orientation = orientation;

	}

	public void SetDirection(GamePlayer owner, ControlSpot startSpot, float baseValue){

		// This wave is in use
		this.used = true;

		this.owner = owner;
		this.startSpot = startSpot;
		this.Value = baseValue;

		// Re activate the graphical objects
		this.ShowGraphics();


		// Come into the spot transform
		this.tr.SetParent(this.startSpot.transform);
		this.tr.localPosition = Vector3.zero;

		this.WaveRenderer.color = this.owner.PlayerMainColor;

		// Set the target direction
		float angle = (this.orientation/(float)WavePool.Instance.NumberOfWaveByImpulse)* (Mathf.PI * 2);
		this.targetPos = 10000f*(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0));


		this.Invoke("AwakeCollider", 0.15f);
	}


	public void FixedUpdate(){

		// TODO
		// Check if this wave owner still exist, else destroy it

		if(!this.destroyed && this.isUsed){

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
				//if(spot.CurrentOwner != this.owner || !spot.IsTaken){
					this.OnDestroyWave();
				//}
			}

		}

	}

	public void OnCollideWave(Wave other){

		//Debug.Log("Wave.OnCollideWave - SelfValue "+this.Value+", other value : "+other.Value);

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

		this.Invoke("ReturnToPool", 0.5f);

		// 
		//GameObject.Destroy(this.gameObject, 0.5f);
	}

	#region Pooling

	private void ReturnToPool(){
		this.used = false;

		// Check if all other siblings are done
		int done = 0;
		foreach(Wave w in this.siblings){

			if(!w.isUsed){
				++done;
			}
			else{
				break;
			}

		}

		// If all siblings are done, return to the Object pool
		if(done == this.siblings.Count){
			WavePool.Instance.ReturnWaveImpulse(this.siblings);
		}

		this.HideGraphics();

	}

	private void HideGraphics(){
		this.colliderObject.gameObject.SetActive(false);

	}

	private void ShowGraphics(){
		this.colliderObject.gameObject.SetActive(true);

	}

	public void ResetUsed(){

		this.HideGraphics();

		this.used = false;

		this.owner = null;
		this.collider.enabled = false;
		this.collidedList.Clear();
		this.startSpot = null;
		this.distanceTraveled = 0f;
		this.destroyed = false;

		this.colliderObject.transform.localScale = Vector3.one;

		this.tr.SetParent(WavePool.Instance.Holder);
		this.tr.localPosition = Vector3.zero;



	}


	#endregion

}

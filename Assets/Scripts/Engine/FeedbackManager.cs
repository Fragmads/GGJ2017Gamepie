using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackManager : MonoBehaviour {

	#region Singleton

	private static FeedbackManager instance;
	public static FeedbackManager Instance{
		get {
			return FeedbackManager.instance;
		}
	}

	public void Awake(){

		if(FeedbackManager.instance == null){
			FeedbackManager.instance = this;
			GameObject.DontDestroyOnLoad(this.transform.root);

		}
		else{
			GameObject.Destroy(this);
		}

	}

	#endregion

	#region Properties

	[Header ("FeedbackManager")]

	[Header ("- Charge Jauge")]
	[SerializeField]
	private GameObject jaugeObject;

	[SerializeField]
	private Transform jaugeBar;

	[SerializeField]
	private Vector3 jaugeObjectOffset;

	#endregion


	public void Start(){
		this.jaugeBar.GetComponent<SpriteRenderer>().color = GameManager.Instance.PlayerColor;

		this.HideJauge();

	}

	public void DisplayJaugeAtSpot(ControlSpot spot){

		this.jaugeObject.transform.position = spot.transform.position + this.jaugeObjectOffset;

		this.jaugeObject.gameObject.SetActive(true);

		this.jaugeBar.localScale = new Vector3(1f, 0f, 1f);

		// Play the loading sound
		SoundManager.Instance.StartChargeSound();

	}

	public void UpdateJaugeBar(float holdTime){
		float barLength = 0f;

		if(holdTime < GameManager.Instance.Level1ChargeTime){
			barLength = holdTime/ GameManager.Instance.Level1ChargeTime * (1/3f);
		}
		else if(holdTime < GameManager.Instance.Level2ChargeTime){
			barLength = ((holdTime - GameManager.Instance.Level1ChargeTime)/ (GameManager.Instance.Level2ChargeTime - GameManager.Instance.Level1ChargeTime) * (1/3f) + (1/3f));
		}
		else{
			barLength = Mathf.Clamp01((holdTime -  GameManager.Instance.Level2ChargeTime) / (GameManager.Instance.Level3ChargeTime - GameManager.Instance.Level2ChargeTime) * (1/3f) + (2/3f));
		}

		this.jaugeBar.localScale = new Vector3(1f, barLength, 1f);

	}

	public void HideJauge(){
		this.jaugeObject.gameObject.SetActive(false);
		SoundManager.Instance.StopChargeSound();
	}

}

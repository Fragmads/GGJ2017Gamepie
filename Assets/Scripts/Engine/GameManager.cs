﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	#region Singleton

	public static GameManager Instance{
		get {
			return GameManager.instance;
		}

	}
	private static GameManager instance;

	private void Awake(){

		if(GameManager.instance == null){

			GameManager.instance = this;
			GameObject.DontDestroyOnLoad(this.transform.root);
		}
		else{
			GameObject.Destroy(this.gameObject);
		}

	}

	#endregion

	#region Properties

	[Header ("GameManager")]

	public GameLevel CurrentLevel;

	[SerializeField]
	public Color PlayerColor;

	[Header("Game Rules")]
	[Tooltip ("Point at which you conquer a neutral control spot")]
	public float NeutralSpotConquerPoint = 100f;

	[Header ("Game settings")]
	public Color NeutralMainColor;

	[Space (10)]
	public float Level1ChargeTime = 0.75f;
	public float Level2ChargeTime = 1.5f;
	public float Level3ChargeTime = 2.25f;

	public float Level1Value = 30f;
	public float Level2Value = 50f;
	public float Level3Value = 70f;

	[HideInInspector]
	public bool ShowLevelSelection = false;

	#endregion

	[Header ("Debug")]
	[SerializeField]
	private ControlSpot forcedSpot;

	public void Update(){

		// Debug only
		#if UNITY_EDITOR

		if (Input.GetKeyDown(KeyCode.E) && this.forcedSpot != null){

			this.forcedSpot.ReleaseWave(this.Level1ChargeTime);

		}


		#endif
	}

	public void Start(){

		SceneManager.sceneLoaded += this.OnSceneLoaded;

	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {

		// Get next scene
		this.CurrentLevel = GameObject.FindObjectOfType<GameLevel>();

	}


	public void GameOver(){

		Debug.Log("GameManager.GameOver");

	}

	public void Win(){
		Debug.Log("GameManager.Win");

	}

}

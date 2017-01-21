using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	[Header("Game Rules")]
	[Tooltip ("Point at which you conquer a neutral control spot")]
	public float NeutralSpotConquerPoint = 100f;

	#endregion


}

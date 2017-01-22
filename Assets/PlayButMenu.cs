using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnMouseDown ()
	{
		if (Input.GetKey ("mouse 0")) {
			SceneManager.LoadScene ("level1");
			
		}
	}
}

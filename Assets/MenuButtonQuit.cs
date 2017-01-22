using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonQuit : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnMouseDown ()
	{
		if (Input.GetKey ("mouse 0")) {
			Application.Quit();

		}
	}
}

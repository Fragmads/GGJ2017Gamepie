using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creditbuttonmenu : MonoBehaviour {
	public GameObject CreditsTarget;
	public GameObject PlayButtonTarget;
	public GameObject QuitButtonTarget;


	public PolygonCollider2D PlayColl;
	public PolygonCollider2D QuitColl;
	public PolygonCollider2D CreditsColl;
	// Use this for initialization
	void Start () {
		PlayColl = PlayButtonTarget.GetComponent<PolygonCollider2D> ();
		QuitColl = QuitButtonTarget.GetComponent<PolygonCollider2D> ();
		CreditsColl = GetComponent<PolygonCollider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnMouseDown ()
	{
		if (Input.GetKey ("mouse 0")) {
			CreditsTarget.gameObject.SetActive(true);
			SwitchColl ();

		}
	}
	public void SwitchColl ()
	{
		PlayColl.enabled = !PlayColl.enabled;
		CreditsColl.enabled = !CreditsColl.enabled;
		QuitColl.enabled = !QuitColl.enabled;
	}
}

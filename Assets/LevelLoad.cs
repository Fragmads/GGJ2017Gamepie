using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoad : MonoBehaviour {
	public Creditbuttonmenu  CreditButtonMenScript;
	public string Lvlname;

	void Update()
	{
		//If the left mouse button is clicked.
		if (Input.GetMouseButtonDown(0))
		{
			//Get the mouse position on the screen and send a raycast into the game world from that position.
			Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(worldPoint,Vector2.zero);

			//If something was hit, the RaycastHit2D.collider will not be null.
			if ( hit.collider != null )
			{
				Lvlname= hit.collider.name;
				LevelTest ();
			}
		}
	}
	public void LevelTest()
	{
		switch (Lvlname) {
		case "Lvl10":
			SceneManager.LoadScene ("level8");
			break;
		case "Lvl9":
			SceneManager.LoadScene ("level7");
			break;
		case "Lvl8":
			SceneManager.LoadScene ("level10");
			break;
		case "Lvl7":
			SceneManager.LoadScene ("level9");
			break;
		case "Lvl6":
			SceneManager.LoadScene ("level01-alex");
			break;
		case "Lvl5":
			SceneManager.LoadScene ("level5");
			break;
		case "Lvl4":
			SceneManager.LoadScene ("level4");
			break;
		case "Lvl3":
			SceneManager.LoadScene ("level3");
			break;
		case "Lvl2":
			SceneManager.LoadScene ("level2");
			break;
		case "Lvl1":
			SceneManager.LoadScene ("level1");
			break;
		case "Home Button":
			CreditButtonMenScript.SwitchColl();
			this.gameObject.SetActive(false);
			break;

		}
}
		
}

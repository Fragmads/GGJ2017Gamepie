// Author : THIERRY Renaud
// Created : 27-10-2015
// 

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class UIScreen : MonoBehaviour {

	// Properties
	//

	[Header ("UIScreen")]
	protected Canvas canvas;
	protected GraphicRaycaster raycaster;

	public string ScreenID = "";

	public bool isOpened = false;

	public enum BackButtonAction	{NONE, TITLE, DIALOG_QUIT_GAME, QUIT_APPLICATION};
	public UIScreen.BackButtonAction ScreenBackAction = BackButtonAction.TITLE;

	// Methods
	//


	// Use this for initialization
	public virtual void Awake () {
		

		if (this.GetComponent<Canvas> () != null) {

			this.canvas = this.GetComponent<Canvas> ();
			
		} else
		// Auto init a canvas
		{

			this.canvas = this.gameObject.AddComponent<Canvas>();
			this.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		}

		// Init the GraphicRaycaster
		if (this.GetComponent<GraphicRaycaster> () != null) {
			this.raycaster = this.GetComponent<GraphicRaycaster> ();
		} else {	
			this.raycaster = this.gameObject.AddComponent<GraphicRaycaster>();
		}

		// If this screen don't have a name ID, set it by it's game object name
		if (this.ScreenID == "") {

			// Case this is an instantiated prefab, we don't want the (Clone) part of the title
			if(this.name.EndsWith("(Clone)")){
				this.ScreenID = this.name.Substring(0, this.name.LastIndexOf("(Clone)"));
			}
			else{
				this.ScreenID = this.name;
			}

		}

	}


	/// <summary>
	/// Action of the back button
	/// </summary>
	public virtual void BackAction(){

		// Execute the wanted action on back button pressed
		switch(this.ScreenBackAction){
		
		case BackButtonAction.TITLE:
            if (UIScreenManager.Instance != null)
            {
                this.OpenOtherScreen(UIScreenManager.Instance.TitleScreenID);
            }
			break;
		case BackButtonAction.DIALOG_QUIT_GAME:
			// Reuse the option quit confirm dialog
			break;
		case BackButtonAction.QUIT_APPLICATION:
			Application.Quit();
			break;

		}

	}

	/// <summary>
	/// Opens the screen.
	/// </summary>
	public virtual void OpenScreen(){

		this.canvas.enabled = true;
		this.raycaster.enabled = true;

	}

	/// <summary>
	/// Closes the screen.
	/// </summary>
	public virtual void CloseScreen(){

		//Debug.Log ("UIScreen.CloseScreen - "+this.name);
		this.canvas.enabled = false;
		this.raycaster.enabled = false;

	}

	/// <summary>
	/// Opens the other screen, closing this one.
	/// </summary>
	/// <param name="nextScreen">Next screen to open. use "Last" for the last different screen opened</param>
	public virtual void OpenOtherScreen(string nextScreen){

		// If we should re open the last popup opened
		if (nextScreen == "Last") {

			// If there is a last Screen to open
			if(UIScreenManager.Instance.LastUIScreen != null){
				UIScreenManager.Instance.OpenScreen (UIScreenManager.Instance.LastUIScreen);
			}
			// Use the Menu as default backup
			else{
				UIScreen UINextScreen = UIScreenManager.Instance.GetUIScreenByID ("Menu");
				
				// If the UIScreen was found
				if (UINextScreen != null) {
					UIScreenManager.Instance.OpenScreen (UINextScreen);
				}
			}
			
		} else {
			UIScreen UINextScreen = UIScreenManager.Instance.GetUIScreenByID (nextScreen);

			// If the UIScreen was found
			if (UINextScreen != null) {
				UIScreenManager.Instance.OpenScreen (UINextScreen);
			}
		}

	}

	/// <summary>
	/// Opens an other pop up.
	/// </summary>
	/// <param name="nextPopup">Next popup to open. Use "Last" for the last different popup opened</param>
	public virtual void OpenOtherPopUp(string nextPopup){

		// If we should re open the last popup opened
		if (nextPopup == "Last") {

			// if there was a last popup opened
			if(UIScreenManager.Instance.LastUIPopup){
				UIScreenManager.Instance.OpenPopUp (UIScreenManager.Instance.LastUIPopup);
			}

		} else {
			UIPopUp UINextPopup = UIScreenManager.Instance.GetUIPopupByID (nextPopup);
		
			// If the UIScreen was found
			if (UINextPopup != null) {
				UIScreenManager.Instance.OpenPopUp (UINextPopup);
			}
		}
		
	}

    public void PlayClick()
    {

        // PLay the click menu sound
        if (SoundManager.Instance != null && SoundManager.Instance.SoundOn)
        {
            SoundManager.Instance.PlayClickMenu();
        }

    }

}

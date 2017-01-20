using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class UIPopUp : MonoBehaviour {

	// Properties
	//

	[Header ("UIPopUp")]
	protected GraphicRaycaster raycaster;

	public bool UseCache = false;
	public bool PlaySoundOnOpenClose = true;

	public string PopupID = "";

	public enum UIPopUpBackAction	{NONE, CLOSE_POPUP, OPEN_OTHER_POPUP, OPEN_OTHER_SCREEN};
	public UIPopUpBackAction BackButtonAction = UIPopUpBackAction.CLOSE_POPUP;
	public string BackActionParam = "Only for open other, Popup or screen ID";
    
	// Methods
	//

	// Use this for initialization
	public virtual void Awake () {

		// Init the GraphicRaycaster
		if (this.GetComponent<GraphicRaycaster> () != null) {
			this.raycaster = this.GetComponent<GraphicRaycaster> ();
		} else {	
			this.raycaster = this.gameObject.AddComponent<GraphicRaycaster>();
		}

		// If this popup don't have a name ID, set it by it's game object name
		if (this.PopupID == "") {
			
			// Case this is an instantiated prefab, we don't want the (Clone) part of the title
			if(this.name.EndsWith("(Clone)")){
				this.PopupID = this.name.Substring(0, this.name.LastIndexOf("(Clone)"));
			}
			else{
				this.PopupID = this.name;
			}			
		}

	}
	

	/// <summary>
	/// Do the back action for this popup
	/// </summary>
	public virtual void BackAction(){

		// Depending of the back action selected
		switch (this.BackButtonAction) {

		case UIPopUpBackAction.CLOSE_POPUP:
			this.ClosePopUp();
			break;
		case UIPopUpBackAction.OPEN_OTHER_POPUP:
			this.OpenOtherPopUp(this.BackActionParam);
			this.ClosePopUp();
			break;
		case UIPopUpBackAction.OPEN_OTHER_SCREEN:
			this.OpenOtherScreen(this.BackActionParam);
			this.ClosePopUp();
			break;

		}

	}


	/// <summary>
	/// Opens the pop up (Scale = 1,1,1).
	/// </summary>
	public virtual void OpenPopUp(){

		// Not in edit mode
		if (Application.isPlaying) {
			this.gameObject.SetActive (true);
		}

		// Enable all canvas inside this popup
		foreach (Canvas c in this.GetComponentsInChildren<Canvas>()) {
			c.enabled = true;
		}

		// Enable all raycaster inside this popup
		foreach (GraphicRaycaster gr in this.GetComponentsInChildren<GraphicRaycaster>()) {
			gr.enabled = true;
		}

		this.transform.localScale = Vector3.one;
		this.raycaster.enabled = true;

        // Play the open popup sound
        if (SoundManager.Instance != null && SoundManager.Instance.SoundOn && this.PlaySoundOnOpenClose) {
            SoundManager.Instance.PlayPopupOpen();
        }

        
	}

	/// <summary>
	/// Closes the pop up (Scale = 0,0,0).
	/// </summary>
	public virtual void ClosePopUp(){

		this.transform.localScale = Vector3.zero;
		this.raycaster.enabled = false;

		// If we where the current popup, hide the Cache
		if (UIScreenManager.Instance != null && UIScreenManager.Instance.CurrentPopUp == this) {
			UIScreenManager.Instance.HidePopUp();
			UIScreenManager.Instance.CurrentPopUp = null;

            // Play the close popup sound
            if (SoundManager.Instance != null && SoundManager.Instance.SoundOn && this.PlaySoundOnOpenClose)
            {
                SoundManager.Instance.PlayPopupClose();
            }

        }

		// Not in edit mode
		if (Application.isPlaying) {
			// Deactivate this game object
			this.gameObject.SetActive (false);
		}

		// Disable all canvas inside this popup
		foreach (Canvas c in this.GetComponentsInChildren<Canvas>()) {
			c.enabled = false;
		}

		// Disable all raycaster inside this popup
		foreach (GraphicRaycaster gr in this.GetComponentsInChildren<GraphicRaycaster>()) {
			gr.enabled = false;
		}

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

    public void PlayClick() {

        // PLay the click menu sound
        if (SoundManager.Instance != null && SoundManager.Instance.SoundOn) {
            SoundManager.Instance.PlayClickMenu();
        }

    }


}

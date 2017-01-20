using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public enum DialogCloseAction
{
	BUTTON_OK,
	BUTTON_CANCEL,
	CLICK_OUTSIDE,
	ALL_CLOSED
}

[ExecuteInEditMode]
public class UIDialog : MonoBehaviour {

	// Properties
	//


	[Header ("UIDialog")]
	public bool UseCache = true;
	public bool CloseOnOutsideClick = true;

	public string DialogID = "";

	public delegate void CloseDialogMethod(DialogCloseAction closeAction);

	protected CloseDialogMethod closeDialogMethod = null;

	protected GraphicRaycaster raycaster;


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
		if (this.DialogID == "") {
			
			// Case this is an instantiated prefab, we don't want the (Clone) part of the title
			if(this.name.EndsWith("(Clone)")){
				this.DialogID = this.name.Substring(0, this.name.LastIndexOf("(Clone)"));
			}
			else{
				this.DialogID = this.name;
			}			
		}
		
	}


	/// <summary>
	/// Opens the dialog.
	/// </summary>
	/// <param name="closeMethod">Close method used by this dialog.</param>
	public virtual void OpenDialog(CloseDialogMethod closeMethod){

		// Set the closing method
		this.closeDialogMethod = closeMethod;

		this.transform.localScale = Vector3.one;
		this.raycaster.enabled = true;

	}


	/// <summary>
	/// Closes the dialog.
	/// </summary>
	/// <param name="closeAction">Close action used to close this Dialog.</param>
	public void CloseDialog(DialogCloseAction closeAction){

		// If  a special close method was used
		if (this.closeDialogMethod != null) {

			CloseDialogMethod tmp = this.closeDialogMethod;
			this.closeDialogMethod = null;
			tmp(closeAction);

		}

		this.transform.localScale = Vector3.zero;
		this.raycaster.enabled = false;
		
		// If we where the current popup, hide the Cache
		if (UIScreenManager.Instance != null && UIScreenManager.Instance.CurrentDialog == this) {
			UIScreenManager.Instance.HideDialog();
			UIScreenManager.Instance.CurrentDialog = null;
		}

	}

	/// <summary>
	/// Closes the dialog with the default close value.
	/// </summary>
	public void CloseDialog(){

		this.CloseDialog (DialogCloseAction.BUTTON_OK);

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

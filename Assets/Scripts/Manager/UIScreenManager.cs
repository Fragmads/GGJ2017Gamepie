using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class UIScreenManager : MonoBehaviour {

	// Properties
	//
    
	// Singleton
	private static UIScreenManager instance;
	public static UIScreenManager Instance{
		get{return UIScreenManager.instance;}
	}
    
	public void Awake(){

		if(Application.isPlaying){

			// Singleton logic
			if(UIScreenManager.instance == null){
				UIScreenManager.instance = this;
				GameObject.DontDestroyOnLoad(this.gameObject);
			}
			else{
				GameObject.Destroy(this.gameObject);
				return;
			}
		}

	}

	[Header("Prefabs List")]
	public List<UIScreen> ScreensPrefab;
	public List<UIPopUp> PopupsPrefab;
	public List<UIDialog> DialogsPrefab;
	public RectTransform PopupCachePrefab;
	public RectTransform DialogCachePrefab;
	public RectTransform InSceneCanvasPrefab;

	[Header("Active Elements")]
	public List<UIScreen> Screens = new List<UIScreen>();
	public List<UIPopUp> Popups = new List<UIPopUp>();
	public List<UIDialog> Dialogs = new List<UIDialog> ();

	public Canvas PopupCanvas;
	public RectTransform PopupCache;

	// Dialog
	public Canvas DialogCanvas;
	public RectTransform DialogCache;

	public UIScreen CurrentScreen;
	public UIScreen LastUIScreen;
	public UIPopUp CurrentPopUp;
	public UIPopUp LastUIPopup;
	public UIDialog CurrentDialog;
	public UIDialog LastUIDialog;

	public string StartingUIScreenID = "SplashTitle";
    [Tooltip("Id of the Title screeen, used in the Back button action TITLE")]
    public string TitleScreenID = "Title";

	[Header("Editor")]
	public bool EditorAutoScreenChange = true;


	protected GameObject UIRoot;
	protected GameObject ScreenRoot;

	// Specific Screen getter
    

    private Canvas inSceneCanvas;
	public Canvas InSceneCanvas{
		get{return this.inSceneCanvas;}
	}

	// Methods
	//

	// Use this for initialization
	void Start () {

        //GameObject.DontDestroyOnLoad(this.gameObject);

		/*
        // If there is an instance, and it's not this game object
        if (UIScreenManager.Instance != null && UIScreenManager.Instance != this) {
            GameObject.DestroyImmediate(this.gameObject);
            return;
        }
		*/
        

		// Init the manager
		this.Screens.Clear ();
		this.Popups.Clear ();
		this.Dialogs.Clear ();
        
		// Destroy old holder that tend to stay in the scene
		GameObject.DestroyImmediate(GameObject.Find("UI"));

		// Create a Holder
		if (this.UIRoot == null) {

			// Init a UI root object
			this.UIRoot = new GameObject ("UI");
			this.UIRoot.transform.SetParent (this.transform.parent);
			if(Application.isPlaying){
				GameObject.DontDestroyOnLoad (this.UIRoot.transform.root);
			}

			// Init the inscene canvas
			this.inSceneCanvas = GameObject.Instantiate(this.InSceneCanvasPrefab).GetComponent<Canvas>();
			this.inSceneCanvas.transform.SetParent(this.UIRoot.transform);

			// Init a Screen object
			this.ScreenRoot = new GameObject("Screens");
			this.ScreenRoot.transform.SetParent(this.UIRoot.transform);

			// Init the Popup canvas
			this.PopupCanvas = new GameObject ("Popups").AddComponent<Canvas> ();
            this.PopupCanvas.gameObject.AddComponent<CanvasRenderer>();
            this.PopupCanvas.gameObject.AddComponent<CanvasScaler>();
            this.PopupCanvas.gameObject.AddComponent<Text>().text = " ";

			this.PopupCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			this.PopupCanvas.transform.SetParent (this.UIRoot.transform);
			this.PopupCanvas.sortingOrder = 10;
			this.PopupCanvas.pixelPerfect = false;

			// Init the popup cache
			this.PopupCache = GameObject.Instantiate<RectTransform>(this.PopupCachePrefab);
			this.PopupCache.SetParent(this.PopupCanvas.transform);
			this.PopupCache.offsetMax = Vector2.zero;
			this.PopupCache.offsetMin = Vector2.zero;
			this.PopupCache.SetAsFirstSibling();

			// Init the dialog canvas
			this.DialogCanvas = new GameObject ("Dialogs").AddComponent<Canvas>();
            this.DialogCanvas.gameObject.AddComponent<CanvasRenderer>();
            this.DialogCanvas.gameObject.AddComponent<CanvasScaler>();
            this.DialogCanvas.gameObject.AddComponent<Text>().text = " ";
            this.DialogCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			this.DialogCanvas.transform.SetParent (this.UIRoot.transform);
			this.DialogCanvas.sortingOrder = 20;

			// Init the dialog cache
			this.DialogCache = GameObject.Instantiate<RectTransform>(this.DialogCachePrefab);
			this.DialogCache.SetParent(this.DialogCanvas.transform);
			this.DialogCache.offsetMax = Vector2.zero;
			this.DialogCache.offsetMin = Vector2.zero;
			this.DialogCache.SetAsFirstSibling();

		}


		// Instantiate the Screen prefabs
		foreach (UIScreen uis in this.ScreensPrefab) {

#if UNITY_EDITOR
			UIScreen screen = ((GameObject)PrefabUtility.InstantiatePrefab(uis.gameObject)).GetComponent<UIScreen>();
#else
			UIScreen screen = GameObject.Instantiate<UIScreen>(uis);
#endif
			screen.transform.SetParent(this.ScreenRoot.transform);
			this.Screens.Add(screen);

		}

		// Instantiate the popup prefabs
		foreach (UIPopUp uip in this.PopupsPrefab) {
			
#if UNITY_EDITOR
			UIPopUp popup = ((GameObject)PrefabUtility.InstantiatePrefab(uip.gameObject)).GetComponent<UIPopUp>();
#else
			UIPopUp popup = GameObject.Instantiate<UIPopUp>(uip);
#endif
			popup.transform.SetParent(this.PopupCanvas.transform, true);
			this.Popups.Add(popup);

			// Don't let Unity put false value in there
			RectTransform rt = popup.GetComponent<RectTransform>();
			rt.offsetMax = Vector2.zero;
			rt.offsetMin = Vector2.zero;

		}

		// Instantiate the dialog prefabs
		foreach (UIDialog uid in this.DialogsPrefab) {
			
#if UNITY_EDITOR
			UIDialog dialog = ((GameObject)PrefabUtility.InstantiatePrefab(uid.gameObject)).GetComponent<UIDialog>();
#else
			UIDialog dialog = GameObject.Instantiate<UIDialog>(uid);
#endif
			dialog.transform.SetParent(this.DialogCanvas.transform, true);
			this.Dialogs.Add(dialog);
			
			// Don't let Unity put false value in there
			RectTransform rt = dialog.GetComponent<RectTransform>();
			rt.offsetMax = Vector2.zero;
			rt.offsetMin = Vector2.zero;
			
		}

		// Close everything
		this.CloseAllDialog ();
		this.CloseAllPopUp ();
		this.CloseAllScreen ();
		this.HidePopUp ();
		this.HideDialog ();

		// Not in edit mode
		if (Application.isPlaying) {

            UIScreenManager.instance = this;

			this.OpenScreen(this.GetUIScreenByID(this.StartingUIScreenID));
		}

	}

	private float backButtonDelay = 0f;
	public void Update(){

		this.backButtonDelay -= Time.deltaTime;

		// If we are playing and the escape key is pressed
		if (Application.isPlaying && Input.GetKeyDown(KeyCode.Escape) && this.backButtonDelay < 0f) {

			// If there is a dialog shown
			if(this.CurrentDialog != null){

				this.CurrentDialog.CloseDialog(DialogCloseAction.CLICK_OUTSIDE);

			}
			// If there is a popup Shown
			else if(this.CurrentPopUp != null){

				this.CurrentPopUp.BackAction();

			}
			// If there is a Screen shown
			else if(this.CurrentScreen != null){

				this.CurrentScreen.BackAction();

			}

			this.backButtonDelay = 0.5f;

		}

	}

	/// <summary>
	/// Closes all screen.
	/// </summary>
	public void CloseAllScreen(){
			
		// For each screen
		foreach (UIScreen uis in this.Screens) {
			// if it's not the currently selected Screen, Close it
			if(uis != this.CurrentScreen){
				uis.CloseScreen();
			}

		}

	}

	/// <summary>
	/// Closes all the pop up.
	/// </summary>
	public void CloseAllPopUp(){

		// For each popup
		foreach (UIPopUp uip in this.Popups) {
			// if it's not the currently selected Popup, Close it
			if(uip != this.CurrentPopUp){
				uip.ClosePopUp();
			}

		}

	}

	/// <summary>
	/// Closes all the dialogs.
	/// </summary>
	public void CloseAllDialog(){
		
		// For each popup
		foreach (UIDialog uid in this.Dialogs) {
			// if it's not the currently selected Popup, Close it
			if(uid != this.CurrentDialog){
				uid.CloseDialog(DialogCloseAction.ALL_CLOSED);
			}
			
		}
		
	}


	/// <summary>
	/// Open the screen.
	/// </summary>
	/// <param name="uis">UIScreen to open.</param>
	public void OpenScreen(UIScreen uis){

		// Setup the last screen var
		if (uis != this.CurrentScreen) {
			this.LastUIScreen = this.CurrentScreen;
		}

		//Debug.Log ("UIScreenManager.OpenScreen - "+uis.name);
		uis.OpenScreen ();
		this.CurrentScreen = uis;

		this.CloseAllScreen ();

	}

    /// <summary>
	/// Open the screen.
	/// </summary>
	/// <param name="screenID">ID of the UIScreen to open.</param>
	public void OpenScreen(string screenID)
    {

        UIScreen uis = this.GetUIScreenByID(screenID);

        if (uis != null) {
            this.OpenScreen(uis);
        }

    }

    /// <summary>
    /// Opens the pop up.
    /// </summary>
    /// <param name="uip">Popup to open.</param>
    public void OpenPopUp(UIPopUp uip){

		// Setup the last screen var
		if (uip != this.CurrentPopUp) {
			this.LastUIPopup = this.CurrentPopUp;
		}

		//Debug.Log ("UIScreenManager.OpenScreen - "+uis.name);
		this.CurrentPopUp = uip;	
		
		this.CloseAllPopUp ();

		this.ShowPopUp ();
        uip.OpenPopUp();
    }

    /// <summary>
    /// Opens the pop up.
    /// </summary>
    /// <param name="popupId">Id of the Popup to open.</param>
    public void OpenPopUp(string popupId)
    {

        UIPopUp uip = this.GetUIPopupByID(popupId);

        if (uip != null) {
            this.OpenPopUp(uip);
        }

    }

    /// <summary>
    /// Opens the Dialog.
    /// </summary>
    /// <param name="uip">Dialog to open.</param>
    /// <param name="closeMethod">Close Method used by this dialog </param>
    public void OpenDialog(UIDialog uid, UIDialog.CloseDialogMethod closeMethod = null){
		
		// Setup the last screen var
		if (uid != this.CurrentDialog) {
			this.LastUIDialog = this.CurrentDialog;
		}
		
		//Debug.Log ("UIScreenManager.OpenScreen - "+uis.name);
		
		this.CurrentDialog = uid;
		
		this.CloseAllDialog ();
		
		this.ShowDialog ();
        uid.OpenDialog(closeMethod);
    }

    /// <summary>
    /// Open the Dialog
    /// </summary>
    /// <param name="dialogID">ID of the Dialog to open</param>
    /// <param name="closeMethod">Close Method used by this Dialog</param>
    public void OpenDialog(string dialogID, UIDialog.CloseDialogMethod closeMethod = null) {        

        UIDialog uid = this.GetUIDialogByID(dialogID);

        if (uid != null) {
            this.OpenDialog(uid, closeMethod);
        }

    }

	/// <summary>
	/// Shows the pop up canvas, if an active popup is found.
	/// </summary>
	public void ShowPopUp(){

		// If there is a popup to show
		if (this.CurrentPopUp != null) {
			this.PopupCanvas.enabled = true;

			if(this.CurrentPopUp.UseCache){
				this.PopupCache.localScale = Vector3.one;
			}
			else{
				this.PopupCache.localScale = Vector3.zero;
			}

		} else {
			this.HidePopUp();
		}

	}

	/// <summary>
	/// Hides the pop up canvas.
	/// </summary>
	public void HidePopUp(){
		// If the popup canvas is set
		if(this.PopupCanvas != null && this.PopupCache != null){
			this.PopupCanvas.enabled = false;
			this.PopupCache.localScale = Vector3.zero;
		}
	}

	/// <summary>
	/// Shows the dialog canvas, if an active dialog is found.
	/// </summary>
	public void ShowDialog(){
		
		// If there is a popup to show
		if (this.CurrentDialog != null) {
			this.DialogCanvas.enabled = true;
			
			if(this.CurrentDialog.UseCache){
				this.DialogCache.localScale = Vector3.one;
			}
			else{
				this.DialogCache.localScale = Vector3.zero;
			}
			
		} else {
			this.HideDialog();
		}
		
	}

	/// <summary>
	/// Hides the dialog canvas.
	/// </summary>
	public void HideDialog(){
		// If the popup canvas is set
		if(this.DialogCanvas != null && this.DialogCache != null){
			this.DialogCanvas.enabled = false;
			this.DialogCache.localScale = Vector3.zero;
		}
	}

	/// <summary>
	/// Gets the user interface screen using it's ID.
	/// </summary>
	/// <returns>UIScreen with this ID.</returns>
	/// <param name="UIScreenID">Id (string, name) of the wanted UIScreen.</param>
	public UIScreen GetUIScreenByID(string UIScreenID){

		// For each Screens
		foreach(UIScreen uis in this.Screens){
			// If it's the right one
			if(uis.ScreenID == UIScreenID){
				return uis;
			}

		}

		// Default return
		Debug.LogWarning ("UIScreenManager.GetUIScreenByID - UIScreen not found : "+UIScreenID);
		return null;
	}

	/// <summary>
	/// Gets the user interface popup using it's ID.
	/// </summary>
	/// <returns>UIPopup by ID.</returns>
	/// <param name="UIPopupID">User interface popup ID (string, name) of the wanted UIPopup.</param>
	public UIPopUp GetUIPopupByID(string UIPopupID){
		
		// For each Screens
		foreach(UIPopUp uip in this.Popups){
			// If it's the right one
			if(uip.PopupID == UIPopupID){
				return uip;
			}
			
		}
		
		// Default return
		Debug.LogWarning ("UIScreenManager.GetUIPopupByID - UIPopup not found : "+UIPopupID);
		return null;
	}

	/// <summary>
	/// Gets the user interface dialog using it's ID.
	/// </summary>
	/// <returns>UIDialog by ID.</returns>
	/// <param name="UIDialogID">User interface Dialog ID (string, name) of the wanted UIDialog.</param>
	public UIDialog GetUIDialogByID(string UIDialogID){
		
		// For each Screens
		foreach(UIDialog uid in this.Dialogs){
			// If it's the right one
			if(uid.DialogID == UIDialogID){
				return uid;
			}
			
		}
		
		// Default return
		Debug.LogWarning ("UIScreenManager.GetUIDialogByID - UIDialog not found : "+UIDialogID);
		return null;
	}

	public void OnDestroy(){

		//Debug.Log ("UIScreenManager.OnDestroy");
		if (!Application.isPlaying) {
			//Debug.Log ("UIScreenManager.OnDestroy - Not playing + "+UIScreenManager.UIRoot.name);
			// Clean the created prefabs
			GameObject.DestroyImmediate(GameObject.Find("UI"));
		} else {
			GameObject.Destroy(this.UIRoot);
		}

	}



#if UNITY_EDITOR

	public void OnDrawGizmos(){
		
		if(Selection.activeGameObject != null && !Application.isPlaying && this.EditorAutoScreenChange){
            
			// If the UIScreen selected is part of this UIScreenManager
			if(Selection.activeGameObject.GetComponentInParent<UIScreen>() != null && this.Screens.Contains(Selection.activeGameObject.GetComponentInParent<UIScreen>())){
				
				this.OpenScreen(Selection.activeGameObject.GetComponentInParent<UIScreen>());

				// Close popups
				this.CurrentPopUp = null;
				this.CloseAllPopUp();
				this.HidePopUp();

			}
			// If the UIPopupSelected is part of this UIScreenManager
			else if(Selection.activeGameObject.GetComponentInParent<UIPopUp>() != null && this.Popups.Contains(Selection.activeGameObject.GetComponentInParent<UIPopUp>())){
                //this.PopupCanvas.enabled = true;
				this.OpenPopUp(Selection.activeGameObject.GetComponentInParent<UIPopUp>());
                

            }
			// If the UIDialogSelected is part of this UIScreenManager
			else if(Selection.activeGameObject.GetComponentInParent<UIDialog>() != null && this.Dialogs.Contains(Selection.activeGameObject.GetComponentInParent<UIDialog>())){
                //this.DialogCanvas.enabled = true;
				this.OpenDialog(Selection.activeGameObject.GetComponentInParent<UIDialog>(), null);
                
            }
			else{
				this.CurrentScreen = null;
				this.CloseAllScreen();
				this.CurrentPopUp = null;
				this.CloseAllPopUp();
				this.HidePopUp();
				this.CurrentDialog = null;
				this.CloseAllDialog();
				this.HideDialog();
			}

		}

	}

#endif

}


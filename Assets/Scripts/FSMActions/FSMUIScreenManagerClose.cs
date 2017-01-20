using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions {
	
	[HutongGames.PlayMaker.Tooltip ("Close a screen, a popup, or a dialog using custom UI Manager")]
	[ActionCategory("CustomUIManager")]
	public class FSMUIScreenManagerClose : FSMUIScreenManagerAction { 

		protected override void DoScreenAction (UIScreen screen)
		{
			screen.CloseScreen();
		}

		protected override void DoPopupAction (UIPopUp popup)
		{
			popup.ClosePopUp();
		}

		protected override void DoDialogAction (UIDialog dialog)
		{
			dialog.CloseDialog();
		}

	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions {


	[ActionCategory("CustomUIManager")]
	[HutongGames.PlayMaker.Tooltip ("Open a screen, a popup, or a dialog using custom UI Manager")]
	public class FSMUIScreenManagerOpen : FSMUIScreenManagerAction { 

		protected override void DoScreenAction (UIScreen screen)
		{
			screen.OpenScreen();
		}

		protected override void DoPopupAction (UIPopUp popup)
		{
			popup.OpenPopUp();
		}

		protected override void DoDialogAction (UIDialog dialog)
		{
			dialog.OpenDialog(null);
		}

	}

}
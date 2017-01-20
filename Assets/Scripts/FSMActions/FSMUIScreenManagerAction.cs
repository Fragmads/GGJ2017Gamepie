using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	//[ActionCategory(ActionCategory.GUI)]
	[HutongGames.PlayMaker.Tooltip ("Open a screen, a popup, or a dialog using custom UI Manager")]
	public abstract class FSMUIScreenManagerAction : FsmStateAction {

		#region Properties

		[UIHint (UIHint.FsmString)]
		public FsmString TargetId;

		#endregion

		#region FSMAction

		public override void Reset(){
			this.TargetId = null;
		}

		public override void OnEnter ()
		{

			if(UIScreenManager.Instance != null && this.TargetId != null){

				UIScreen screen = UIScreenManager.Instance.GetUIScreenByID(this.TargetId.Value);

				if(screen != null){
					this.DoScreenAction(screen);
				}
				else {

					UIPopUp popup = UIScreenManager.Instance.GetUIPopupByID(this.TargetId.Value);

					if(popup != null){
						this.DoPopupAction(popup);
					}
					else {
						UIDialog dialog = UIScreenManager.Instance.GetUIDialogByID(this.TargetId.Value);

						if(dialog != null){
							this.DoDialogAction(dialog);
						}
						else{
							Debug.LogWarning("FSMUIScreenManagerAction.OnEnter - No screen/popup/dialog found for : "+this.TargetId.Value);
						}
					}

				}

			}

			Finish();
		}

		#endregion

		#region Abstract Methods

		protected abstract void DoScreenAction(UIScreen screen);
		protected abstract void DoPopupAction(UIPopUp popup);
		protected abstract void DoDialogAction(UIDialog dialog);

		#endregion

	}


}

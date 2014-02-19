using UnityEngine;
using System.Collections;

public class CloseButton : SimpleButton {
			
	public override void released() {
		Debug.Log ("Closed");
		base.released();
		InfoWindowManager.Instance.toggleScreen(InfoWindowManager.Instance.currentScreen);
	}
}
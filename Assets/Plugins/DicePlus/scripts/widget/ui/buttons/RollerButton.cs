using UnityEngine;

public class RollerButton : SimpleButton {
	
	public override void released() {
		base.released();
		InfoWindowManager.Instance.disableInfoWindow();
		DicePlusConnectionManager.Instance.enableRoller();
	}
}

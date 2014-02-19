using UnityEngine;

public class ScreenButton : SimpleButton {
	
	[HideInInspector]
	public InfoWindowManager.ScreenType type;
	
	public override void released() {
		base.released();
		InfoWindowManager.Instance.toggleScreen(type);
	}
}
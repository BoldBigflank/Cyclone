using UnityEngine;
using System.Collections;

public class NextButton : SimpleButton {
			
	public override void released() {
		base.released();
		ButtonManager.Instance.selectNextRadioButton((int)InfoWindowManager.Instance.currentScreen, 1, true);
	}
}

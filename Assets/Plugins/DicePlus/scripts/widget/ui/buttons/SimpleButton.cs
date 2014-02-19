using UnityEngine;

public class SimpleButton : DiceButton {
		
	public override void handleTouch(ButtonManager.WTouch touch, Vector3 position, bool start, bool end) {
		base.handleTouch(touch, position, start, end);
		if (end) {
			released();
		}
	}
	
	public virtual void released() {
		runClickAnimation();
	}
}

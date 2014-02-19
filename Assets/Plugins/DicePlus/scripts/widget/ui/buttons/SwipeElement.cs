using UnityEngine;

public class SwipeElement : DiceButton {
	
	const int MOUSE_FINGER_ID = 111053;
	//dragging
	Vector3 clickStartPosition = Vector3.zero;
	Vector3 startPosition = Vector3.zero;
	bool dragging = false;
	
	private int buttonTouchId = -1;
	[HideInInspector]
	public int dragTreshold = 10;
	[HideInInspector]
	public int screen = 0;
	[HideInInspector]
	public InfoStreamAnimator infoScreenAnimator;
	[HideInInspector]
	public Camera cam;
	public override void handleTouch(ButtonManager.WTouch touch, Vector3 position, bool start, bool end) {
		base.handleTouch(touch, position, start, end);
		Vector3 clickNewPosition = cam.ScreenToWorldPoint(position);
		if (start) {
			clickStartPosition = clickNewPosition;
			started = true;
			pressState = true;
			buttonTouchId = (touch!=null?touch.touch.fingerId:MOUSE_FINGER_ID);
			startPosition = infoScreenAnimator.transform.localPosition;

		}
		if (started && buttonTouchId == (touch!=null?touch.touch.fingerId:MOUSE_FINGER_ID)) {
			if (dragging || (clickNewPosition - clickStartPosition).magnitude > dragTreshold) {
				if (!dragging) {
					clickStartPosition = clickNewPosition;
					startPosition = infoScreenAnimator.transform.localPosition;
					dragging = true;
				}
				Vector3 diff = (clickNewPosition - clickStartPosition);
				Vector3 newPosition = startPosition + diff;
				infoScreenAnimator.setPosition(newPosition);
				ButtonManager.Instance.selectNextRadioButton(screen, (int)((Mathf.Abs(diff.x) + InfoStreamAnimator.SCREEN_WIDTH/2f)/InfoStreamAnimator.SCREEN_WIDTH) * (int)Mathf.Sign(-diff.x), false);
			} else {
				if (end) {
					released(clickNewPosition);
					buttonTouchId = -1;
					started = false;
					return;
				}
			}
			if (end) {
				released(clickNewPosition);
				dragging = false;
				buttonTouchId = -1;
				started = false;
				return;
			}
		}
	}
	
	public void released(Vector3 clickNewPosition) {
		Vector3 diff = (clickNewPosition - clickStartPosition);
		ButtonManager.Instance.selectNextRadioButton(screen, (int)((Mathf.Abs(diff.x) + InfoStreamAnimator.SCREEN_WIDTH/2f)/InfoStreamAnimator.SCREEN_WIDTH) * (int)Mathf.Sign(-diff.x), true);
	}
}

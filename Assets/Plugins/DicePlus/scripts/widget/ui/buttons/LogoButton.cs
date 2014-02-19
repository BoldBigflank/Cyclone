using UnityEngine;

public class LogoButton : DiceButton {
		
	float timerInterval = 1f;
	
	const int MOUSE_FINGER_ID = 111053;
	
	//dragging
	Vector3 clickStartPosition = Vector3.zero;
	Vector3 startPosition = Vector3.zero;
	float startTime = 0.0f;
	bool dragging = false;
	
	//positioning
	[HideInInspector]
	public Camera cam;
	
	private int buttonTouchId = -1;
	[HideInInspector]
	public int dragTreshold = 20;
	[HideInInspector]
	public PositionUITool positionTool;
	
	public override void handleTouch(ButtonManager.WTouch touch, Vector3 position, bool start, bool end) {
		base.handleTouch(touch, position, start, end);
		Vector3 clickNewPosition = cam.ScreenToWorldPoint(position);
		if (start) {
			clickStartPosition = clickNewPosition;
			startPosition = positionTool.transform.localPosition;
			startTime = Time.time;
			started = true;
			pressState = true;
			buttonTouchId = (touch!=null?touch.touch.fingerId:MOUSE_FINGER_ID);
		}
		if (started && buttonTouchId == (touch!=null?touch.touch.fingerId:MOUSE_FINGER_ID)) {
			if (positionTool.dragable && (dragging || (clickNewPosition - clickStartPosition).magnitude > dragTreshold)) {
				if (!dragging) {
					clickStartPosition = clickNewPosition;
					startPosition = positionTool.transform.localPosition;
					dragging = true;
					WidgetEventDispather.Instance.notifyWidgetDraggingStart();
					disableTimerAnimation();
				}
				Vector3 newPosition = startPosition + (clickNewPosition - clickStartPosition);
				positionTool.setSnapedPosition(newPosition);
			} else {
				if (DicePlusConnectionManager.Instance.state == DicePlusConnectionManager.State.ROLLER) {
					animation["timer"].speed = 1/Time.timeScale;
					animation.CrossFade("timer");
					if (Time.time > startTime + timerInterval) {
						runClickAnimation();
						DicePlusConnectionManager.Instance.disableRoller();
						buttonTouchId = -1;
						started = false;
						disableTimerAnimation();
						return;
					}
				}
				if (end) {
					released();
					buttonTouchId = -1;
					started = false;
					disableTimerAnimation();
					return;
				}
			}
			if (end) {
				dragging = false;
				WidgetEventDispather.Instance.notifyWidgetDraggingEnd();

				buttonTouchId = -1;
				started = false;
				disableTimerAnimation();
				return;
			}
		}
	}
	
	void disableTimerAnimation() {
		animation["timer"].speed = 0;
		animation["timer"].time = 0;
		animation.CrossFade("timer");
	}
	
	public void released() {
		if (DicePlusConnectionManager.Instance.state == DicePlusConnectionManager.State.ROLLER) {
			runClickAnimation();
			DicePlusConnectionManager.Instance.roll();
		} else {
			runClickAnimation();
			DicePlusConnectionManager.Instance.toggleButtons();
		} 
	}
}

using UnityEngine;

public class InfoStreamAnimator : InfoScreen {

	public override void screenToggled() {
		//return to the first element in info stream
		ButtonManager.Instance.selectRadioButton(0, 0, true);
		
		Vector3 tmp = transform.localPosition;
		tmp.x = 0;
		transform.localPosition = tmp;
	}
	
	[HideInInspector]
	public Vector3 targetPosition;
	[HideInInspector]
	public float dumpingFactor = 0.125f; 
	
	//width of single element info stream
	public static float SCREEN_WIDTH = 600;
	
	void Start() {
		targetPosition = transform.localPosition;
	}
	
	void FixedUpdate() {
		targetPosition.y = transform.localPosition.y;
		transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, dumpingFactor);
	}
	
	public void setTargetPosition(Vector3 pos) {
		targetPosition = pos;
	}
	
	public void setPosition(Vector3 pos) {
		targetPosition = pos;
		targetPosition.y = transform.localPosition.y;
		transform.localPosition = targetPosition;

	}
	[HideInInspector]
	public Transform first;
	//first info stream element
	public Transform getFirst() {
		return first;
	}
	[HideInInspector]
	public Transform last;
	//last info stream element
	public Transform getLast() {
		return last;
	}
}


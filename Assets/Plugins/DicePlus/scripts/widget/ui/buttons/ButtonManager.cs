using UnityEngine;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour {
	
	private static ButtonManager instance;
	public static ButtonManager Instance {
		get {
			return instance;
		}
	}
	
	public class WTouch {
		public Touch touch;
		
		public WTouch(Touch t) {
			touch = t;
		}
	}
	
	void Awake () {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(this);
		}
	}
	
	void OnDestroy() {
		if (this == instance) {
			instance = null;
		}
	}
	
	[HideInInspector]
	public LayerMask layer = 1<<31;
	[HideInInspector]
	public Camera [] cams;
		
	void Update() {
		if (Input.touchCount > 0) {
			checkTouch();
		} else {
			checkMouse();
		}
	}
	
	private void checkTouch () {
		foreach (Touch touch in Input.touches) {
			if (InputManager.Instance.isTouchHandled(touch)) {
				continue;
			}
			handleTouch(new WTouch(touch), touch.position, touch.phase == TouchPhase.Began, touch.phase == TouchPhase.Ended);
        }
	}
	
	private void checkMouse() {
		if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) {
			if (InputManager.Instance.isClickHandled()) {
				return;
			}
			handleTouch(null, Input.mousePosition, Input.GetMouseButtonDown(0), Input.GetMouseButtonUp(0));
		}
	}
	
	private void handleTouch(WTouch touch, Vector3 position, bool start, bool end) {
		foreach(Camera cam in cams) {
			if (!cam.enabled) {
				continue;
			}
			Vector3 vpp = cam.ScreenToViewportPoint(position);
			if (vpp.x < 0f || vpp.x > 1f  || vpp.y < 0f || vpp.y > 1f) {
				continue;
			}
			
			foreach(DiceButton btn in buttons) {
				if (btn.started) {
					if (touch != null) {
						InputManager.Instance.markTouchAsHandled(touch.touch);
					} else {
						InputManager.Instance.markClickAsHandled();
					}
					btn.handleTouch(touch, position, start, end);
					return;
				}
			}

			Ray ray = cam.ScreenPointToRay(position);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000, layer)) {
				foreach(DiceButton btn in buttons) {
					if (hit.transform == btn.transform && !btn.started) {
						if (touch != null) {
							InputManager.Instance.markTouchAsHandled(touch.touch);
						} else {
							InputManager.Instance.markClickAsHandled();
						}
						btn.handleTouch(touch, position, start, end);
						return;
					}
				}
			}
		}
	}
	
	private HashSet<DiceButton> buttons = new HashSet<DiceButton>();
	
	public void registerButton(DiceButton listener) {
		HashSet<DiceButton> tmplisteners = new HashSet<DiceButton>(buttons);
		tmplisteners.Add(listener);
		buttons = tmplisteners;
	}
	
	public void unregisterButton(DiceButton listener) {
		HashSet<DiceButton> tmplisteners = new HashSet<DiceButton>(buttons);
		tmplisteners.Remove(listener);
		buttons = tmplisteners;
	}
	
	Dictionary<int, HashSet<RadioButton>> radiobuttons = new Dictionary<int, HashSet<RadioButton>>();


	public void registerRadioButton(RadioButton listener) {
		HashSet<RadioButton> tmplisteners;
		if (radiobuttons.ContainsKey(listener.radioGroup)) {
			tmplisteners = new HashSet<RadioButton>(radiobuttons[listener.radioGroup]);
		} else {
			tmplisteners = new HashSet<RadioButton>();
		}
		tmplisteners.Add(listener);
		radiobuttons[listener.radioGroup] = tmplisteners;
		
		foreach(RadioButton db in radiobuttons[listener.radioGroup]) {
			db.resetPosition(tmplisteners.Count);
		}
	}
	
	public void unregisterRadioButton(RadioButton listener) {
		HashSet<RadioButton> tmplisteners;
		if (radiobuttons.ContainsKey(listener.radioGroup)) {
			tmplisteners = new HashSet<RadioButton>(radiobuttons[listener.radioGroup]);
		} else {
			tmplisteners = new HashSet<RadioButton>();
		}
		if (tmplisteners.Contains(listener)) {
			tmplisteners.Remove(listener);
		}
		radiobuttons[listener.radioGroup] = tmplisteners;
		
		foreach(RadioButton db in radiobuttons[listener.radioGroup]) {
			db.resetPosition(tmplisteners.Count);
		}
	}
	
	Dictionary<int, int> radioGroup = new Dictionary<int, int>();

	
	public void selectNextRadioButton(int rgroup, int diff, bool updateValue) {
		if (radioGroup.ContainsKey(rgroup) && radiobuttons.ContainsKey(rgroup)) {
			selectRadioButton(rgroup, (radioGroup[rgroup]+radiobuttons[rgroup].Count+diff)%radiobuttons[rgroup].Count, updateValue);
		}
		
		int groupElementCount = radiobuttons[rgroup].Count;

		if (updateValue) {
			if (radioGroup[rgroup] == (groupElementCount-1) && diff < 0) {
				Vector3 pos = InfoWindowManager.Instance.infoScreenAnimator[rgroup].transform.localPosition;
				pos.x -= groupElementCount * InfoStreamAnimator.SCREEN_WIDTH;
				InfoWindowManager.Instance.infoScreenAnimator[rgroup].transform.localPosition = pos;
			}
			if (radioGroup[rgroup] == 0 && diff > 0) {
				Vector3 pos = InfoWindowManager.Instance.infoScreenAnimator[rgroup].transform.localPosition;
				pos.x += groupElementCount * InfoStreamAnimator.SCREEN_WIDTH;
				InfoWindowManager.Instance.infoScreenAnimator[rgroup].transform.localPosition = pos;
			}
		}
	}
	
	public void selectRadioButton(int rgroup, int rvalue, bool updateValue) {
		if (updateValue) {
			radioGroup[rgroup] = rvalue;
			
			Transform last = InfoWindowManager.Instance.infoScreenAnimator[rgroup].getLast();
			Transform first = InfoWindowManager.Instance.infoScreenAnimator[rgroup].getFirst();
			
			int groupElementCount = radiobuttons[rgroup].Count;
			if (radioGroup[rgroup] == 0) {
				last.localPosition = new Vector3(-1 * InfoStreamAnimator.SCREEN_WIDTH, 0, 0);
			} else {
				last.localPosition = new Vector3((groupElementCount - 1) * InfoStreamAnimator.SCREEN_WIDTH, 0, 0);
			}
			if (radioGroup[rgroup] == (groupElementCount - 1)) {
				first.localPosition = new Vector3(groupElementCount * InfoStreamAnimator.SCREEN_WIDTH, 0, 0);
			} else {
				first.localPosition = new Vector3(0, 0, 0);
			}
		}
		if (radiobuttons.ContainsKey(rgroup)) {
			foreach(RadioButton db in radiobuttons[rgroup]) {
				if (db.radioGroup == rgroup) {
					db.setEnabled(db.radioValue == rvalue);
				}
			}
		}
		if (updateValue) {
			foreach(RadioGroupListener rgl in rgls) {
				rgl.onRadioGroupChange(rgroup, rvalue);
			}
		}
	}
	
	private HashSet<RadioGroupListener> rgls = new HashSet<RadioGroupListener>();
	
	public void registerRadioGroupListener(RadioGroupListener listener) {
		HashSet<RadioGroupListener> tmplisteners = new HashSet<RadioGroupListener>(rgls);
		tmplisteners.Add(listener);
		rgls = tmplisteners;
	}
	
	public void unregisterRadioGroupListener(RadioGroupListener listener) {
		HashSet<RadioGroupListener> tmplisteners = new HashSet<RadioGroupListener>(rgls);
		tmplisteners.Remove(listener);
		rgls = tmplisteners;
	}
}

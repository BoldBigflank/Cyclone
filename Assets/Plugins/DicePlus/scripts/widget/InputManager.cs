using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Provides information about input interactions which where handled by widget. It also provides opportunity to mark touches and clicks as handled, which will make widget ignore them.
/// </summary>
public class InputManager : MonoBehaviour {
	
	private static InputManager instance;
	public static InputManager Instance {
		get {
			return instance;
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
	/// <summary>
	/// Marks the touch as handled
	/// </summary>	
	public void markTouchAsHandled(Touch touch) {
		touchesHandled.Add(touch);
	}
	/// <summary>
	/// Checks whether touch is already handled
	/// </summary>
	/// <returns>
	/// true if touch was marked as handled in this update, false otherwise
	/// </returns>
	public bool isTouchHandled(Touch touch) {
		return touchesHandled.Contains(touch);
	}
	
	HashSet<Touch> touchesHandled = new HashSet<Touch>();

	/// <summary>
	/// Marks the mouse click as handled
	/// </summary>
	public void markClickAsHandled() {
		clickHandled = true;
	}
	/// <summary>
	/// Checks whether mouse click is already handled
	/// </summary>
	/// <returns>
	/// true if click was marked as handled in this update, false otherwise
	/// </returns>
	public bool isClickHandled () {
		return clickHandled;
	}
	
	bool clickHandled = false;
	
	void Update() {
		touchesHandled.Clear();
		clickHandled = false;
	}
	
}

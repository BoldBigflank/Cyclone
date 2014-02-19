using UnityEngine;
using System.Collections;

public class InfoWindowManager : MonoBehaviour, RadioGroupListener {
	
	private static InfoWindowManager instance;
	public static InfoWindowManager Instance {
		get {
			return instance;
		}
	}
	
	void Awake () {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}
	}
	
	void OnDestroy() {
		if (this == instance) {
			instance = null;
		}
	}
	
		
	void Start () {
		ButtonManager.Instance.registerRadioGroupListener(this);
	}
	[HideInInspector]
	public Camera infoScreenCamera;
	[HideInInspector]
	public InfoStreamAnimator [] infoScreenAnimator;
	[HideInInspector]
	public InfoScreen [] infoScreens;
	[HideInInspector]
	public Transform [] dots;

	
	public enum ScreenType {
		HELP = 0,
		BUY = 1
	};
	
	[HideInInspector]
	public ScreenType currentScreen;
	
	/// <summary>
	/// Disables info window
	/// </summary>
	public void disableInfoWindow() {
		bool wasEnabled = infoScreenCamera.camera.enabled;
		
		infoScreenCamera.camera.enabled = false;
		
		if (wasEnabled) {
			WidgetEventDispather.Instance.notifyInfoWindowStateChange(infoScreenCamera.camera.enabled);
		}	
	}
	
	/// <summary>
	/// Toggles between screen types.
	/// It also enables info window if it is disabled, and disables it when called with screen type which is currently shown
	/// </summary>
	/// <param name='type'>
	/// screen type
	/// </param>
	public void toggleScreen(ScreenType type) {
		bool wasEnabled = infoScreenCamera.camera.enabled;
		
		if (type == currentScreen) {
			infoScreenCamera.camera.enabled = !infoScreenCamera.camera.enabled;
		} else {
			infoScreenCamera.camera.enabled = true;
		}
		
		if (infoScreenCamera.camera.enabled != wasEnabled) {
			WidgetEventDispather.Instance.notifyInfoWindowStateChange(infoScreenCamera.camera.enabled);
		}
				
		Vector3 tmp;
		
		tmp = infoScreens[(int)currentScreen].transform.parent.localPosition;
		tmp.y = -1000;
		infoScreens[(int)currentScreen].transform.parent.localPosition = tmp;
		
		currentScreen = type;
		
		tmp = infoScreens[(int)currentScreen].transform.parent.localPosition;
		tmp.y = 0;
		infoScreens[(int)currentScreen].transform.parent.localPosition = tmp;
		
				
		infoScreens[(int)currentScreen].screenToggled();
	}
	
	
	public void onRadioGroupChange(int rgroup, int rvalue) {
		infoScreenAnimator[rgroup].targetPosition.x = rvalue * -InfoStreamAnimator.SCREEN_WIDTH;
	}
}

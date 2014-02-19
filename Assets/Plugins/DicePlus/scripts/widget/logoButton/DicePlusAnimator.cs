using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

public class DicePlusAnimator: MonoBehaviour {
	
	[HideInInspector]
	public Transform [] connected;
	[HideInInspector]
	public Transform [] disconnected;
	[HideInInspector]
	public Transform [] searching;
	[HideInInspector]
	public Transform [] lowBattery;
	[HideInInspector]
	public Transform [] rollerElements;
	[HideInInspector]
	public Transform [] version;
	[HideInInspector]
	public Transform [] bluetooth;
	[HideInInspector]
	public Transform text;
	[HideInInspector]
	public TextMesh textLabel;
	
	[HideInInspector]
	public Transform connector;
	
	float hideStartTime = 0;
	float hideTransitionDuration = 0;
	bool hide = false;
	Vector3 hideStartScale = Vector3.zero;
	
	bool forceHide = false;
	/// <summary>
	/// Setting it to true, hides widget disregarding its current state. Setting it back to false, allows widget to change its visibility state
	/// </summary>
	/// <param name='forceHideConnector'>
	/// force hide
	/// </param>
	public void setForceHideConnector(bool forceHideConnector) {
		if (forceHideConnector) {
			OrbitingButtonsManager.Instance.disableTutorial();
		}

		bool before = isHidden();
		
		forceHide = forceHideConnector;
		runHideConnector(hide, 0.25f);
		
		bool after = isHidden();
		
		if (before != after) {
			WidgetEventDispather.Instance.notifyWidgetStateChange(after);
		}
	}
	/// <summary>
	/// Returns current visibility state of widget
	/// </summary>
	/// <returns>
	/// ture if it is hidden, false otherwise
	/// </returns>
	public bool isHidden() {
		return hide || forceHide;
	}
	/// <summary>
	/// Changes visibility state of widget as long as widget is forced to be hidden calling this method won't take effect
	/// </summary>
	/// <param name='hideConnector'>
	/// new hide state
	/// </param>
	/// <param name='time'>
	/// Transition time
	/// </param>
	public void hideConnector(bool hideConnector, float time) {
		bool before = isHidden();
		
		runHideConnector(hideConnector, time);
		
		bool after = isHidden();
		
		if (before != after) {
			WidgetEventDispather.Instance.notifyWidgetStateChange(after);
		}
	}
	
	void runHideConnector(bool hideConnector, float time) {		
		hideStartTime = Time.time;
		hideTransitionDuration = time;
		hide = hideConnector;
		hideStartScale = connector.transform.parent.localScale;
	}
	
	//values provided as an animationId parameter in animationFinished function
	public static int CONNECTED_ANIMATION = 601111;
	public static int SEARCHING_ANIMATION = 5347611;
	
	//function used as callback for unity animations
	public void animationFinished(int animationId) {
		animating = false;
		
		if (DicePlusConnectionManager.Instance != null && DicePlusConnectionManager.Instance.state == DicePlusConnectionManager.State.CONNECTED && animationQueue.Count == 0) {
			hideConnector(true, 0.25f);
			return;
		}

		if (DicePlusConnectionManager.Instance != null && DicePlusConnectionManager.Instance.state != DicePlusConnectionManager.State.ROLLER && DicePlusConnectionManager.Instance.state != DicePlusConnectionManager.State.NO_BLUETOOTH) {
			if (animationId == CONNECTED_ANIMATION) {
				runSearchingMoreAnimation();
			} else {
				if (animationQueue.Count > 0) {
					AnimationDelegate animDelegate = animationQueue.Dequeue();
					animDelegate();
				} else {
					runSearchingAnimation(animationId != SEARCHING_ANIMATION);
				}
			}
		} else {
			if (animationQueue.Count > 0) {
				AnimationDelegate animDelegate = animationQueue.Dequeue();
				animDelegate();
			}
		}
	}
	
	public IEnumerator lowBatteryCorutine() {
		hideConnector(false, 0.0f);
		runLowBatteryAnimation();
		yield return new WaitForSeconds(2.0f);
		if (DicePlusConnectionManager.Instance.state == DicePlusConnectionManager.State.CONNECTED) {
			hideConnector(true, 0.25f);
		}
    }
	
	void disableAll() {
		foreach(Transform tr in lowBattery) {
#if UNITY_3_5
			tr.gameObject.SetActiveRecursively(false);
#else
			tr.gameObject.SetActive(false);
#endif		
		}
		foreach(Transform tr in searching) {
#if UNITY_3_5
			tr.gameObject.SetActiveRecursively(false);
#else
			tr.gameObject.SetActive(false);
#endif		
		}
		foreach(Transform tr in connected) {
#if UNITY_3_5
			tr.gameObject.SetActiveRecursively(false);
#else
			tr.gameObject.SetActive(false);
#endif		
		}
		foreach(Transform tr in disconnected) {
#if UNITY_3_5
			tr.gameObject.SetActiveRecursively(false);
#else
			tr.gameObject.SetActive(false);
#endif		
		}
		foreach(Transform tr in rollerElements) {
#if UNITY_3_5
			tr.gameObject.SetActiveRecursively(false);
#else
			tr.gameObject.SetActive(false);
#endif		
		}
		foreach(Transform tr in version) {
#if UNITY_3_5
			tr.gameObject.SetActiveRecursively(false);
#else
			tr.gameObject.SetActive(false);
#endif		
		}
		foreach(Transform tr in bluetooth) {
#if UNITY_3_5
			tr.gameObject.SetActiveRecursively(false);
#else
			tr.gameObject.SetActive(false);
#endif		
		}
	}
	
	//text info popup background 
	[HideInInspector]
	public CloudScalableSprite label;
	
	public delegate void AnimationDelegate();
	public Queue<AnimationDelegate> animationQueue = new Queue<AnimationDelegate>();
	bool animating = false;
			
	public void runBluetoothDisabled() {
		if (animating) {
			animationQueue.Enqueue(delegate() {
				runBluetoothDisabled();
			});
			return;
		}
		disableAll();
		foreach(Transform tr in bluetooth) {
#if UNITY_3_5
			tr.gameObject.SetActiveRecursively(true);
#else
			tr.gameObject.SetActive(true);
#endif		
		}
		
		hideConnector(false, 0.25f);
		connector.animation["info"].speed = 1f/Time.timeScale;
		connector.animation["info"].wrapMode = WrapMode.ClampForever;
		connector.animation["info"].normalizedTime = 0f;
		connector.animation["info"].layer = 333;		
		connector.animation.Play("info");
		animating = true;

		if (!OrbitingButtonsManager.Instance.isTutorialFired()) {
			runPopupAnimation(Translations.translateKey(Translations.Key.NO_BLUETOOTH));
		}
	}
	
 	public void runPopupAnimation(string popupText) {
		runPopupAnimation(popupText, "popup");
	}
	
	 public void runLongPopupAnimation(string popupText) {
		runPopupAnimation(popupText, "popupLong");
	}
	
	public void runMediumPopupAnimation(string popupText) {
		runPopupAnimation(popupText, "popupMed");
	}

	[HideInInspector]
	public Camera cam;
	[HideInInspector]
	public Transform positioner;	
	[HideInInspector]
	public Transform textAnchor;
	
	void runPopupAnimation(string popupText, string anim) {

		text.animation[anim].speed = 1f/Time.timeScale;
		text.animation[anim].wrapMode = WrapMode.ClampForever;
		text.animation[anim].normalizedTime = 0f;
		text.animation[anim].layer = 222;		
		text.animation.Play(anim);
		
		textLabel.text = popupText.ToUpper();
		
		float letterSize = textLabel.characterSize + 0.5f;
		float xsize = letterSize * (Translations.estimateLength(textLabel.text) + 2f);
		
		label.trans.localScale = new Vector3(xsize, label.trans.localScale.y, label.trans.localScale.z);
		
		updateLabel();
	}
	
	// updates background of text info popup
	public void updateLabel() {
		float variablePartSize = label.getVariablePartLength();

		float distanceFromLeft = Mathf.Abs(-cam.orthographicSize*cam.aspect - positioner.localPosition.x);
		float distanceFromRight = Mathf.Abs(cam.orthographicSize*cam.aspect - positioner.localPosition.x);
		
		float offset = 0;
		
		if (distanceFromRight < label.trans.localScale.x/2f) {
			offset = -(variablePartSize/2f - (distanceFromRight - label.elements[2]/2f - label.elements[4]));
		}
		if (distanceFromLeft < label.trans.localScale.x/2f) {
			offset = (variablePartSize/2f - (distanceFromLeft - label.elements[2]/2f - label.elements[0]));
		}
		
		if (variablePartSize/2f > Mathf.Abs(offset)) {
			textAnchor.localPosition = new Vector3(offset, 0, 0);
			label.elements[1] = -variablePartSize/2f + offset;
			label.elements[3] = -variablePartSize/2f - offset;
		} else {
			textAnchor.localPosition = new Vector3(0, 0, 0);
			label.elements[1] = -1;
			label.elements[3] = -1;
		}
	}

	public void runLowBatteryAnimation() {
		if (animating) {
			animationQueue.Enqueue(delegate() {
				runLowBatteryAnimation();
			});
			return;
		}
		disableAll();
		foreach(Transform tr in lowBattery) {
#if UNITY_3_5
			tr.gameObject.SetActiveRecursively(true);
#else
			tr.gameObject.SetActive(true);
#endif		
		}
		connector.animation["info"].speed = 1f/Time.timeScale;
		connector.animation["info"].wrapMode = WrapMode.ClampForever;
		connector.animation["info"].normalizedTime = 0f;
		connector.animation["info"].layer = 333;		
		connector.animation.Play("info");
		animating = true;

		if (!OrbitingButtonsManager.Instance.isTutorialFired()) {
			runPopupAnimation(Translations.translateKey(Translations.Key.LOW_BATTERY));
		}
	}
	
	public void runConnectedAnimation() {
		if (animating) {
			animationQueue.Enqueue(delegate() {
				runConnectedAnimation();
			});
			return;
		}
		disableAll();
		foreach(Transform tr in connected) {
#if UNITY_3_5
			tr.gameObject.SetActiveRecursively(true);
#else
			tr.gameObject.SetActive(true);
#endif
		}
		connector.animation["connected"].speed = 1f/Time.timeScale;
		connector.animation["connected"].wrapMode = WrapMode.ClampForever;
		connector.animation["connected"].normalizedTime = 0f;
		connector.animation["connected"].layer = 333;		
		connector.animation.Play("connected");
		animating = true;

		OrbitingButtonsManager.Instance.disableTutorial();
		runLongPopupAnimation(Translations.translateKey(Translations.Key.CONNECTED));
	}
	
	public void runRollerAnimation() {
		if (animating) {
		animationQueue.Enqueue(delegate() {
				runRollerAnimation();
			});
			return;
		}
		disableAll();
		foreach(Transform tr in rollerElements) {
#if UNITY_3_5
			tr.gameObject.SetActiveRecursively(true);
#else
			tr.gameObject.SetActive(true);
#endif
		}
		
		connector.animation["info"].speed = 1f/Time.timeScale;
		connector.animation["info"].wrapMode = WrapMode.ClampForever;
		connector.animation["info"].normalizedTime = 0f;
		connector.animation["info"].layer = 333;		
		connector.animation.Play("info");
		animating = true;

	}
	
	public void runDisconnectedAnimation() {
		if (animating) {
			animationQueue.Enqueue(delegate() {
				runDisconnectedAnimation();
			});
			return;
		}
		disableAll();
		foreach(Transform tr in disconnected) {
#if UNITY_3_5
			tr.gameObject.SetActiveRecursively(true);
#else
			tr.gameObject.SetActive(true);
#endif
		}
		hideConnector(false, 0.25f);
		connector.animation["info"].speed = 1f/Time.timeScale;
		connector.animation["info"].wrapMode = WrapMode.ClampForever;
		connector.animation["info"].normalizedTime = 0f;
		connector.animation["info"].layer = 333;		
		connector.animation.Play("info");
		animating = true;

		if (!OrbitingButtonsManager.Instance.isTutorialFired()) {
			runPopupAnimation(Translations.translateKey(Translations.Key.DISCONNECTED));
		}
	}
	
	public void runVersionMissmatchAnimation(bool toNew) {
		if (animating) {
			animationQueue.Enqueue(delegate() {
				runVersionMissmatchAnimation(toNew);
			});
			return;
		}
		disableAll();
		foreach(Transform tr in version) {
#if UNITY_3_5
			tr.gameObject.SetActiveRecursively(true);
#else
			tr.gameObject.SetActive(true);
#endif
		}
		hideConnector(false, 0.25f);
		connector.animation["info"].speed = 1f/Time.timeScale;
		connector.animation["info"].wrapMode = WrapMode.ClampForever;
		connector.animation["info"].normalizedTime = 0f;
		connector.animation["info"].layer = 333;		
		connector.animation.Play("info");
		animating = true;

		if (!OrbitingButtonsManager.Instance.isTutorialFired()) {
			runPopupAnimation(Translations.translateKey(toNew?Translations.Key.TOO_NEW:Translations.Key.TOO_OLD));
		}
	}
	
	public void runSearchingMoreAnimation() {
		if (animating) {
			animationQueue.Enqueue(delegate() {
				runSearchingMoreAnimation();
			});
			return;
		}
		disableAll();
		foreach(Transform tr in searching) {
#if UNITY_3_5
			tr.gameObject.SetActiveRecursively(true);
#else
			tr.gameObject.SetActive(true);
#endif
		}
		bool se = (DicePlusConnectionManager.Instance == null || DicePlusConnectionManager.Instance.isScanningEnabled());
		if (se) {
			connector.animation["searching"].speed = 1f/Time.timeScale;
			connector.animation["searching"].wrapMode = WrapMode.ClampForever;
			connector.animation["searching"].normalizedTime = 0f;
			connector.animation["searching"].layer = 333;		
			connector.animation.Play("searching");
			animating = true;
		}
		if (se && (OrbitingButtonsManager.Instance == null || !OrbitingButtonsManager.Instance.isTutorialFired())) {
			runPopupAnimation(Translations.translateKey(Translations.Key.SEARCH_MORE));
		}
	}
	
	public void runSearchingAnimation(bool runPopup) {
		if (animating) {
			animationQueue.Enqueue(delegate() {
				runSearchingAnimation(runPopup);
			});
			return;
		}
		disableAll();
		foreach(Transform tr in searching) {
#if UNITY_3_5
			tr.gameObject.SetActiveRecursively(true);
#else
			tr.gameObject.SetActive(true);
#endif
		}
		bool se = (DicePlusConnectionManager.Instance == null || DicePlusConnectionManager.Instance.isScanningEnabled());
		if (se) {
			connector.animation["searching"].speed = 1f/Time.timeScale;
			connector.animation["searching"].wrapMode = WrapMode.ClampForever;
			connector.animation["searching"].normalizedTime = 0f;
			connector.animation["searching"].layer = 333;		
			connector.animation.Play("searching");
			animating = true;
		}
		if (se && runPopup && (OrbitingButtonsManager.Instance == null || !OrbitingButtonsManager.Instance.isTutorialFired())) {
			runPopupAnimation(Translations.translateKey(Translations.Key.SEARCHING));
		}
	}
	
	private static DicePlusAnimator instance;
	public static DicePlusAnimator Instance {
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
	
	void Update() {
		if (hideTransitionDuration != 0) {
			connector.transform.parent.localScale = Vector3.Lerp(hideStartScale, (isHidden()?Vector3.zero:new Vector3(1,1,1)), (Time.time - hideStartTime)/hideTransitionDuration);
		} else {
			connector.transform.parent.localScale = (isHidden()?Vector3.zero:new Vector3(1,1,1));
		}
	}
}
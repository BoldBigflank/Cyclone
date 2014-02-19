using UnityEngine;
using System.Collections;

public class RadioButton : SimpleButton {
	
	[HideInInspector]
	public int radioValue;	
	[HideInInspector]
	public GameObject actived;
	[HideInInspector]
	public GameObject deactived;
	[HideInInspector]
	public int radioGroup = 0;
	[HideInInspector]
	public bool enabledOnStart = false;
	
	public override void Start() {
		base.Start();
		ButtonManager.Instance.registerRadioButton(this);
		if (enabledOnStart) {
			ButtonManager.Instance.selectRadioButton(radioGroup, radioValue, true);
		}
		setEnabled(enabledOnStart);
	}
	
	public override void OnDestroy() {
		base.OnDestroy();
		if (ButtonManager.Instance != null) {
			ButtonManager.Instance.unregisterRadioButton(this);
		}
	}
	
	public void resetPosition(int screenCount) {
		transform.localPosition = new Vector3(20f * radioValue - 20f * (screenCount-1)/2f,transform.localPosition.y,transform.localPosition.z);
	}
	
	public void setEnabled(bool enabled) {
#if UNITY_3_5
		actived.SetActiveRecursively(enabled);
		deactived.SetActiveRecursively(!enabled);
#else
		actived.SetActive(enabled);
		deactived.SetActive(!enabled);
#endif
	}
	
	public override void released() {
		base.released();
		ButtonManager.Instance.selectRadioButton(radioGroup, radioValue, true);
	}
}

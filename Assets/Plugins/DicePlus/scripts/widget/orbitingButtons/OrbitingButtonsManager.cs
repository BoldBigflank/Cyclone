using UnityEngine;
using System.Collections;

public class OrbitingButtonsManager : MonoBehaviour
{
	private static OrbitingButtonsManager instance;
	public static OrbitingButtonsManager Instance {
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
	
	
	[HideInInspector]
	public OrbitingElement [] buttons;
	
	bool forceHide = false;
	/// <summary>
	/// Setting it to true, hides widget buttons disregarding its current state. Setting it back to false, allows widget buttons to change its visibility state
	/// </summary>
	/// <param name='forceHideConnector'>
	/// force hide
	/// </param>
	public void setForceHideButtons(bool forceHideButtons) {
		if (forceHideButtons) {
			OrbitingButtonsManager.Instance.disableTutorial();
		}
		forceHide = forceHideButtons;
		showButtons(buttonsShown);
	}
	
	/// <summary>
	/// Returns current visibility state of widget buttons
	/// </summary>
	/// <returns>
	/// ture if buttons are hidden, false otherwise
	/// </returns>
	public bool isHidden() {
		return !buttonsShown || forceHide;
	}
	
	//orbiting buttons management
	bool buttonsShown = false;
	/// <summary>
	/// Changes visibility state of widget buttons as long as widget buttons are forced to be hidden calling this method won't take effect
	/// </summary>
	/// <param name='hideConnector'>
	/// new hide show
	/// </param>
	/// <param name='time'>
	/// Transition time
	/// </param>
	public void showButtons(bool show) {
		if (show) {
			startTutorial();
		} else {
			stopTutorial();
		}
		buttonsShown = show;
		foreach(OrbitingElement oe in buttons) {
			oe.setHidden(isHidden(), false);
		}
	}
	
	public void toggleButtons() {
		showButtons(!buttonsShown);
	}
	
	
	//widget tutorial elements
	[HideInInspector]
	public bool showTutorialOnce = true;
	public void startTutorial() {
		if (showTutorialOnce) {
			StartCoroutine("tutorialCorutine");
		}
		showTutorialOnce = false;
	}
	
	public void stopTutorial() {
		tutorialFired = false;
		StopCoroutine("tutorialCorutine");
	}
	
	[HideInInspector]
	public float waitToShowText = 1f;
	[HideInInspector]
	public float waitToShowHalo = 1f;
	bool tutorialFired = false;
	
	public IEnumerator buyMeCorutine() {
		buttons[1].setHidden(false, false);
		DicePlusAnimator.Instance.runPopupAnimation(Translations.translateKey(Translations.Key.BUY));

		yield return new WaitForSeconds(0.25f);
		buttons[1].GetComponent<DiceButton>().runClickAnimation();
				
		yield return new WaitForSeconds(0.5f);
		buttons[1].GetComponent<DiceButton>().runClickAnimation();
		
		yield return new WaitForSeconds(0.5f);
		buttons[1].setHidden(true, false);
	}
	
	private IEnumerator tutorialCorutine() {
		tutorialFired = true;
		
		yield return new WaitForSeconds(waitToShowHalo);
		if (!tutorialFired) yield break;
		buttons[0].GetComponent<DiceButton>().runAnimation("tutorial");
        
		yield return new WaitForSeconds(waitToShowText);
		if (!tutorialFired) yield break;
		DicePlusAnimator.Instance.runMediumPopupAnimation(Translations.translateKey(Translations.Key.HELP));
		
		yield return new WaitForSeconds(0.5f);
		if (!tutorialFired) yield break;
		buttons[0].GetComponent<DiceButton>().runAnimation("tutorial");
		
		yield return new WaitForSeconds(waitToShowHalo);
		if (!tutorialFired) yield break;
		buttons[1].GetComponent<DiceButton>().runAnimation("tutorial");
		
        yield return new WaitForSeconds(waitToShowText);
		if (!tutorialFired) yield break;
		DicePlusAnimator.Instance.runMediumPopupAnimation(Translations.translateKey(Translations.Key.BUY));
		
		yield return new WaitForSeconds(0.5f);
		if (!tutorialFired) yield break;
		buttons[1].GetComponent<DiceButton>().runAnimation("tutorial");
		
		yield return new WaitForSeconds(waitToShowHalo);
		if (!tutorialFired) yield break;
		buttons[2].GetComponent<DiceButton>().runAnimation("tutorial");
		
        yield return new WaitForSeconds(waitToShowText);
		if (!tutorialFired) yield break;
		DicePlusAnimator.Instance.runMediumPopupAnimation(Translations.translateKey(Translations.Key.ROLLER));
		
		yield return new WaitForSeconds(0.5f);
		if (!tutorialFired) yield break;
		buttons[2].GetComponent<DiceButton>().runAnimation("tutorial");
		
		yield return new WaitForSeconds(waitToShowText);
		tutorialFired = false;
	}
	
	public bool isTutorialFired() {
		return tutorialFired;
	}
	
	public void disableTutorial() {
		tutorialFired = false;
	}
}



using UnityEngine;
using System.Collections.Generic;
using System.Collections;
/// <summary>
/// This class allows you run animations relating to DICE+ interactions
/// </summary>
public class DicePlusSystemAnimation : MonoBehaviour {
			
#region Lifetime events
	
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
	
	private static DicePlusSystemAnimation instance;
	public static DicePlusSystemAnimation Instance {
		get {
			return instance;
		}
	}
	
#endregion
	
	[HideInInspector]
	public Animation sysAnimations;
	
	public void disableAllAnimations() {
		foreach(SimpleSprite sprite in sysAnimations.transform.GetComponentsInChildren<SimpleSprite>()) {
			if (!sprite.Equals("Background")) {
				sprite.enabled = false;
			}
		}
	}
	
	/// <summary>
	/// Stop any animation running
	/// </summary>
	public void stopAnimation() {
		disableAllAnimations();
		//it actually runs one of animation with animation time set to the end
		sysAnimations["roll-time"].speed = 1f/Time.timeScale;
		sysAnimations["roll-time"].wrapMode = WrapMode.ClampForever;
		sysAnimations["roll-time"].normalizedTime = 1f;
		sysAnimations["roll-time"].layer = 444;		
		sysAnimations.Play("roll-time");
	}
	
	/// <summary>
	/// Runs animation showing that roll was to short
	/// </summary>
	public void runRollTimeAnimation() {
		disableAllAnimations();
		
		sysAnimations["roll-time"].speed = 1f/Time.timeScale;
		sysAnimations["roll-time"].wrapMode = WrapMode.ClampForever;
		sysAnimations["roll-time"].normalizedTime = 0f;
		sysAnimations["roll-time"].layer = 444;		
		sysAnimations.Play("roll-time");
	}
	/// <summary>
	/// Runs animation showing how to use DICE+ as a controller
	/// </summary>
	public void runControllerAnimation() {
		disableAllAnimations();

		sysAnimations["controller"].speed = 1f/Time.timeScale;
		sysAnimations["controller"].wrapMode = WrapMode.ClampForever;
		sysAnimations["controller"].normalizedTime = 0f;
		sysAnimations["controller"].layer = 444;		
		sysAnimations.Play("controller");
	}
	/// <summary>
	/// Runs animation showing that roll was invalid due to tilt
	/// </summary>
	public void runRollAngleAnimation() {
		disableAllAnimations();

		sysAnimations["roll-angle"].speed = 1f/Time.timeScale;
		sysAnimations["roll-angle"].wrapMode = WrapMode.ClampForever;
		sysAnimations["roll-angle"].normalizedTime = 0f;
		sysAnimations["roll-angle"].layer = 444;		
		sysAnimations.Play("roll-angle");
	}
	/// <summary>
	/// Runs animation showing that user should interact with tablet
	/// </summary>
	public void runInteractAnimation() {
		disableAllAnimations();
		
		sysAnimations["interact"].speed = 1f/Time.timeScale;
		sysAnimations["interact"].wrapMode = WrapMode.ClampForever;
		sysAnimations["interact"].normalizedTime = 0f;
		sysAnimations["interact"].layer = 444;		
		sysAnimations.Play("interact");
	}
	/// <summary>
	/// Runs animation showing that user should roll the DICE+
	/// </summary>
	public void runRollAnimation() {
		disableAllAnimations();

		sysAnimations["roll"].speed = 1f/Time.timeScale;
		sysAnimations["roll"].wrapMode = WrapMode.ClampForever;
		sysAnimations["roll"].normalizedTime = 0f;
		sysAnimations["roll"].layer = 444;		
		sysAnimations.Play("roll");
	}
	
}
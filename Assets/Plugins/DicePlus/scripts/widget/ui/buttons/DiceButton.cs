using UnityEngine;

public class DiceButton : MonoBehaviour {
	
	public virtual void Start() {
		ButtonManager.Instance.registerButton(this);
	}
	
	public virtual void OnDestroy() {
		if (ButtonManager.Instance != null) {
			ButtonManager.Instance.unregisterButton(this);
		}
	}
	
	protected bool pressState = false;
	protected bool lastUpdatePressState = false;
	[HideInInspector]
	public bool started = false;
	
	public virtual void handleTouch(ButtonManager.WTouch touch, Vector3 position, bool start, bool end) {
		if (started) {
			pressState = true;
		}
	}
	
	void Update() {
		update();
		pressState = false;
	}
	
	public virtual void update() {
		lastUpdatePressState = pressState;
	}
	
	[HideInInspector]
	public string clickAnimationName  = "clicked-halo";
	
	public void runClickAnimation() {
		if (animation != null) {
			animation[clickAnimationName].speed = 1f/Time.timeScale;
			//if (!animation["clicked-halo"].enabled) 
			{
				animation[clickAnimationName].layer = 111;
				animation[clickAnimationName].normalizedTime = 0f;					
				animation.Play(clickAnimationName);
			}
		}
	}
	
	public void runAnimation(string anim) {
		if (animation != null) {
			animation[anim].speed = 1f/Time.timeScale;
			//if (!animation["clicked-halo"].enabled) 
			{
				animation[anim].layer = 111;
				animation[anim].normalizedTime = 0f;					
				animation.Play(anim);
			}
		}
	}
}

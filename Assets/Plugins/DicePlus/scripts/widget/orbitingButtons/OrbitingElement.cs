using UnityEngine;

public class OrbitingElement : MonoBehaviour {
	
	[HideInInspector]
	public Transform axis;
	[HideInInspector]
	public int radius = 100;
	[HideInInspector]
	public float elementRadius = 25;
	[HideInInspector]
	public int position = 0;
	[HideInInspector]
	public int maxPosition = 1;
	
	[HideInInspector]
	public bool hidden = false;
	
	int firstQuarter = 0;
	int quartersAvaliable = 1;
	int quarterAngle = 180;
		
	bool transition = false;
		
	[HideInInspector]
	public AnimationCurve curve;
	[HideInInspector]
	public SimpleSprite sprite;

	[HideInInspector]
	public Camera cam;
	
	void Start() {
		atime = hidden?0:1;
		setHideTimes();
	}
	
	static int Sign(float number) {
    	return number < 0 ? -1 : (number > 0 ? 1 : 0);
 	}
	//check if orbiting button may occupy quarter
	bool checkQuarter(int quarter) {
		float rad =(-45 - 90 * quarter - quarterAngle) * Mathf.Deg2Rad;
		Vector3 point2check = new Vector3(axis.localPosition.x + radius * Sign(Mathf.Cos(rad)), axis.localPosition.y + radius * Sign(Mathf.Sin(rad)), 0);
		return (Mathf.Abs(point2check.x) + elementRadius <= (cam.orthographicSize*cam.aspect)) && (Mathf.Abs(point2check.y) + elementRadius <= (cam.orthographicSize));
	}
	
	void Update() {
		if (hidden && !transition) {
			return;
		}
		float progress = (Time.time - changeTime)  * (hidden?speed:hideSpeed);
		if (!transition) {
			updateQuarters(false);
		} else {
			float angle = 90f * quartersAvaliable * position / (quartersAvaliable!=4?maxPosition:maxPosition+1);
			atime = Mathf.Lerp(startAtime, hidden?0:1, progress);
			float animator = curve.Evaluate(atime);			
			transform.localPosition = new Vector3(animator * radius * Mathf.Cos((quarterAngle - 90 * firstQuarter - angle) * Mathf.Deg2Rad), animator * radius * Mathf.Sin((quarterAngle - 90 * firstQuarter - angle) * Mathf.Deg2Rad), 0);
			//transform.localRotation = Quaternion.Euler(0, 0, 360 * animator);
			//transform.localScale = new Vector3(animator, animator, animator);
			sprite.color = new Color(1f,1f,1f,1f*animator);
			if (progress > 1) {
				transition = false;
				transitionEnded();
			}
		}
	}
	
	//hide show animation
	float changeTime;
	float startAtime;
	float atime;
	float speed = 1f;
	float hideSpeed = 1f;
	
	void setHideTimes() {
		speed = 3.6f - 0.6f * position;
		hideSpeed = 1.8f + 0.6f * position;
	}
	
	public void setHidden(bool newHidden, bool newSheduleShow) {
		if (newHidden == false) {
			updateQuarters(true);
		}
		if (hidden != newHidden) {
			changeTime = Time.time;
			hidden = newHidden;
			startAtime = atime;
		}
		sheduleShow = newSheduleShow;
		transition = true;
	}
	
	bool sheduleShow = false;
	public virtual void transitionEnded() {
		if (sheduleShow) {
			updateQuarters(true);
			setHidden(false, false);
		}
	}
	//check which quarters are available
	public void updateQuarters(bool instantly) {
			bool broken = false;
			int newFirstQuarter = -1;
			int newQuartersAvaliable = 0;
			for (int i = 0; i < 4; i++) {
				if (checkQuarter(i)) {
					if (newFirstQuarter == -1) {
						newFirstQuarter = i;
						newQuartersAvaliable++;
					} else {
						if (broken) {
							newFirstQuarter = i;
						}
						newQuartersAvaliable++;
					}
				} else {
					if (newFirstQuarter != -1) {
						broken = true;
					}
				}
			}
			if ((firstQuarter != newFirstQuarter || quartersAvaliable != newQuartersAvaliable) && !instantly) {
				setHidden(true, true);
			} else {
				firstQuarter = newFirstQuarter;
				quartersAvaliable = newQuartersAvaliable;
			}
	}

}
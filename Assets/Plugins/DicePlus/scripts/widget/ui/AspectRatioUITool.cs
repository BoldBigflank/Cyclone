using UnityEngine;
using System.Collections;

public class AspectRatioUITool : MonoBehaviour {
	[HideInInspector]
	public Camera cam;
	[HideInInspector]
	public float designWidth = 1152f;
	[HideInInspector]
	public float designHeight = 768f;
	
	float prev;
	
	void Start() {
        prev = cam.aspect;
        resetCameraSize();

    }
	
    void Update() {
        if (cam.aspect != prev) {
			prev = cam.aspect;
			resetCameraSize();
			
		}
        
    }
	
	void resetCameraSize () {
		if (cam != null) {
			if (cam.aspect > 1) {
				cam.orthographicSize = Mathf.Max((designWidth/2f)/cam.aspect, (designHeight/2f));
			} else {
				cam.orthographicSize = Mathf.Max((designHeight/2f)*cam.aspect, (designWidth/2f));
			}
		}
	}
}

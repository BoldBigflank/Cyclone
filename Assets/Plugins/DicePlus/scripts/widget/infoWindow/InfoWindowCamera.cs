using UnityEngine;
using System.Collections;

public class InfoWindowCamera : MonoBehaviour {
	
	[HideInInspector]
	public float windowWidth = 768f;
	[HideInInspector]
	public float windowHeight = 512f;
	
	[HideInInspector]
	public float designWidth = 1152f;
	[HideInInspector]
	public float designHeight = 768f;

	float prev;

	
	void Start() {
        prev = (float)Screen.width/(float)Screen.height;

        resetCameraSize();

    }
	
    void Update() {
        if (camera.aspect != prev) {
    	    prev = (float)Screen.width/(float)Screen.height;
			resetCameraSize();
			
		}
        
    }
	
	void resetCameraSize () {
		camera.aspect = windowWidth/windowHeight;
		camera.orthographicSize = windowHeight/2f;
		
		float scale = 1f;
		float aspect = (float)Screen.width/(float)Screen.height;
		if (aspect > 1f) {
			scale = Screen.height/designHeight;
		} else {
			scale = Screen.width/designHeight;
		}
		float xsize = scale*windowWidth/Screen.width;
		float ysize = scale*windowHeight/Screen.height;
		camera.rect = new Rect(0.5f - xsize/2f,0.5f - ysize/2f,xsize,ysize);
	}
	
}

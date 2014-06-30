/**
 * HeadTrackingSampleActivity.cs
 *
 * Copyright (c) 2014 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Use is subject to license terms.
 */

using UnityEngine;

// Sample activity that demonstrates how to check for head position.
// This script assumes the HeadTrackingReceiver script is also attached
// to the game object and that a Player game object is also in the scene.
public class HeadTrackingSampleActivity : MonoBehaviour {

    // Scale at which to move camera based on head movement
    public float CAMERA_DISTANCE = 10.0f;
	public bool test = false;

    // Position of Player object
    private GameObject player;
	private Vector3 lookPosition;

    // Position of the camera object
	private GameObject mainCamera;
    private Vector3 cameraPosition;

	// Fake head
	private GameObject fakeHead;

	// Default camera variables
	public float damping = 6.0F;
	public bool smooth = true;

	public float bigR = 0.75F;
	public float r = 1.25F;
	float t;


	 
    // Initialization
    void Start () {

        // Enable auto screen rotation for everything except PortraitUpsideDown.
//        Screen.autorotateToPortrait = true;
//        Screen.autorotateToLandscapeLeft = true;
//        Screen.autorotateToLandscapeRight = true;
//        Screen.autorotateToPortraitUpsideDown = false;
//        Screen.orientation = ScreenOrientation.AutoRotation;

        // Get the position of the Player object
		player = GameObject.FindGameObjectWithTag("Player");
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		fakeHead = GameObject.FindGameObjectWithTag("FakeHead");

		GameController.headTracking = HeadTrackingReceiver.isAvailable;

		// Default Camera Movement
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
		t = 0.0F;

    }

    // Loops and redraws the UI
    void OnGUI () {

        GUI.skin.label.fontSize = 20;
        GUI.skin.label.fontStyle = FontStyle.Bold;

        if (!HeadTrackingReceiver.isAvailable) {
//            GUI.Label (new Rect (15, 50, 500, 50), "Head tracking APIs are not supported on this device.");
            return;
        }

        // Display the latest head tracking event data
//        if (HeadTrackingReceiver.lastEvent != null && HeadTrackingReceiver.lastEvent.isTracking) {
//
//            GUI.Label (new Rect (15, 50, 500, 50), "Last Received Head Tracking Event");
//
//            GUI.skin.label.fontStyle = FontStyle.Normal;
//
//            GUI.Label (new Rect (50, 100, 500, 50), "X-Coordinate = " + HeadTrackingReceiver.lastEvent.x + " m");
//            GUI.Label (new Rect (50, 150, 500, 50), "Y-Coordinate = " + HeadTrackingReceiver.lastEvent.y + " m");
//            GUI.Label (new Rect (50, 200, 500, 50), "Z-Coordinate = " + HeadTrackingReceiver.lastEvent.z + " m");
//            GUI.Label (new Rect (50, 250, 500, 50), "Head Inclination Angle = " + HeadTrackingReceiver.lastEvent.headInclinationAngle + " degs");
//            GUI.Label (new Rect (50, 300, 500, 50), "Relative Orientation = " + HeadTrackingReceiver.lastEvent.orientation);
//            GUI.Label (new Rect (50, 350, 500, 50), "Is Face Detected? " + HeadTrackingReceiver.lastEvent.isFaceDetected);
//            GUI.Label (new Rect (50, 400, 500, 50), "Is Tracking? " + HeadTrackingReceiver.lastEvent.isTracking);
//            GUI.Label (new Rect (50, 450, 500, 50), "Timestamp = " + HeadTrackingReceiver.lastEvent.timestamp + "nsecs");
//        } else {
//            GUI.Label (new Rect (15, 50, 500, 50),
//                           "No head tracking event data.");
//        }
    }

    void LateUpdate() {
        // If an event is available and the data is valid, move the camera
        if (HeadTrackingReceiver.isAvailable &&
            (HeadTrackingReceiver.lastEvent != null && HeadTrackingReceiver.lastEvent.isTracking)) {

			cameraPosition.x = HeadTrackingReceiver.lastEvent.x;
			cameraPosition.y = HeadTrackingReceiver.lastEvent.y;
//			cameraPosition.z = HeadTrackingReceiver.lastEvent.z * -1.0F;
			cameraPosition.z = HeadTrackingReceiver.lastEvent.z;

			mainCamera.transform.rotation = Quaternion.LookRotation(Vector3.Scale(cameraPosition, new Vector3(-1.0F, -1.0F, 1.0F) ) ) ;
			// This will move the camera to the angle 
			mainCamera.transform.position = (Vector3.Scale( Vector3.forward, player.transform.position))  + Vector3.back * CAMERA_DISTANCE;
			

			// *** OLD CODE ***
			// Move the camera in the X/Y plane based on the most recent head tracking data
//            cameraPosition.x = HeadTrackingReceiver.lastEvent.x * HEAD_MOVEMENT_SCALE;
//            cameraPosition.y = HeadTrackingReceiver.lastEvent.y * HEAD_MOVEMENT_SCALE;
//            transform.position = cameraPosition;
//            transform.LookAt(playerPosition);
        } else {
			// Use a fake head vector
			cameraPosition.x = fakeHead.transform.position.x;
			cameraPosition.y = fakeHead.transform.position.y;
			cameraPosition.z = fakeHead.transform.position.z;
//			cameraPosition.z = fakeHead.transform.position.z * -1.0F;


			// Default camera movement
			// Look at and dampen the rotation
			Vector3 lookPosition = new Vector3(player.transform.position.x * 0.75F, player.transform.position.y * 0.75F, player.transform.position.z);
			Quaternion rotation = Quaternion.LookRotation(lookPosition - mainCamera.transform.position);
			mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, rotation, Time.deltaTime * damping);

			t += Time.deltaTime;
			Vector3 pos = new Vector3();

			pos.z = player.transform.position.z - CAMERA_DISTANCE;
			pos.x = (bigR - r) * Mathf.Cos(  t ) + r * Mathf.Cos( (bigR - r)/r * t );
			pos.y = (bigR - r) * Mathf.Sin(  t ) + r * Mathf.Sin( (bigR - r)/r * t );		
			mainCamera.transform.position = pos;
		}


    }
}

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
    private static readonly float HEAD_MOVEMENT_SCALE = 100.0f;

    // Position of Player object
    private Vector3 playerPosition;

    // Position of the camera object
    private Vector3 cameraPosition;

    // Initialization
    void Start () {

        // Enable auto screen rotation for everything except PortraitUpsideDown.
        Screen.autorotateToPortrait = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.orientation = ScreenOrientation.AutoRotation;

        // Get the position of the Player object
        playerPosition = GameObject.FindWithTag("Player").transform.position;

        // Set the initial position of the camera
        cameraPosition = transform.position;

        // Have camera look at the Player object
        transform.LookAt(playerPosition);
    }

    // Loops and redraws the UI
    void OnGUI () {

        GUI.skin.label.fontSize = 20;
        GUI.skin.label.fontStyle = FontStyle.Bold;

        if (!HeadTrackingReceiver.isAvailable) {
            GUI.Label (new Rect (15, 50, 500, 50), "Head tracking APIs are not supported on this device.");
            return;
        }

        // Display the latest head tracking event data
        if (HeadTrackingReceiver.lastEvent != null && HeadTrackingReceiver.lastEvent.isTracking) {

            GUI.Label (new Rect (15, 50, 500, 50), "Last Received Head Tracking Event");

            GUI.skin.label.fontStyle = FontStyle.Normal;

            GUI.Label (new Rect (50, 100, 500, 50), "X-Coordinate = " + HeadTrackingReceiver.lastEvent.x + " m");
            GUI.Label (new Rect (50, 150, 500, 50), "Y-Coordinate = " + HeadTrackingReceiver.lastEvent.y + " m");
            GUI.Label (new Rect (50, 200, 500, 50), "Z-Coordinate = " + HeadTrackingReceiver.lastEvent.z + " m");
            GUI.Label (new Rect (50, 250, 500, 50), "Head Inclination Angle = " + HeadTrackingReceiver.lastEvent.headInclinationAngle + " degs");
            GUI.Label (new Rect (50, 300, 500, 50), "Relative Orientation = " + HeadTrackingReceiver.lastEvent.orientation);
            GUI.Label (new Rect (50, 350, 500, 50), "Is Face Detected? " + HeadTrackingReceiver.lastEvent.isFaceDetected);
            GUI.Label (new Rect (50, 400, 500, 50), "Is Tracking? " + HeadTrackingReceiver.lastEvent.isTracking);
            GUI.Label (new Rect (50, 450, 500, 50), "Timestamp = " + HeadTrackingReceiver.lastEvent.timestamp + "nsecs");
        } else {
            GUI.Label (new Rect (15, 50, 500, 50),
                           "No head tracking event data.");
        }
    }

    void Update() {

        // Exit application
        if (Input.GetKey(KeyCode.Escape)) {
           Application.Quit();
        }

        // If an event is available and the data is valid, move the camera
        if (HeadTrackingReceiver.isAvailable &&
            (HeadTrackingReceiver.lastEvent != null && HeadTrackingReceiver.lastEvent.isTracking)) {

            // Move the camera in the X/Y plane based on the most recent head tracking data
            cameraPosition.x = HeadTrackingReceiver.lastEvent.x * HEAD_MOVEMENT_SCALE;
            cameraPosition.y = HeadTrackingReceiver.lastEvent.y * HEAD_MOVEMENT_SCALE;
            transform.position = cameraPosition;
            transform.LookAt(playerPosition);
        }

    }
}

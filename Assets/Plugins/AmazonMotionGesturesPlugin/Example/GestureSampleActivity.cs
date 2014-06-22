/**
 * GestureSampleActivity.cs
 *
 * Copyright (c) 2014 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Use is subject to license terms.
 */

using UnityEngine;

// Sample activity that demonstrates how to check for gesture events.
// This script assumes the GestureReceiver script is also attached
// to the game object and that a Player game object is also in the scene.
public class GestureSampleActivity : MonoBehaviour {

    // Speed at which to rotate around Player object on peek events
    // in degrees per second.
    private static readonly float PEEK_ROTATION_SPEED = 150.0f;

    // Position of Player object
    private Vector3 playerPosition;

    // Timestamp of the last processed tilt event. Since this sample polls GestureReceiver.lastEvent
    // instead of establishing its own the gesture event callback, this is used to determine
    // whether or not a given tilt event was already processed.
    private float lastProcessedTiltTimestamp;

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

        // Have camera look at the Player object
        transform.LookAt(playerPosition);
    }

    // Loops and redraws the UI with the latest gesture event data.
    void OnGUI () {

        GUI.skin.label.fontSize = 20;
        GUI.skin.label.fontStyle = FontStyle.Bold;

        if (!GestureReceiver.isAvailable) {
            GUI.Label (new Rect (15, 50, 500, 50), "Motion gestures APIs are not supported on this device.");
            return;
        }

        // Display the latest gesture event data.
        if (GestureReceiver.lastEvent != null) {

            GUI.Label (new Rect (15, 50, 500, 50), "Last Received Gesture Event");

            GUI.skin.label.fontStyle = FontStyle.Normal;

            GUI.Label (new Rect (50, 100, 500, 50), "Id = " + GestureReceiver.lastEvent.gestureId);
            GUI.Label (new Rect (50, 150, 500, 50), "Direction = " + GestureReceiver.lastEvent.direction);
            GUI.Label (new Rect (50, 200, 500, 50), "Action = " + GestureReceiver.lastEvent.action);
            GUI.Label (new Rect (50, 250, 500, 50), "Magnitude = " + GestureReceiver.lastEvent.magnitude);
            GUI.Label (new Rect (50, 300, 500, 50), "Relative Orientation = " + GestureReceiver.lastEvent.orientation);

        } else {
            GUI.Label (new Rect (15, 50, 500, 50),
                           "No gesture event data.");
        }
    }

    // Process the last received gesture event to alter the view of Player object.
    // Peek left/right rotates around the Player object.
    // Tilt forward/back moves towards or away from the Player object.
    void Update() {

        // Exit application
        if (Input.GetKey(KeyCode.Escape)) {
           Application.Quit();
        }

        if (!GestureReceiver.isAvailable) {
            return;
        }

        // Get the last received gesture event
        GestureEvent currentEvent = GestureReceiver.lastEvent;

        if (currentEvent != null) {
            switch (currentEvent.gestureId) {
                case GestureEvent.GestureId.PEEK:

                    if (currentEvent.action == GestureEvent.Action.ON) {
                        // Rotate the object based on the direction of the event
                        RotateAroundPlayer(currentEvent.direction, PEEK_ROTATION_SPEED);
                    }
                    break;

                case GestureEvent.GestureId.TILT:

                    if (currentEvent.timestamp != lastProcessedTiltTimestamp) {

                        // If there is a new tilt gesture event, move the camera either
                        // further away or closer based on the direction of the tilt gesture.
                        Vector3 dir = playerPosition - transform.position;
                        dir = dir.normalized;

                        if (currentEvent.direction == GestureEvent.Direction.FORWARD) {
                            // move away from Player object
                            transform.Translate(-dir, Space.World);
                        } else if(currentEvent.direction == GestureEvent.Direction.BACK) {
                            // move towards Player object
                            transform.Translate(dir, Space.World);
                        }

                        // update timestamp of last processed tilt event
                        lastProcessedTiltTimestamp = currentEvent.timestamp;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Rotate around the Player game object based on the given direction and speed.
    /// </summary>
    /// <param name='direction'>
    /// Gesture event direction required for determining the rotation direction.
    /// </param>
    /// <param name='speed'>
    /// Speed at which to rotate around the Player game object.
    /// </param>
    void RotateAroundPlayer(GestureEvent.Direction direction, float speed) {
        switch (direction){
            case GestureEvent.Direction.BACK:
                transform.RotateAround(playerPosition, Vector3.right, Time.deltaTime * speed);
                break;
            case GestureEvent.Direction.FORWARD:
                transform.RotateAround(playerPosition, Vector3.left, Time.deltaTime * speed);
                break;
            case GestureEvent.Direction.RIGHT:
                transform.RotateAround(playerPosition, Vector3.up, Time.deltaTime * speed);
                break;
            case GestureEvent.Direction.LEFT:
                transform.RotateAround(playerPosition, Vector3.down, Time.deltaTime * speed);
                break;
        }
    }
}

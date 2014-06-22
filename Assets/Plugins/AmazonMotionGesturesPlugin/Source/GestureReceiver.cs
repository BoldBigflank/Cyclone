/**
 * GestureReceiver.cs
 *
 * Copyright (c) 2014 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Use is subject to license terms.
 */

using UnityEngine;

// Default component used for receiving gesture events.
// This component will register for GestureEvent.GestureId.TILT and
// GestureEvent.GestureId.PEEK events with the direction masks provided
// as DEFAULT_TILT_DIRECTIONS and DEFAULT_PEEK_DIRECTIONS.
//
// The lastEvent field will be updated to reflect the last received GestureEvent.
public class GestureReceiver : MonoBehaviour,GestureListener {

    // The last received gesture event
    public static GestureEvent lastEvent;

    // Status of motion gestures on device, updated in Start()
    public static bool isAvailable = false;

    // Instance of the gesture manager
    private static GestureManager gestureManager;

    // Represents the current orientation of the application.
    private static ScreenOrientation currentOrientation = ScreenOrientation.Portrait;

    // Default directions for which to receive gesture events relative to current ScreenOrientation.
    private static readonly int DEFAULT_TILT_DIRECTIONS = (int) (GestureEvent.Direction.FORWARD |
                                            GestureEvent.Direction.BACK);

    private static readonly int DEFAULT_PEEK_DIRECTIONS = (int) (GestureEvent.Direction.LEFT |
                                            GestureEvent.Direction.RIGHT);

    // Initialization
    void Start () {

        // Determine if motion gestures are supported on device
        isAvailable = GestureManager.IsAvailable();

        if (isAvailable) {
            // Retrieve an instance of the gesture manager.
            gestureManager = GestureManager.CreateInstance();

            // Register for gesture events.
            if(gestureManager != null) {

                // Register game object for GestureEvent.GestureId.TILT with DEFAULT_TILT_DIRECTIONS.
                gestureManager.RegisterListener(gameObject.name,
                                                GestureEvent.GestureId.TILT,
                                                DEFAULT_TILT_DIRECTIONS);

                // Register game object for GestureEvent.GestureId.PEEK with DEFAULT_PEEK_DIRECTIONS.
                gestureManager.RegisterListener(gameObject.name,
                                                GestureEvent.GestureId.PEEK,
                                                DEFAULT_PEEK_DIRECTIONS);

            }
        }
    }

    void Update () {

        // On orientation change, it is important to set the new relative orientation to ensure
        // the correct gesture events are received based on the registered directions.
        if (isAvailable && (currentOrientation != Screen.orientation) && (gestureManager != null)) {

            // Update the relative orientation
            currentOrientation = Screen.orientation;
            gestureManager.SetRelativeOrientation(currentOrientation);
        }
    }

    void OnApplicationQuit () {

        if(isAvailable && gestureManager != null) {
            // Unregister game object from all gesture events.
            gestureManager.UnregisterListener(gameObject.name);
        }

    }

    // Callback to receive gesture events.
    public void OnGestureEvent(string eventJSON) {

        // Convert JSON into GestureEvent
        lastEvent = GestureEvent.GetGestureEventFromJSON(eventJSON);
    }

}

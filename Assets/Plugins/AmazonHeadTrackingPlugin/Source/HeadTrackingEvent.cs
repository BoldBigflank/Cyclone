/**
 * HeadTrackingEvent.cs
 *
 * Copyright (c) 2014 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Use is subject to license terms.
 */

using AmazonCommon;
using System;
using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;

// Event used to represent head tracking data.
public class HeadTrackingEvent {

    // Import C API from native head tracking library

#if UNITY_ANDROID && !UNITY_EDITOR

    [DllImport("headtracking_client")]
    private static extern IntPtr Amazon_HeadTrackingEvent_createInstance();

    [DllImport("headtracking_client")]
    private static extern void Amazon_HeadTrackingEvent_extractHeadTrackingData(IntPtr headTrackingEvent,
                                                                                out float x_mm,
                                                                                out float y_mm,
                                                                                out float z_mm,
                                                                                out float headInclinationAngle_deg,
                                                                                out bool isTracking,
                                                                                out bool isFaceDetected,
                                                                                out long timestamp_nsecs);

#endif

    // The pointer to the native head tracking event.
    public IntPtr nativeHeadTrackingEvent;

    // X-coordinate head position value in meters relative to the center of the device.
    public float x;

    // Y-coordinate head position value in meters relative to the center of the device.
    public float y;

    // Z-coordinate head position value in meters relative to the center of the device.
    public float z;

    // The sideways inclination of the head relative to the device in degrees.
    // If the head is aligned straight with respect to the device in portrait orientation, the
    // inclination angle is 0. It is a positive value if the head rotates clockwise, and negative
    // if it rotates counterclockwise. The range of values is from -180 to 179 degrees, inclusive.
    public float headInclinationAngle;

    // The relative orientation associated with this event.
    public ScreenOrientation orientation;

    // True if the head is currently being tracked and the data is valid
    // The data associated with this event should not be used if this field is false.
    public bool isTracking;

    // True if the face is detected for this event
    public bool isFaceDetected;

    // The time of event occurrence in nanoseconds
    public long timestamp;

    // If true, rotates events according to screen orientation.
    public static bool ROTATE_EVENT_TO_SCREEN_ORIENTATION = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeadTrackingEvent"/> class with
    /// the provided pointer to the corresponding native head tracking event.
    /// </summary>
    /// <param name='nativeEvent'>
    /// The pointer to the native HeadTrackingEvent.
    /// </param>
    private HeadTrackingEvent(IntPtr nativeEvent) {
        nativeHeadTrackingEvent = nativeEvent;
    }

    /// <summary>
    /// Extracts the data from the native HeadTrackingEvent associated with this object.
    /// </summary>
    /// <param name='headTrackingEvent'>
    /// Pointer to the native HeadTrackingEvent.
    /// </param>
    public void UpdateHeadTrackingData() {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Extract the data from the native HeadTrackingEvent.
        Amazon_HeadTrackingEvent_extractHeadTrackingData(nativeHeadTrackingEvent,
                                                         out x,
                                                         out y,
                                                         out z,
                                                         out headInclinationAngle,
                                                         out isTracking,
                                                         out isFaceDetected,
                                                         out timestamp);

        // Check to ensure the data is valid
        if (isTracking) {

            // Convert millimeters to meters, 1 float = 1 meter
            x = x / 1000;
            y = y / 1000;
            z = z / 1000;

            // All data from the native API is relative to ScreenOrientation.Portrait.
            orientation = ScreenOrientation.Portrait;

            if(HeadTrackingEvent.ROTATE_EVENT_TO_SCREEN_ORIENTATION) {
                // Rotate the event to accurately account for the ScreenOrientation
                RotateEventToScreenOrientation();
            }
        }
#else
        isTracking = false;
#endif
    }

    /// <summary>
    /// Rotates the event based on the current ScreenOrientation. All events received from the
    /// the native API are relative to ScreenOrientation.Portrait. This method will update the
    /// head tracking event's data to be relative to the current ScreenOrientation.
    /// </summary>
    private void RotateEventToScreenOrientation() {

        if (Screen.orientation == ScreenOrientation.Portrait || orientation != ScreenOrientation.Portrait) {
            // All event data from the native API is provided in portrait mode.
            // If the incoming event was already rotated, do not rotate again.
        } else {
            if (Screen.orientation == ScreenOrientation.LandscapeLeft) {
                float temp = x;
                x = -y;
                y = temp;
                headInclinationAngle += 270.0f;
                orientation = ScreenOrientation.LandscapeLeft;
            } else if (Screen.orientation == ScreenOrientation.LandscapeRight) {
                float temp = x;
                x = y;
                y = -temp;
                headInclinationAngle += 90.0f;
                orientation = ScreenOrientation.LandscapeRight;
            } else {
                y = -y;
                x = -x;
                headInclinationAngle += 180.0f;
                orientation = ScreenOrientation.PortraitUpsideDown;
            }


            // Normalize between 180.0f inclusive and -180.0f exclusive
            if (headInclinationAngle > 180.0f) {
                headInclinationAngle -= 360.0f;
            }

        }
    }

    /// <summary>
    /// Creates an instance of a HeadTrackingEvent.
    /// </summary>
    /// <returns>
    /// Instance of a HeadTrackingEvent to use for polling or null if it cannot
    /// initialized properly.
    /// </returns>
    public static HeadTrackingEvent CreateInstance() {

#if UNITY_ANDROID && !UNITY_EDITOR
        IntPtr nativeEvent = Amazon_HeadTrackingEvent_createInstance();

        if(nativeEvent == IntPtr.Zero) {
            Debug.LogError("Could not create native HeadTrackingEvent");
            return null;
        }

        return new HeadTrackingEvent(nativeEvent);
#else
        return null;
#endif
    }
}

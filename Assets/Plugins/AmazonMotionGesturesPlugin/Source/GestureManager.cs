/**
 * GestureManager.cs
 *
 * Copyright (c) 2014 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Use is subject to license terms.
 */

using UnityEngine;
using System;

// This class enables a Unity application to register for gesture events.
public class GestureManager {

#if UNITY_ANDROID && !UNITY_EDITOR
    // Reference to the java GestureManager
    private AndroidJavaObject javaGestureManager;

    // Singleton reference
    private static GestureManager gestureManager;

    // Java manager class name
    private static readonly string GESTURES_JAVA_PROXY_CLASS_NAME = "com.amazon.motiongestures.UnityGestureManager";

    // Software feature name associated with the motion gestures APIs on device
    private static readonly string FEATURE_MOTION_GESTURES = "com.amazon.software.motiongestures";
#endif

    /// <summary>
    /// Creates an instance of self for Unity applications to use.
    /// </summary>
    /// <returns>
    /// The instance or null if not initialized properly.
    /// </returns>
    public static GestureManager CreateInstance() {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (gestureManager == null) {
            gestureManager = new GestureManager();
            if(gestureManager.javaGestureManager == null || gestureManager.javaGestureManager.GetRawObject() == System.IntPtr.Zero) {
                Debug.LogError("The java GestureManager was not initialized properly.");
                // set back to null since native HeadTrackingManager was not initialized properly.
                gestureManager = null;
            }
        }
        return gestureManager;
#else
        return null;
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GestureManager"/> class.
    /// </summary>
    private GestureManager() {
#if UNITY_ANDROID && !UNITY_EDITOR
        try {
            javaGestureManager = new AndroidJavaObject(GestureManager.GESTURES_JAVA_PROXY_CLASS_NAME);
        } catch {
            Debug.LogError("Could not obtain java GestureManager.");
        }
#endif
    }

    /// <summary>
    /// Register the provided game object for gesture events.
    /// The game object must implement the GestureListener interface.
    /// This method can be called multiple times to register the same listener
    /// for additional gesture ids. Registering for the same gesture id multiple times
    /// will update the registered directions and clear and previously registered directions
    /// for the given gesture id.
    /// </summary>
    /// <param name='gameObj'>
    /// The game object to register for events.
    /// </param>
    /// <param name='gestureId'>
    /// The gesture event id for which to receive events.
    /// </param>
    /// <param name='directions'>
    /// The gesture directions relative to the most recent orientation value provided to
    /// SetRelativeOrientation(ScreenOrientation) for which to receive events for the provided gesture id.
    /// </param>
    public void RegisterListener(string gameObj, GestureEvent.GestureId gestureId, int directions) {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (gestureManager.javaGestureManager == null) {
            Debug.LogWarning("javaGestureManager is not initialized. Unable to register.");
            return;
        }

        gestureManager.javaGestureManager.Call("registerListener", gameObj, (int)gestureId, directions);
#endif
    }

    /// <summary>
    /// Unregisters the provided game object from all gesture events.
    /// </summary>
    /// <param name='gameObj'>
    /// The game object to unregister.
    /// </param>
    public void UnregisterListener(string gameObj) {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (gestureManager.javaGestureManager == null) {
            Debug.LogWarning("javaGestureManager is not initialized. Unable to unregister.");
            return;
        }

        gestureManager.javaGestureManager.Call("unregisterListener", gameObj);
#endif
    }

    /// <summary>
    /// Unregisters the provided game object from all events of the provided gesture.
    /// </summary>
    /// <param name='gameObj'>
    /// The game object to unregister.
    /// </param>
    /// <param name='gestureId'>
    /// The gesture event id from which to unregister for gesture events.
    /// </param>
    public void UnregisterListener(string gameObj, GestureEvent.GestureId gestureId) {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (gestureManager.javaGestureManager == null) {
            Debug.LogWarning("javaGestureManager is not initialized. Unable to unregister.");
            return;
        }

        gestureManager.javaGestureManager.Call("unregisterListener", gameObj, (int)gestureId);
#endif
    }

    /// <summary>
    /// Sets the relative orientation for gesture events. You must register for gesture event
    /// directions relative to the provided orientation.
    /// The default orientation is set to ScreenOrientation.Portrait.
    /// </summary>
    /// <param name='orientation'>
    /// The ScreenOrientation value.
    /// </param>
    public void SetRelativeOrientation(ScreenOrientation orientation) {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (gestureManager.javaGestureManager == null) {
            Debug.LogWarning("javaGestureManager is not initialized. Unable to set relative orientation.");
            return;
        }

        gestureManager.javaGestureManager.Call("setRelativeOrientation", (int)orientation);
#endif
    }

    /// <summary>
    /// Helper API to determine whether or not the motion gesture APIs are supported
    /// and available on the device.
    /// </summary>
    /// <returns>
    /// True if the motion gesture APIs are supported and available, otherwise false.
    /// </returns>
    public static bool IsAvailable() {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject pm = activity.Call<AndroidJavaObject>("getPackageManager");
        return pm.Call<bool>("hasSystemFeature", FEATURE_MOTION_GESTURES);
#else
        return false;
#endif
    }

}

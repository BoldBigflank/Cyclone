/**
 * HeadTrackingManager.cs
 *
 * Copyright (c) 2014 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Use is subject to license terms.
 */

using UnityEngine;
using System;
using System.Runtime.InteropServices;

// This class enables a Unity application to create pollers used to receive
// the latest head tracking event data.
public class HeadTrackingManager {

#if UNITY_ANDROID && !UNITY_EDITOR

    // Import C API from native head tracking library
    [DllImport("headtracking_client")]
    private static extern IntPtr Amazon_HeadTrackingManager_getInstance();

    [DllImport("headtracking_client")]
    private static extern IntPtr Amazon_HeadTrackingManager_createPoller(IntPtr headtrackingManager);

    [DllImport("headtracking_client")]
    private static extern IntPtr Amazon_HeadTrackingManager_createPollerWithConfig(IntPtr headtrackingManager,
                                                                                   IntPtr headtrackingConfiguration);

    [DllImport("headtracking_client")]
    private static extern void Amazon_HeadTrackingManager_releasePoller(IntPtr headtrackingManager,
                                                                        IntPtr headtrackingPoller);

    [DllImport("headtracking_client")]
    private static extern void Amazon_HeadTrackingManager_requestStandby(IntPtr headtrackingManager,
                                                                         bool standby);

    // Pointer to the native HeadTrackingManager
    private IntPtr nativeHeadTrackingManager;

    // Singleton reference
    private static HeadTrackingManager headTrackingManager;

    // Software feature name associated with the head tracking APIs on device
    private static readonly string FEATURE_HEAD_TRACKING = "com.amazon.software.headtracking";

#endif

    /// <summary>
    /// Creates instance of self for unity applications to use.
    /// </summary>
    /// <returns>
    /// The manager or null if the native HeadTrackingManager is not initialized properly.
    /// </returns>
    public static HeadTrackingManager CreateInstance() {

#if UNITY_ANDROID && !UNITY_EDITOR
        if (headTrackingManager == null) {

            headTrackingManager = new HeadTrackingManager();
            if (headTrackingManager.nativeHeadTrackingManager == IntPtr.Zero) {
                Debug.LogError("Could not obtain the native HeadTrackingManager");

                // set back to null since native HeadTrackingManager was not initialized properly.
                headTrackingManager = null;
            }
        }

        return headTrackingManager;
#else
        return null;
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeadTrackingManager"/> class.
    /// </summary>
    private HeadTrackingManager() {
#if UNITY_ANDROID && !UNITY_EDITOR
        nativeHeadTrackingManager = Amazon_HeadTrackingManager_getInstance();
#endif
    }

    /// <summary>
    /// Creates a HeadTrackingPoller with the default HeadTrackingConfiguration.
    /// </summary>
    /// <returns>
    /// The poller or null if the native HeadTrackingManager or HeadTrackingPoller is not initialized properly.
    /// </returns>
    public HeadTrackingPoller CreatePoller() {

#if UNITY_ANDROID && !UNITY_EDITOR
        if (nativeHeadTrackingManager == IntPtr.Zero) {
            Debug.LogWarning("The native HeadTrackingManager is not initialized. Unable to create poller.");
            return null;
        }

        // Obtain poller from the native api
        IntPtr poller = Amazon_HeadTrackingManager_createPoller(nativeHeadTrackingManager);

        if (poller == IntPtr.Zero) {
            Debug.LogWarning("Unable to obtain native HeadTrackingPoller.");
            return null;
        }

        // return new poller
        return new HeadTrackingPoller(poller);
#else
        return null;
#endif
    }

    /// <summary>
    /// Creates a HeadTrackingPoller with the provided HeadTrackingConfiguration.
    /// If the provided HeadTrackingConfiguration is not valid, the default configuration will be used instead.
    /// </summary>
    /// <returns>
    /// The poller or null if the native HeadTrackingManager or HeadTrackingPoller is not initialized properly.
    /// </returns>
    /// <param name='config'>
    /// The configuration to associate with this poller.
    /// </param>
    public HeadTrackingPoller CreatePoller(HeadTrackingConfiguration config) {

#if UNITY_ANDROID && !UNITY_EDITOR
        if (nativeHeadTrackingManager == IntPtr.Zero) {
            Debug.LogWarning("The native HeadTrackingManager is not initialized. Unable to create poller.");
            return null;
        }

        IntPtr poller;

        if(config == null) {
            Debug.LogWarning("The provided HeadTrackingConfiguration is not initialized properly. Using the default HeadTrackingConfiguration instead.");
            poller = Amazon_HeadTrackingManager_createPoller(nativeHeadTrackingManager);
        } else {
            poller = Amazon_HeadTrackingManager_createPollerWithConfig(
                         nativeHeadTrackingManager, config.nativeHeadTrackingConfiguration);
        }

        if (poller == IntPtr.Zero) {
            Debug.LogWarning("Unable to obtain native HeadTrackingPoller.");
            return null;
        }

        return new HeadTrackingPoller(poller);
#else
        return null;
#endif

    }

    /// <summary>
    /// Release the provided HeadTrackingPoller. Once released, the poller cannot be used to sample for new data.
    /// </summary>
    /// <param name='poller'>
    /// The poller to release.
    /// </param>
    public void ReleasePoller(HeadTrackingPoller poller) {

#if UNITY_ANDROID && !UNITY_EDITOR
        if (nativeHeadTrackingManager == IntPtr.Zero) {
            Debug.LogWarning("The native HeadTrackingManager is not initialized. Unable to release poller.");
            return;
        } else if (poller == null) {
            Debug.LogWarning("The provided poller is not initialized. Unable to release poller.");
            return;
        }

        Amazon_HeadTrackingManager_releasePoller(nativeHeadTrackingManager, poller.nativeHeadTrackingPoller);
#endif

    }

    /// <summary>
    /// Toggles whether head tracking is paused. When this method is called with true, all headtracking events are
    /// paused and polling will return false. Headtracking does not resume until this method is called again with false.
    /// </summary>
    /// <param name='standby'>
    /// True to disable polling, false to re-enable polling.
    /// </param>
    public void RequestStandby(bool standby) {

#if UNITY_ANDROID && !UNITY_EDITOR
        if (nativeHeadTrackingManager == IntPtr.Zero) {
            Debug.LogWarning("The native HeadTrackingManager is not initialized. Cancelling standby request.");
            return;
        }

        Amazon_HeadTrackingManager_requestStandby(nativeHeadTrackingManager, standby);
#endif

    }

    /// <summary>
    /// Helper API to determine whether or not the headtracking APIs are supported
    /// and available on the device.
    /// </summary>
    /// <returns>
    /// True if the headtracking APIs are supported and available, otherwise false.
    /// </returns>
    public static bool IsAvailable() {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject pm = activity.Call<AndroidJavaObject>("getPackageManager");
        return pm.Call<bool>("hasSystemFeature", FEATURE_HEAD_TRACKING);
#else
        return false;
#endif
    }

}

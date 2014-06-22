/**
 * HeadTrackingPoller.cs
 *
 * Copyright (c) 2014 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Use is subject to license terms.
 */

using UnityEngine;
using System;
using System.Runtime.InteropServices;

// This class enables a Unity application to poll for head tracking events.
public class HeadTrackingPoller {

    // Import C API from native head tracking library

    [DllImport("headtracking_client")]
    private static extern bool Amazon_HeadTrackingPoller_sample(IntPtr headTrackingPoller, IntPtr headTrackingEvent);

    [DllImport("headtracking_client")]
    private static extern bool Amazon_HeadTrackingPoller_updateConfiguration(IntPtr headTrackingPoller, IntPtr headTrackingConfig);

    // The pointer to the native head tracking poller
    public IntPtr nativeHeadTrackingPoller;

    /// <summary>
    /// Polls for the latest head tracking event and conditionally rotates it based
    /// on the value of ROTATE_EVENT_TO_SCREEN_ORIENTATION. By default, all events are
    /// received relative to ScreenOrientation.Portrait.
    /// </summary>
    /// <param name='event'>
    /// The event to populate - should not be null.
    /// </param>
    /// <returns>
    /// True if sample was successful and valid data was received, otherwise false.
    //// </returns>
    public bool Sample(ref HeadTrackingEvent htEvent) {

        if (htEvent == null) {
            Debug.LogWarning("The provided HeadTrackingEvent is not initialized. Unable to sample.");
            return false;
        } else if(nativeHeadTrackingPoller == IntPtr.Zero) {
            Debug.LogWarning("The native HeadTrackingPoller is not initialized. Unable to sample.");
            return false;
        }

        // Obtain the latest HeadTrackingEvent data.
        bool isValid = Amazon_HeadTrackingPoller_sample(nativeHeadTrackingPoller, htEvent.nativeHeadTrackingEvent);

        if (!isValid) {
            // the sample failed
            return false;
        }

        // Update the data associated with the provided HeadTrackingEvent
        htEvent.UpdateHeadTrackingData();
        return true;

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeadTrackingPoller"/> class
    /// to use for sampling for HeadTrackingEvent data.
    /// </summary>
    /// <param name='poller'>
    /// Pointer to the native HeadTrackingPoller.
    /// </param>
    public HeadTrackingPoller(IntPtr poller) {
        nativeHeadTrackingPoller = poller;
    }

    /// <summary>
    /// Updates the configuration associated with this poller.
    /// </summary>
    /// <param name='config'>
    /// The new configuration to associate with this poller.
    /// </param>
    public void UpdateConfiguration(HeadTrackingConfiguration config) {

        if(config == null) {
            Debug.LogWarning("Unable to update HeadTrackingConfiguration.");
        } else {
            Amazon_HeadTrackingPoller_updateConfiguration(nativeHeadTrackingPoller, config.nativeHeadTrackingConfiguration);
        }
    }

}

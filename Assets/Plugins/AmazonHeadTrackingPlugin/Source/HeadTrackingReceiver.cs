/**
 * HeadTrackingReceiver.cs
 *
 * Copyright (c) 2014 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Use is subject to license terms.
 */

using UnityEngine;

// Default component used to poll for latest head tracking event data.
// This component will use the standard HeadTrackingConfiguration by default.
//
// The lastEvent field will be updated to reflect the last received
// HeadTrackingEvent.
public class HeadTrackingReceiver : MonoBehaviour {

    // The lase received head tracking event.
    public static HeadTrackingEvent lastEvent;

    // Status of head tracking on device, updated in Start()
    public static bool isAvailable = false;

    // Instance of the head tracking manager.
    private HeadTrackingManager headTrackingManager;

    // Instance of the head tracking poller.
    private HeadTrackingPoller poller;

    // Initialization
    void Start () {

        // Determine if head tracking is supported on device
        isAvailable = HeadTrackingManager.IsAvailable();

        if (isAvailable) {
            // Retrieve an instance of the head tracking manager.
            headTrackingManager = HeadTrackingManager.CreateInstance();

            // Retrieve an instance of a head tracking event.
            lastEvent = HeadTrackingEvent.CreateInstance();

            // Create new poller with the default configuration
            if(headTrackingManager != null) {
                poller = headTrackingManager.CreatePoller();
            }
        }
    }

    void OnApplicationQuit () {

        if (isAvailable && (headTrackingManager != null) && (poller != null)) {
            // Release the poller.
            headTrackingManager.ReleasePoller(poller);
        }

    }

    // Poll for the latest head position data.
    void Update () {
        if (isAvailable && poller != null && lastEvent != null) {
            if(!poller.Sample(ref lastEvent)) {
                Debug.LogWarning("Head Tracking sample failed");
            }
        }
    }
}

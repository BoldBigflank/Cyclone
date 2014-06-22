/**
 * GestureEvent.cs
 *
 * Copyright (c) 2014 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Use is subject to license terms.
 */

using AmazonCommon;
using System;
using System.Collections;
using UnityEngine;

// Event used to represent a gesture that has been detected.
public class GestureEvent {

    // Gesture ID values
    public enum GestureId {
        // Triggered with a rocking motion of an edge of the device away and back (Action: DEFAULT)
        TILT = 0x0,
        // Triggered when the device is held at a small off-center angle (Action: ON) and then again
        // when the device returns back to center (Action: OFF)
        PEEK = 0x1,
        // Similar to PEEK, but also provides updates as the amount peeked changes (Action: UPDATE). The magnitude field
        // of the event corresponds to the percentage peeked between 0.0f (Action: OFF) and 1.0f (Action: ON) inclusive.
        // Note: PEEK is a subset of CONTINUOUS_PEEK, so there is no need to register for both events.
        CONTINUOUS_PEEK = 0x10
    }

    // Gesture direction values
    [Flags]
    public enum Direction {
        NONE    = 0x0,
        LEFT    = 0x1,
        RIGHT   = 0x2,
        BACK    = 0x4,
        FORWARD = 0x8
    }

    // Gesture action values
    [Flags]
    public enum Action {
        // Indicates a stateless gesture, like TILT, has been detected.
        DEFAULT = 0x0,
        // Indicates a stateful gesture, like PEEK and CONTINUOUS_PEEK, is in the on state.
        ON      = 0x1,
        // Indicates a stateful gesture, like PEEK and CONTINUOUS_PEEK, is in the off state.
        OFF     = 0x2,
        // Indicates that a continous gesture, like CONTINUOUS_PEEK, is in an intermediate state.
        UPDATE  = 0x4
    }

    // Gesture ID value
    public GestureEvent.GestureId gestureId;

    // Gesture direction value relative to orientation
    public GestureEvent.Direction direction;

    // Gesture action value
    //   TILT Actions: DEFAULT
    //   PEEK Actions: ON, OFF
    public GestureEvent.Action action;

    // The magnitude of the gesture event
    //   Stateful gestures, like PEEK, will report 0.0f when OFF and 1.0f when ON
    //   Stateless gestures, like TILT, will report 1.0f when DEFAULT
    //   Continuous gestures, like CONTINUOUS_PEEK,  will report a magnitude
    //       corresponding to the percentage beween PEEK ON and PEEK OFF
    public float magnitude;

    // The relative orientation associated with this event.
    public ScreenOrientation orientation;

    // The time of event occurrence in nanoseconds.
    public long timestamp;

    /// <summary>
    /// Initializes a new instance of the <see cref="GestureEvent"/> class with
    /// the provided data.
    /// </summary>
    /// <param name='id'>
    /// The gesture id value.
    /// </param>
    /// <param name='direction'>
    /// The gesture direction value.
    /// </param>
    /// <param name='action'>
    /// The gesture action value.
    /// </param>
    /// <param name='orientation'>
    /// The relative orientation value.
    /// </param>
    /// <param name='magnitude'>
    /// The magnitude value.
    /// </param>
    /// <param name='timestamp'>
    /// The time of event occurrence in nanoseconds.
    /// </param>
    private GestureEvent(GestureEvent.GestureId id, GestureEvent.Direction direction,
                         GestureEvent.Action action, ScreenOrientation orientation,
                         float magnitude, long timestamp) {
        this.gestureId   = id;
        this.direction   = direction;
        this.action      = action;
        this.orientation = orientation;
        this.magnitude   = magnitude;
        this.timestamp   = timestamp;
    }

    /// <summary>
    /// Converts a JSON representation of the event received by the Java API
    /// into a GestureEvent object to be used within Unity.
    /// </summary>
    /// <returns>
    /// The gesture event or null if the event cannot be constructed properly.
    /// </returns>
    /// <param name='eventJSON'>
    /// The JSON representation of the gesture event.
    /// </param>
    public static GestureEvent GetGestureEventFromJSON(string eventJSON) {
        Hashtable ht = eventJSON.hashtableFromJson();

        if (ht == null)
            return null;

        return new GestureEvent((GestureEvent.GestureId)Convert.ToInt32(ht["gestureId"]),
                                (GestureEvent.Direction)Convert.ToInt32(ht["direction"]),
                                (GestureEvent.Action)Convert.ToInt32(ht["action"]),
                                (ScreenOrientation)Convert.ToInt32(ht["orientation"]),
                                Convert.ToSingle(ht["magnitude"]),
                                Convert.ToInt64(ht["timestamp_nsecs"]));

    }

}

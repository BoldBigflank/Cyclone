/**
 * GestureListener.cs
 *
 * Copyright (c) 2014 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Use is subject to license terms.
 */

// Interface to implement for receiving callbacks for gesture events.
// Any game objects that register for gesture events must implement this interface.
// See GestureReceiver.cs for an example implementation.
interface GestureListener {
    /// <summary>
    /// Callback for received gesture events
    /// </summary>
    /// <param name='eventJSON'>
    /// The JSON representation of the gesture event.
    /// </param>
    void OnGestureEvent(string eventJSON);
}

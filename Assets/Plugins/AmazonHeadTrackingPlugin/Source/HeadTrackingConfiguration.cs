/**
 * HeadTrackingConfiguration.cs
 *
 * Copyright (c) 2014 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Use is subject to license terms.
 */

using UnityEngine;
using System;
using System.Runtime.InteropServices;

// Class used to represent the configuration required to create a
// HeadTrackingPoller to poll for head tracking events.
public class HeadTrackingConfiguration {

#if UNITY_ANDROID && !UNITY_EDITOR

    // Import C API from native head tracking library
    [DllImport("headtracking_client")]
    private static extern IntPtr Amazon_HeadTrackingConfiguration_createInstance();

    [DllImport("headtracking_client")]
    private static extern void Amazon_HeadTrackingConfiguration_setFidelity(IntPtr headTrackingConfiguration, int fidelity);

    [DllImport("headtracking_client")]
    private static extern void Amazon_HeadTrackingConfiguration_setFilterType(IntPtr headTrackingConfiguration, int filterType);

#endif

    // Represents the head tracking filter that is applied to events.
    public enum FilterType {
        HT_ONLY          = 0,
        HT_SENSOR_FUSION = 1
    }

    // Represents the desired quality of the head tracking information.
    public enum Fidelity {
        LOW_POWER = 1,
        NORMAL    = 2,
        HIGH      = 3
    }

    // The pointer to the native head tracking configuration.
    public IntPtr nativeHeadTrackingConfiguration;

    // Value of the Filter for this HeadTrackingConfiguration
    private HeadTrackingConfiguration.FilterType filterType = HeadTrackingConfiguration.FilterType.HT_SENSOR_FUSION;

    // Value of the Fidelity for this HeadTrackingConfiguration
    private HeadTrackingConfiguration.Fidelity fidelity = HeadTrackingConfiguration.Fidelity.NORMAL;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeadTrackingConfiguration"/> class with
    /// the provided pointer to the corresponding native head tracking configuration.
    /// </summary>
    /// <param name='nativeConfig'>
    /// The pointer to the native HeadTrackingConfiguration.
    /// </param>
    private HeadTrackingConfiguration(IntPtr nativeConfig) {
        nativeHeadTrackingConfiguration = nativeConfig;
    }

    /// <summary>
    /// Set the fidelity for this configuration.
    /// </summary>
    /// <param name='fidelity'>
    /// The fidelity value.
    /// </param>
    public void SetFidelity(HeadTrackingConfiguration.Fidelity fidelity) {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (nativeHeadTrackingConfiguration != IntPtr.Zero) {
            Amazon_HeadTrackingConfiguration_setFidelity(nativeHeadTrackingConfiguration, (int)fidelity);
            this.fidelity = fidelity;
        }
#endif
    }

    /// <summary>
    /// Set the filter type for this configuration.
    /// </summary>
    /// <param name='filterType'>
    /// The filter type value.
    /// </param>
    public void SetFilterType(HeadTrackingConfiguration.FilterType filterType) {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (nativeHeadTrackingConfiguration != IntPtr.Zero) {
            Amazon_HeadTrackingConfiguration_setFilterType(nativeHeadTrackingConfiguration, (int)filterType);
            this.filterType = filterType;
        }
#endif
    }

    /// <summary>
    /// Get the fidelity for this configuration.
    /// </summary>
    public HeadTrackingConfiguration.Fidelity GetFidelity() {
        return fidelity;
    }

    /// <summary>
    /// Get the filter type for this configuration.
    /// </summary>
    public HeadTrackingConfiguration.FilterType GetFilterType() {
        return filterType;
    }

    /// <summary>
    /// Creates an instance of a HeadTrackingConfiguration with the default settings.
    /// </summary>
    /// <returns>
    /// Instance of a HeadTrackingConfiguration or null if it cannot be
    /// initialized properly.
    /// </returns>
    public static HeadTrackingConfiguration CreateInstance() {
#if UNITY_ANDROID && !UNITY_EDITOR
        IntPtr nativeConfig = Amazon_HeadTrackingConfiguration_createInstance();

        if(nativeConfig == IntPtr.Zero) {
            Debug.LogError("Could not create native HeadTrackingConfiguration");
            return null;
        }

        return new HeadTrackingConfiguration(nativeConfig);
#else
        return null;
#endif
    }

}

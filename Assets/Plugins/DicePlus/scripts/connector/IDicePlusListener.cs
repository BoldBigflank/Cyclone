using UnityEngine;
using System.Collections.Generic;
using System;
/// <summary>
/// Listener to die related events
/// </summary>
public interface IDicePlusListener
{
	/// <summary>
	/// Called when accelerometer readout arrives from the die
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='time'>
	/// timestamp
	/// </param>
	/// <param name='v'>
	/// readout from the die
	/// </param>
	/// <param name='type'>
	/// readout type
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onAccelerometerReadout(DicePlus dicePlus, long time, Vector3 v, int type, string errorMsg);
	/// <summary>
	/// Called when the die detected tap event
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='time'>
	/// timestamp
	/// </param>
	/// <param name='v'>
	/// readout from the die
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onTapReadout(DicePlus dicePlus, long time, Vector3 v, string errorMsg);
	/// <summary>
	/// Called when orientation readout arrives from the die
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='time'>
	/// timestamp
	/// </param>
	/// <param name='v'>
	/// readout from a die
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onOrientationReadout(DicePlus dicePlus, long time, Vector3 v, string errorMsg);
	/// <summary>
	/// Called when thermometer readout arrives from the die
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='time'>
	/// timestamp
	/// </param>
	/// <param name='temperature'>
	/// readout from a die
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onTemperatureReadout(DicePlus dicePlus, long time, float temperature, string errorMsg);
	/// <summary>
	/// Informs about the state of the charging process
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='state'>
	/// battery charging state
	/// </param>
	/// <param name='low'>
	/// indicates that battery level is low
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onBatteryState(DicePlus dicePlus, DicePlusConnector.BatteryState state, int percentage, bool low, string errorMsg);
	/// <summary>
	/// Called when capacitive sensor sent an touch/release readout
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='time'>
	/// timestamp
	/// </param>
	/// <param name='current'>
	/// bit mask of currently touched die's faces
	/// </param>
	/// <param name='change'>
	/// bit mask of changes from previous state
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onTouchReadout(DicePlus dicePlus, long time, int current, int change, string errorMsg);
	/// <summary>
	/// Called when a capacitive readout arrives from the die
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='time'>
	/// timestamp
	/// </param>
	/// <param name='readouts'>
	/// readout from the die
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onProximityReadout(DicePlus dicePlus, long time, List<int> readouts, string errorMsg);
	/// <summary>
	/// Called as a result of LED animation request
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onRunLedAnimationStatus(DicePlus dicePlus, string errorMsg);
	/// <summary>
	/// Called when user throws the die, null if roll was simulated by widget
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='time'>
	/// timestamp
	/// </param>
	/// <param name='duration'>
	/// Roll duration, expressed in ms
	/// </param>
	/// <param name='face'>
	/// Roll result - which face is up.
	/// </param>
	/// <param name='invalidityFlags'>
	/// Flags indicating invalid rolls. If flags is 0, then roll is valid.
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onRoll(DicePlus dicePlus, long time, int duration, int face, int invalidityFlags, string errorMsg);
	/// <summary>
	/// Called when up face changed
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='time'>
	/// timestamp
	/// </param>
	/// <param name='face'>
	/// which face is up, 
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onFaceReadout(DicePlus dicePlus, long time, int face, string errorMsg);
	/// <summary>
	/// Called when magnetometer readout arrives from the die
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='time'>
	/// timestamp
	/// </param>
	/// <param name='v'>
	/// readout from a die
	/// </param>
	/// <param name='type'>
	/// readout type
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onMagnetometerReadout(DicePlus dicePlus, long time, Vector3 v, int type, string errorMsg);
	/// <summary>
	/// Callback run when LED animation state changed on at least one of dies face
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='time'>
	/// timestamp
	/// </param>
	/// <param name='ledMask'>
	/// bit mask indicating which dice faces have led animation currently running
	/// </param>
	/// <param name='animationId'>
	/// animation id
	/// </param>
	/// <param name='type'>
	/// bit mask indicating type of animation event
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onLedState(DicePlus dicePlus, long time, DicePlusConnector.LedFace ledMask, long animationId, int type, string errorMsg);
	/// <summary>
	/// Called when subscription state changed. Response to all subscribe[Sensor], unsubscribe[Sensor] calls.
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='dataSource'>
	/// Data source.
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onSubscriptionChangeStatus(DicePlus dicePlus, DicePlusConnector.DataSource dataSource, string errorMsg);
	/// <summary>
	/// Called as a result of setMode request
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onSetModeStatus(DicePlus dicePlus, string errorMsg);
	/// <summary>
	/// Called as a result of sleep request.
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onSleepStatus(DicePlus dicePlus, string errorMsg);
	/// <summary>
	/// Called as a result of die statistics query
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='diceStatistics'>
	/// statistics data object
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onDiceStatistics(DicePlus dicePlus, DiceStatistics diceStatistics, string errorMsg);
	
	/// <summary>
	/// Called as a result of power mode query
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='time'>
	/// timestamp
	/// </param>
	/// <param name='mode'>
	/// readout from a die
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onPowerMode(DicePlus dicePlus, long time, DicePlusConnector.PowerMode mode, string errorMsg);
	/// <summary>
	/// Called upon successful reset of all persistent storage record values to factory defaults
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onPStorageReset(DicePlus dicePlus, string errorMsg);
	/// <summary>
	/// Called as a result of value query on string type persistent storage record
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='handle'>
	/// the handle of record
	/// </param>
	/// <param name='str'>
	/// value read from record
	/// </param>
	void onPStorageValueRead(DicePlus dicePlus, int handle, String str);
	/// <summary>
	/// Called as a result of value query on vector type persistent storage record
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='handle'>
	/// the handle of record
	/// </param>
	/// <param name='vector'>
	/// value read from record
	/// </param>
	void onPStorageValueRead(DicePlus dicePlus, int handle, Vector3 vector);
	/// <summary>
	/// Called as a result of value query on vector type persistent storage record
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='handle'>
	/// the handle of record
	/// </param>
	/// <param name='intvalue'>
	/// value read from record
	/// </param>
	void onPStorageValueRead(DicePlus dicePlus, int handle, int intvalue);
	/// <summary>
	/// Called upon successful persistent storage communication initialization
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='count'>
	/// persistent storage record count
	/// </param>
	void onPStorageCommunicationInitialized(DicePlus dicePlus, int count);
	/// <summary>
	/// Called upon error which occurred during persistent storage operation
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='errorMsg'>
	/// error message, null if there was no error
	/// </param>
	void onPStorageOperationFailed(DicePlus dicePlus, string errorMsg);
	/// <summary>
	/// Called when persistent storage record value was successfully set
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='handle'>
	/// the handle to record
	/// </param>
	void onPStorageValueWrite(DicePlus dicePlus, int handle);
}


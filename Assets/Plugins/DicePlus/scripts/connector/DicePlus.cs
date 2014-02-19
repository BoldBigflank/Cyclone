using UnityEngine;
using System;
using System.Collections.Generic;
/// <summary>
/// Relates to a physical die device. Provides interface to interact with die. Dispatches events to listeners.
/// </summary>
public class DicePlus
{	
	private HashSet<IDicePlusListener> listeners = new HashSet<IDicePlusListener>();
	DicePlusConnector dpc;
	/// <summary>
	/// DicePlus bluetooth address - unique identifier
	/// </summary>
	public string address;
	/// <summary>
	/// RSSI value at the moment when die was discovered
	/// </summary>
	public int rssi;
	int pstorageRecordCount = 0;
	
	public DicePlus(DicePlusConnector dpc, string address) {
		this.dpc = dpc;
		this.address = address;
	}
	/// <summary>
	/// Registers <see cref="IDicePlusListener"/> to be informed about dice related events
	/// </summary>
	/// <param name='listener'>
	/// registered listener
	/// </param>	
	public void registerListener(IDicePlusListener listener) {
		HashSet<IDicePlusListener> tmplisteners = new HashSet<IDicePlusListener>(listeners);
		tmplisteners.Add(listener);
		listeners = tmplisteners;
	}
	/// <summary>
	/// Unregisters <see cref="IDicePlusListener"/>
	/// </summary>
	/// <param name='listener'>
	/// unregistered listener
	/// </param>
	public void unregisterListener(IDicePlusListener listener) {
		HashSet<IDicePlusListener> tmplisteners = new HashSet<IDicePlusListener>(listeners);
		tmplisteners.Remove(listener);
		listeners = tmplisteners;
	}
	
	public void onAccelerometerReadout(long time, Vector3 v, int type, string errorMsg) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onAccelerometerReadout(this, time, v, type, errorMsg);
		}
	}

	public void onTapReadout(long time, Vector3 v, string errorMsg) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onTapReadout(this, time, v, errorMsg);
		}
	}

	public void onBatteryState(DicePlusConnector.BatteryState status, int percentage, bool low, string errorMsg) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onBatteryState(this, status, percentage, low, errorMsg);
		}
	}

	public void onTemperatureReadout(long time, float temperature, string errorMsg) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onTemperatureReadout(this, time, temperature, errorMsg);
		}
	}	

	public void onTouchReadout(long time, int current, int change, string errorMsg) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onTouchReadout(this, time, current, change, errorMsg);
		}
	}
	

	public void onProximityReadout(long time, List<int> readouts, string errorMsg) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onProximityReadout(this, time, readouts, errorMsg);
		}
	}
	
	public void onRunLedAnimationStatus(string message) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onRunLedAnimationStatus(this, message);
		}
	}
	
	public void onRoll(long time, int duration, int face, int invalidityFlags, string errorMsg) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onRoll(this, time, duration, face, invalidityFlags, errorMsg);
		}
	}
	
	public void onMagnetometerReadout(long time, Vector3 v, int type, string errorMsg) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onMagnetometerReadout(this, time, v, type, errorMsg);
		}
	}

	public void onOrientationReadout(long time, Vector3 v, string errorMsg) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onOrientationReadout(this, time, v, errorMsg);
		}
	}
	
	public void onLedState(long time, DicePlusConnector.LedFace ledMask, long animationId, int type, string errorMsg) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onLedState(this, time, ledMask, animationId, type, errorMsg);
		}
	}
	
	public void onSubscriptionChangeStatus(DicePlusConnector.DataSource dataSource, string message) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onSubscriptionChangeStatus(this, dataSource, message);
		}
	}
	
	public void onSetModeStatus(string message) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onSetModeStatus(this, message);
		}
	}
	
	public void onSleepStatus(string message) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onSleepStatus(this, message);
		}
	}
	
	public void onDiceStatistics(DiceStatistics diceStatistics, string errorMsg) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onDiceStatistics(this, diceStatistics, errorMsg);
		}
	}

	public void onFaceReadout(long time, int face, string errorMsg) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onFaceReadout(this, time, face, errorMsg);
		}
	}
	
	public void onPowerMode(long time, DicePlusConnector.PowerMode mode, string errorMsg) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onPowerMode(this, time, mode, errorMsg);
		}
	}

	public void onPStorageReset(string errorMessage) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onPStorageReset(this, errorMessage);
		}
	}

	public void onPStorageValueRead(int handle, String str) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onPStorageValueRead(this, handle, str);
		}
	}

	public void onPStorageValueRead(int handle, Vector3 vector) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onPStorageValueRead(this, handle, vector);
		}
	}

	public void onPStorageValueRead(int handle, int intvalue) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onPStorageValueRead(this, handle, intvalue);
		}
	}
	/// <summary>
	/// Gets persistent storage record count
	/// </summary>
	/// <returns>
	/// persistent storage record count
	/// </returns>
	public int getPStorageRecordCount() {
		return pstorageRecordCount;
	}
	
	public void onPStorageCommunicationInitialized(int count) {
		pstorageRecordCount = count;
		foreach (IDicePlusListener listener in listeners) {
			listener.onPStorageCommunicationInitialized(this, count);
		}
	}

	public void onPStorageOperationFailed(string errorMessage) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onPStorageOperationFailed(this, errorMessage);
		}
	}

	public void onPStorageValueWrite(int handle) {
		foreach (IDicePlusListener listener in listeners) {
			listener.onPStorageValueWrite(this, handle);
		}
	}
	
	/// <summary>
	/// Sends request to the given dice to reset all persistent storage settings
	/// </summary>
	public void	resetPStorage(){
		dpc.resetPStorage(this);
	} 
	/// <summary>
	/// Sends request to the given dice to initialize persistent storage communication
	/// </summary>
	public void	initializePStorageCommunication(){
		dpc.initializePStorageCommunication(this);
	} 
	/// <summary>
	/// Sends request to the given dice to get persistent storage record value
	/// </summary>
	/// <param name='handle'>
	/// the handle to requested record
	/// </param>
	public void	readPStorageValue(int handle){
		dpc.readPStorageValue(this, handle);
	} 
	/// <summary>
	/// Sends request to the given dice to set persistent storage record value
	/// </summary>
	/// <param name='handle'>
	/// the handle to requested record
	/// </param>
	/// <param name='intvalue'>
	/// integer value to be set
	/// </param>
	public void	writePStorageValue(int handle, int intvalue){
		dpc.writePStorageValue(this, handle, intvalue);
	} 
	/// <summary>
	/// Sends request to the given dice to set persistent storage record value
	/// </summary>
	/// <param name='handle'>
	/// the handle to requested record
	/// </param>
	/// <param name='x'>
	/// first vector value to be set
	/// </param>
	/// <param name='y'>
	/// second vector value to be set
	/// </param>
	/// <param name='z'>
	/// third vector value to be set
	/// </param>
	public void	writePStorageValue(int handle, int x, int y, int z){
		dpc.writePStorageValue(this, handle, x, y, z);
	} 
	/// <summary>
	/// Sends request to the given dice to set persistent storage record value
	/// </summary>
	/// <param name='handle'>
	/// the handle to requested record
	/// </param>
	/// <param name='str'>
	/// string value to be set
	/// </param>
	public void	writePStorageValue(int handle, String str){
		dpc.writePStorageValue(this, handle, str);
	} 
	/// <summary>
	/// Gets persistent storage record description for given handle
	/// </summary>
	/// <returns>
	/// persistent storage record description, null if handle was invalid
	/// </returns>
	/// <param name='handle'>
	/// the handle to requested record
	/// </param>
	public PStorageRecordDescription getPStorageRecordDescription(int handle) {
		return dpc.getPStorageRecordDescription(this, handle);
	}
	/// <summary>
	/// Subscribes to accelerometer readouts with default values. 
	/// Frequency - 10 Hz, readout filter type - AccelerometerReadoutType.READOUT_RAW
	/// </summary>
	public void	subscribeAccelerometerReadouts(){
		dpc.subscribeAccelerometerReadouts(this);
	} 		
	/// <summary>
	/// Subscribes to accelerometer readouts and configure sensor
	/// </summary>
	/// <param name='readFrequency'>
	/// readout frequency (in Hz), values: 1 - 60
	/// </param>
	/// <param name='readoutType'>
	/// a readout filter type
	/// </param>
	public void	subscribeAccelerometerReadouts(int readFrequency, DicePlusConnector.AccelerometerReadoutType readoutType){
		dpc.subscribeAccelerometerReadouts(this, readFrequency, readoutType);
	}
	/// <summary>
	/// Unsubscribes from accelerometer readouts
	/// </summary>
	public void	unsubscribeAccelerometerReadouts(){
		dpc.unsubscribeAccelerometerReadouts(this);
	} 
	/// <summary>
	/// Subscribes to periodic battery state events. Sends current battery state query
	/// </summary>
	public void	subscribeBatteryState(){
		dpc.subscribeBatteryState(this);
	} 
	/// <summary>
	/// Unsubscribes from battery state events
	/// </summary>
	public void	unsubscribeBatteryState(){
		dpc.unsubscribeBatteryState(this);
	} 
	/// <summary>
	/// Subscribes to touch/release state events. Sends current touch/release state query.
	/// </summary>
	public void	subscribeTouchReadouts(){
		dpc.subscribeTouchReadouts(this);
	} 
	/// <summary>
	/// Unsubscribes from touch/release state events
	/// </summary>
	public void	unsubscribeTouchReadouts(){
		dpc.unsubscribeTouchReadouts(this);
	} 
	/// <summary>
	/// Subscribes to proximity readouts
	/// </summary>
	public void	subscribeProximityReadouts(){
		dpc.subscribeProximityReadouts(this);
	} 
	/// <summary>
	/// Subscribes to proximity readouts and configures sensor
	/// </summary>
	/// <param name='freq'>
	/// readout frequency (in Hz), values: 1 - 10
	/// </param>
	public void	subscribeProximityReadouts(int freq){
		dpc.subscribeProximityReadouts(this, freq);
	} 
	/// <summary>
	/// Unsubscribes from proximity sensor readouts
	/// </summary>
	public void	unsubscribeProximityReadouts(){
		dpc.unsubscribeProximityReadouts(this);
	} 
	/// <summary>
	/// Subscribes to LED state events - run on LED animation start and finish
	/// </summary>
	public void	subscribeLedState(){
		dpc.subscribeLedState(this);
	} 
	/// <summary>
	/// Unsubscribes from LED state events
	/// </summary>
	public void	unsubscribeLedState(){
		dpc.unsubscribeLedState(this);
	} 
	/// <summary>
	/// Subscribes to magnetometer readouts with default values. 
	/// Frequency - 10 Hz, readout filter type - MagnetometerReadoutType.READOUT_RAW
	/// </summary>
	public void	subscribeMagnetometerReadouts(){
		dpc.subscribeMagnetometerReadouts(this);
	} 	
	/// <summary>
	/// Subscribes to magnetometer readouts and configures sensor.
	/// </summary>
	/// <param name='freq'>
	/// readout frequency (in Hz), values: 1 - 30
	/// </param>
	/// <param name='type'>
	/// a readout type
	/// </param>
	public void	subscribeMagnetometerReadouts(int freq,  DicePlusConnector.MagnetometerReadoutType type){
		dpc.subscribeMagnetometerReadouts(this, freq, type);
	} 
	/// <summary>
	/// Unsubscribes from magnetometer readouts.
	/// </summary>
	public void	unsubscribeMagnetometerReadouts(){
		dpc.unsubscribeMagnetometerReadouts(this);
	}
	/// <summary>
	/// Subscribes to temperature readouts.
	/// </summary>
	public void	subscribeTemperatureReadouts(){
		dpc.subscribeTemperatureReadouts(this);
	} 
	/// <summary>
	/// Unsubscribes from temperature readouts.
	/// </summary>
	public void	unsubscribeTemperatureReadouts(){
		dpc.unsubscribeTemperatureReadouts(this);
	} 
	/// <summary>
	/// Subscribes to tap readouts.
	/// </summary>
	public void	subscribeTapReadouts(){
		dpc.subscribeTapReadouts(this);
	} 
	/// <summary>
	/// Unsubscribes from tap readouts.
	/// </summary>
	public void	unsubscribeTapReadouts(){
		dpc.unsubscribeTapReadouts(this);
	} 
	/// <summary>
	/// Subscribes to face readouts.
	/// </summary>
	public void	subscribeFaceReadouts(){
		dpc.subscribeFaceReadouts(this);
	} 
	/// <summary>
	/// Unsubscribes from face readouts.
	/// </summary>
	public void	unsubscribeFaceReadouts(){
		dpc.unsubscribeFaceReadouts(this);
	} 
	/// <summary>
	/// Subscribes to orientation readouts
	/// </summary>
	public void	subscribeOrientationReadouts(){
		dpc.subscribeOrientationReadouts(this);
	} 
	/// <summary>
	/// Subscribes to orientation readouts and configures sensor.
	/// </summary>
	/// <param name='freq'>
	/// readout frequency (in Hz), values: 1 - 20
	/// </param>
	public void	subscribeOrientationReadouts(int freq){
		dpc.subscribeOrientationReadouts(this, freq);
	} 
	/// <summary>
	/// Unsubscribes from orientation readouts
	/// </summary>
	public void	unsubscribeOrientationReadouts(){
		dpc.unsubscribeOrientationReadouts(this);
	} 
	/// <summary>
	/// Subscribes from PowerMode changes. Sends current PowerMode query.
	/// </summary>
	public void	subscribePowerMode(){
		dpc.subscribePowerMode(this);
	} 
	/// <summary>
	/// Unsubscribes from PowerMode changes
	/// </summary>
	public void	unsubscribePowerMode(){
		dpc.unsubscribePowerMode(this);
	} 
	/// <summary>
	/// Subscribes to roll events
	/// </summary>
	public void	subscribeRolls(){
		dpc.subscribeRolls(this);
	} 	
	/// <summary>
	/// Unsubscribes from roll events
	/// </summary>
	public void	unsubscribeRolls(){
		dpc.unsubscribeRolls(this);
	}
	
	/// <summary>
	/// Disconnects specified dice.
	/// </summary>
	public void	disconnect(){
		dpc.disconnect(this);
	} 
	
	/// <summary>
	/// Sends setMode packet to the given die
	/// </summary>
	/// <param name='mode'>
	/// mode to set
	/// </param>
	public void	setMode(DicePlusConnector.DieMode mode) {
		dpc.setMode(this, mode);
	}
	
	/// <summary>
	/// Sends sleep request to the given dice
	/// </summary>
	public void	sleep() {
		dpc.sleep(this);
	}
	
	/// <summary>
	/// Sends the die statistics request to the given dice.
	/// </summary>
	public void	getDiceStatistics() {
		dpc.getDiceStatistics(this);
	}
	
	/// <summary>
	/// Gets model number.
	/// </summary>
	/// <returns>
	/// device model number, null when die has not yet connected and authenticated connection.
	/// </returns>
	public long? getModelNumber() {
		return dpc.getModelNumber(this);
	}  
	
	/// <summary>
	/// Gets software version.
	/// </summary>
	/// <returns>
	/// number representing software version, null when die has not yet connected and authenticated connection.
	/// </returns>
	public SoftwareVersion getSoftwareVersion() {
		return dpc.getSoftwareVersion(this);
	} 
	/// <summary>
	/// Gets hardware version.
	/// </summary>
	/// <returns>
	/// number representing hardware version, null when die has not yet connected and authenticated connection.
	/// </returns>
	public HardwareVersion getHardwareVersion() {
		return dpc.getHardwareVersion(this);
	} 

	/// <summary>
	/// Gets unique identifier
	/// </summary>
	/// <returns>
	/// dies unique identifier, null when die has not yet connected and authenticated connection.
	/// </returns>
	public string getUID() {
		return dpc.getUID(this);
	} 

	/// <summary>
	/// Gets dies system status
	/// </summary>
	/// <returns>
	/// dies status, null when die has not yet connected and authenticated connection.
	/// </returns>
	public int? getStatus() {
		return dpc.getStatus(this);
	} 
	/// <summary>
	/// Gets number of leds die has.
	/// </summary>
	/// <returns>
	/// number of leds, null when die has not yet connected and authenticated connection
	/// </returns>
	public int? getLedCount() {
		return dpc.getLedCount(this);
	} 
	
	/// <summary>
	/// Gets number of faces die has. 
	/// </summary>
	/// <returns>
	/// number of faces, null when die has not yet connected and authenticated connection.
	/// </returns>
	public int? getFaceCount() {
		return dpc.getFaceCount(this);
	} 
	
	/// <summary>
	/// Sends blink LED animation request
	/// </summary>
	/// <param name='ledMask'>
	/// which LEDs to run animation on
	/// </param>
	/// <param name='priority'>
	/// animation with lower value overrides other, values: 0 - 255
	/// </param>
	/// <param name='color'>
	/// color to be blinked
	/// </param>
	/// <param name='ledOnPeriod'>
	/// time in ms LED is ON, values: 0 - 65535
	/// </param>
	/// <param name='ledCyclePeriod'>
	/// time in ms one blink cycle lasts, values: 0 - 65535
	/// </param>
	/// <param name='blinkNumber'>
	/// number of blinks, values: 0 - 255
	/// </param>
	public void runBlinkAnimation(DicePlusConnector.LedFace ledMask, int priority, 
			Color color, int ledOnPeriod, int ledCyclePeriod, int blinkNumber) {
		dpc.runBlinkAnimation(this, ledMask, priority, color, ledOnPeriod, ledCyclePeriod, blinkNumber);
	} 
	/// <summary>
	/// Sends fade LED animation request
	/// </summary>
	/// <param name='ledMask'>
	/// which LEDs to run animation on
	/// </param>
	/// <param name='priority'>
	/// animation with lower value overrides other, values: 0 - 255
	/// </param>
	/// <param name='color'>
	/// color to be faded
	/// </param>
	/// <param name='fadeInOutTime'>
	/// time in ms LED is fading in/out, values: 0 - 65535
	/// </param>
	/// <param name='pauseTime'>
	/// time in ms LED stays at full bright before fading out, values: 0 - 65535
	/// </param>
	public void runFadeAnimation(DicePlusConnector.LedFace ledMask, int priority, 
			Color color, int fadeInOutTime, int pauseTime) { 
		dpc.runFadeAnimation(this, ledMask, priority, color, fadeInOutTime, pauseTime);
	} 
	/// <summary>
	/// Sends standard LED animation request
	/// </summary>
	/// <param name='ledMask'>
	/// which LEDs to run animation on
	/// </param>
	/// <param name='priority'>
	/// animation with lower value overrides other, values: 0 - 255
	/// </param>
	/// <param name='animation'>
	/// standard animation type
	/// </param>
	public void runStandardAnimation(DicePlusConnector.LedFace ledMask, int priority, 
			DicePlusConnector.AnimationType animation) {	
		dpc.runStandardAnimation(this, ledMask, priority, animation);
	} 
	
	/// <summary>
	/// Checks if the die is connected and authenticated
	/// </summary>
	/// <returns>
	/// true, if the die is authenticated; false otherwise.
	/// </returns>
	public bool	hasConnectionEstabilished() {
		return dpc.hasConnectionEstabilished(this);
	} 
}

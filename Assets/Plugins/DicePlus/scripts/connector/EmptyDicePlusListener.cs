using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class EmptyDicePlusListener : MonoBehaviour, IDicePlusListener {		
		
	public virtual void onAccelerometerReadout(DicePlus dicePlus, long time, Vector3 v, int type, string errorMsg) {		 

	}
	public virtual void onTapReadout(DicePlus dicePlus, long time, Vector3 v, string errorMsg) {		 

	}
	public virtual void onOrientationReadout(DicePlus dicePlus, long time, Vector3 v, string errorMsg) {

	}
	public virtual void onTemperatureReadout(DicePlus dicePlus, long time, float temperature, string errorMsg) {

	}
	public virtual void onBatteryState(DicePlus dicePlus, DicePlusConnector.BatteryState state, int percentage, bool low, string errorMsg) {

	}
	
	public virtual void onTouchReadout(DicePlus dicePlus, long time, int current, int change, string errorMsg)  {

	}
	
	public virtual void onProximityReadout(DicePlus dicePlus, long time, List<int> readouts, string errorMsg)  {

	}
	public virtual void onRoll(DicePlus dicePlus, long time, int duration, int face, int invalidityFlags, string errorMsg)  {

	}
	public virtual void onFaceReadout(DicePlus dicePlus, long time, int face, string errorMsg)  {

	}
	public virtual void onMagnetometerReadout(DicePlus dicePlus, long time, Vector3 v, int type, string errorMsg)  {

	}
	public virtual void onRunLedAnimationStatus(DicePlus dicePlus, string errorMsg)  {

	}
	public virtual void onLedState(DicePlus dicePlus, long time, DicePlusConnector.LedFace ledMask, long animationId, int type, string errorMsg)  {

	}
	public virtual void onSetModeStatus(DicePlus dicePlus, string errorMsg)  {

	}
	public virtual void onSleepStatus(DicePlus dicePlus, string errorMsg)  {

	}
	public virtual void onDiceStatistics(DicePlus dicePlus, DiceStatistics diceStatistics, string errorMsg)   {

	}
	
	public virtual void onSubscriptionChangeStatus(DicePlus dicePlus, DicePlusConnector.DataSource dataSourceCode, string errorMsg)  {

	}
	public virtual void onReadoutFailed(DicePlus dicePlus, int dataSourceCode, string errorMsg)  {

	}
	public virtual void onPowerMode(DicePlus dicePlus, long time, DicePlusConnector.PowerMode mode, string errorMsg){

	}
	
	public virtual void onPStorageReset(DicePlus dicePlus, string errorMsg){		

	}
	public virtual void onPStorageValueRead(DicePlus dicePlus, int handle, String str){

	}
	public virtual void onPStorageValueRead(DicePlus dicePlus, int handle, Vector3 vector){

	}
	public virtual void onPStorageValueRead(DicePlus dicePlus, int handle, int intvalue){

	}
	public virtual void onPStorageCommunicationInitialized(DicePlus dicePlus, int count){		

	}
	public virtual void onPStorageOperationFailed(DicePlus dicePlus, string errorMsg){

	}
	public virtual void onPStorageValueWrite(DicePlus dicePlus, int handle){

	}

}

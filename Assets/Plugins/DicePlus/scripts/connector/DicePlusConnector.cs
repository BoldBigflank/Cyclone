using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Provides interface to find dice and interact with them. Dispatches events to dice and connection listeners.
/// </summary>
public class DicePlusConnector : MonoBehaviour { 
	
	public static int ERR_VERSION_MISSMATCH = 0x1;
	
	/// <summary>
	/// Data source type. 
	/// May correspond to real sensor, emulated sensor, die internal state or any other data source provided by die.
	/// </summary>
	public enum DataSource {
		/// <summary>
		/// Corresponds roll triggered events
		/// </summary>
		ROLL = 1,
		/// <summary>
		/// Corresponds periodic magnetometer data readouts
		/// </summary>
		MAGNETOMETER = 2,
		/// <summary>
		/// Corresponds periodic accelerometer data readouts
		/// </summary>
		ACCELEROMETER = 3,
		/// <summary>
		/// Corresponds periodic temperature readouts
		/// </summary>
		TEMPERATURE = 4,
		/// <summary>
		/// Corresponds events triggered on dies face touch
		/// </summary>
		TOUCH = 5,
		/// <summary>
		/// Corresponds periodic capacitive readouts
		/// </summary>
		CAPACITIVE = 6,
		/// <summary>
		/// Corresponds periodic orientation calculations
		/// </summary>
		ORIENTATION = 9,
		/// <summary>
		/// Corresponds led state changes
		/// </summary>
		LED_STATE = 10,
		/// <summary>
		/// Corresponds events triggered on dies power state changes
		/// </summary>
		POWER_MODE = 11,
		/// <summary>
		/// Corresponds battery charging level
		/// </summary>
		BATTERY_LEVEL = 13,
		/// <summary>
		/// Corresponds die roll statistics
		/// </summary>
		STATISTICS = 14,
		/// <summary>
		/// Corresponds battery charging level
		/// </summary>
		FACE = 15,
		/// <summary>
		/// Corresponds die roll statistics
		/// </summary>
		TAP = 16
	}
	
	/// <summary>
	/// Accelerometer readout type.
	/// </summary>
	[Flags]
	public enum AccelerometerReadoutType {
		/// <summary>
		/// Raw readout type
		/// </summary>
		RAW = 0x1,
		/// <summary>
		/// Low pass filter readout type
		/// </summary>
		LOW_PASS_FILTER = 0x2,
		/// <summary>
		/// High pass filter readout type
		/// </summary>
		HIGH_PASS_FILTER = 0x4
	}
	
	/// <summary>
	/// Magnetometer readout type.
	/// </summary>
	[Flags]
	public enum MagnetometerReadoutType {
		/// <summary>
		/// Raw readout type
		/// </summary>
		RAW = 0x1,
		/// <summary>
		/// Low pass filter readout type
		/// </summary>
		LOW_PASS_FILTER = 0x2
	}
	/// <summary>
	/// Power mode.
	/// </summary>
	public enum PowerMode {
		/// <summary>
		/// Die operates normally
		/// </summary>
		NORMAL = 0,
		/// <summary>
		/// Die is preparing to shutdown
		/// </summary>
		SHUTDOWN = 1,
		/// <summary>
		/// Die is about to sleep
		/// </summary>
		SLEEP = 2
	}
	/// <summary>
	/// Battery states
	/// </summary>
	public enum BatteryState {
		/// <summary>
		/// Battery is discharging
		/// </summary>
		DISCHARGING = 0,
		/// <summary>
		/// Battery is charging
		/// </summary>
		CHARGING = 1
	}
	/// <summary>
	/// Bluetooth state
	/// </summary>
	public enum BluetoothState {
		/// <summary>
		/// Bluetooth adapter is ready
		/// </summary>
		READY = 0,
		/// <summary>
		/// Bluetooth adapter is not ready
		/// </summary>
		NOT_READY = 1,
		/// <summary>
		/// Bluetooth is not supported
		/// On iOS device it means that Bluetooth 4.0 is not supported
		/// </summary>
		UNSUPPORTED = 2
	}
	/// <summary>
	// Flag values marking invalid rolls
	/// </summary>
	[Flags]
	public enum RollFlags {
		/// <summary>
		/// Roll is tilted flag
		/// </summary>
		TILT = 0x01,
		/// <summary>
		/// Roll was too short flag
		/// </summary>
		TOO_SHORT = 0x02,
		/// <summary>
		/// Die was touched
		/// </summary>
		TOUCH = 0x04
	}
	/// <summary>
	/// Modes in which die may be set to operate
	/// </summary>
	public enum DieMode {
		/// <summary>
		/// Mode in which die operates normally
		/// </summary>
		NORMAL = 0,
		/// <summary>
		/// Mode in which die does not light LEDs on rolls
		/// </summary>
		NO_ROLL_ANIMATIONS = 1
	}
	/// <summary>
	/// Standard led animation types
	/// </summary>
	public enum AnimationType {
		/// <summary>
		/// turns LEDs off 
		/// </summary>
		CLEAR = 0,
		/// <summary>
		/// standard roll ok animation 
		/// </summary>
		ROLL_OK = 4,
		/// <summary>
		/// standard roll failed animation 
		/// </summary>
		ROLL_FAILED = 5,
	}
	/// <summary>
	/// Bit values for led bitmasks
	/// </summary>
	[Flags]
	public enum LedFace {
		/// <summary>
		/// Led on 1st face. 
		/// </summary>
		LED_1 = 0x1,
		/// <summary>
		/// Led on 2nd face. 
		/// </summary>
		LED_2 = 0x2,
		/// <summary>
		/// Led on 3rd face. 
		/// </summary>
		LED_3 = 0x4,
		/// <summary>
		/// Led on 4th face. 
		/// </summary>
		LED_4 = 0x8,
		/// <summary>
		/// Led on 5th face. 
		/// </summary>
		LED_5 = 0x10,
		/// <summary>
		/// Led on 6th face. 
		/// </summary>
		LED_6 = 0x20,
		/// <summary>
		/// Bit mask for all leds turned on 
		/// </summary>
		LED_ALL = -1
	}

	[Flags]
	public enum DieFace {
		/// <summary>
		/// 1st face
		/// </summary>
		FACE_1 = 0x1,
		/// <summary>
		/// 2nd face
		/// </summary>
		FACE_2 = 0x2,
		/// <summary>
		/// 3rd face
		/// </summary>
		FACE_3 = 0x4,
		/// <summary>
		/// 4th face
		/// </summary>
		FACE_4 = 0x8,
		/// <summary>
		/// 5th face
		/// </summary>
		FACE_5 = 0x10,
		/// <summary>
		/// 6th face
		/// </summary>
		FACE_6 = 0x20
	}
	
	private HashSet<IDicePlusConnectorListener> listeners = new HashSet<IDicePlusConnectorListener>();
	private Dictionary<string, DicePlus> dice = new Dictionary<string, DicePlus>();
	
	/// <summary>
	/// Developer key
	/// </summary>
	public int[] developerKey = new int[8] {
	    131,
	    237,
	    96,
	    14,
	    93,
	    49,
	    143,
	    231,
	};

#if UNITY_EDITOR

#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern bool ios_create(int [] developerKey);
	[DllImport ("__Internal")]
	private static extern void ios_disconnectAll();
	[DllImport ("__Internal")]
	private static extern void ios_startScan();
	[DllImport ("__Internal")]
	private static extern void ios_stopScan();
	[DllImport ("__Internal")]
	private static extern void ios_connect(string address);
	[DllImport ("__Internal")]
	private static extern bool ios_connected(string address);
	[DllImport ("__Internal")]
	private static extern void ios_disconnect(string addres);
	[DllImport ("__Internal")]
	private static extern void ios_sleep(string addres);
	[DllImport ("__Internal")]
	private static extern void ios_destroy();
	[DllImport ("__Internal")]
	private static extern string ios_getModelNumber(string address);
	[DllImport ("__Internal")]
	private static extern string ios_getSoftwareVersion(string address);
	[DllImport ("__Internal")]
	private static extern string ios_getHardwareVersion(string address);
	[DllImport ("__Internal")]
	private static extern string ios_getFaceCount(string address);
	[DllImport ("__Internal")]
	private static extern string ios_getLedCount(string address);
	[DllImport ("__Internal")]
	private static extern string ios_getUID(string address);
	[DllImport ("__Internal")]
	private static extern string ios_getStatus(string address);
	[DllImport ("__Internal")]
	private static extern void ios_getDiceStatistics(string address);
	[DllImport ("__Internal")]
	private static extern void ios_subscribeAccelerometerReadouts(string address);
	[DllImport ("__Internal")]
	private static extern void ios_subscribeAndConfigureAccelerometerReadouts(string address, int readFrequency, int readoutType);

	[DllImport ("__Internal")]
	private static extern void ios_unsubscribeAccelerometerReadouts(string address);	
	[DllImport ("__Internal")]
	private static extern void ios_subscribeMagnetometerReadouts(string address);	

	[DllImport ("__Internal")]
	private static extern void ios_subscribeAndConfigureMagnetometerReadouts(string address, int readFrequency, int readoutType);

	[DllImport ("__Internal")]
	private static extern void ios_unsubscribeMagnetometerReadouts(string address);	
	[DllImport ("__Internal")]
	private static extern void ios_subscribeRolls(string address);
	[DllImport ("__Internal")]
	private static extern void ios_unsubscribeRolls(string address);

	[DllImport ("__Internal")]
	private static extern void ios_subscribeFaceReadouts(string address);
	[DllImport ("__Internal")]
	private static extern void ios_unsubscribeFaceReadouts(string address);

	[DllImport ("__Internal")]
	private static extern void ios_subscribeTapReadouts(string address);
	[DllImport ("__Internal")]
	private static extern void ios_unsubscribeTapReadouts(string address);

	[DllImport ("__Internal")]
	private static extern void ios_subscribeTemperatureReadouts(string address);

	[DllImport ("__Internal")]
	private static extern void ios_unsubscribeTemperatureReadouts(string address);

	[DllImport ("__Internal")]
	private static extern void ios_subscribeOrientationReadouts(string address);
	[DllImport ("__Internal")]
	private static extern void ios_subscribeAndConfigureOrientationReadouts(string address, int freq);

	[DllImport ("__Internal")]
	private static extern void ios_unsubscribeOrientationReadouts(string address);		
	[DllImport ("__Internal")]
	private static extern void ios_subscribeTouchReadouts(string address);		

	[DllImport ("__Internal")]
	private static extern void ios_unsubscribeTouchReadouts(string address);
	[DllImport ("__Internal")]
	private static extern void ios_subscribeProximityReadouts(string address);
	[DllImport ("__Internal")]
	private static extern void ios_subscribeAndConfigureProximityReadouts(string address, int freq);

	[DllImport ("__Internal")]
	private static extern void ios_unsubscribeProximityReadouts(string address);	

	[DllImport ("__Internal")]
	private static extern void ios_subscribeLedState(string address);

	[DllImport ("__Internal")]
	private static extern void ios_unsubscribeLedState(string address);	
	[DllImport ("__Internal")]
	private static extern void ios_subscribeBatteryState(string address);

	[DllImport ("__Internal")]
	private static extern void ios_unsubscribeBatteryState(string address);	

	[DllImport ("__Internal")]
	private static extern void ios_subscribePowerMode(string address);

	[DllImport ("__Internal")]
	private static extern void ios_unsubscribePowerMode(string address);	
	[DllImport ("__Internal")]
	private static extern void ios_runBlinkAnimation(string address, int ledMask, int priority, int r, int g, int b, int ledOnPeriod, int ledCyclePeriod, int blinkNumber);	
	[DllImport ("__Internal")]
	private static extern void ios_runFadeAnimation(string address, int ledMask, int priority, int r, int g, int b, int fadeInOutTime, int pauseTime);	
	[DllImport ("__Internal")]
	private static extern void ios_runStandardAnimation(string address, int ledMask, int priority, int animation);
	[DllImport ("__Internal")]
	private static extern void ios_setMode(string address, int mode);
	
	[DllImport ("__Internal")]
	private static extern void ios_writePStorageStrValue(string address, int handle, string str);
	[DllImport ("__Internal")]
	private static extern void ios_writePStorageVecValue(string address, int handle, int x, int y, int z);
	[DllImport ("__Internal")]
	private static extern void ios_writePStorageIntValue(string address, int handle, int intvalue);
	[DllImport ("__Internal")]
	private static extern void ios_readPStorageValue(string address, int handle);
	[DllImport ("__Internal")]
	private static extern void ios_initializePStorageCommunication(string address);
	[DllImport ("__Internal")]
	private static extern string ios_getPStorageRecordDescription(string address, int handle);
	[DllImport ("__Internal")]
	private static extern void ios_resetPStorage(string address);
#elif UNITY_ANDROID
	private AndroidJavaObject dmo;
#endif
	
	/// <summary>
	/// Should game object persist scene transitions
	/// </summary>
	public bool persistSceneTransitions = true;
	
	private static DicePlusConnector instance;
	public static DicePlusConnector Instance {
		get {
			return instance;
		}
	}
	
	bool initialized = false;
	
	public bool isInitialized() {
		return initialized;
	}
	
	void Awake () {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
			return;
		}
		if (persistSceneTransitions) {
			DontDestroyOnLoad(transform.gameObject);
		}
		if (create() == false) {
			Debug.LogError("DICE+ wrapper initialization failed");
			initialized = true;
		}
	}
	
	void OnDestroy() {
		if (this == instance) {
			disconnectAll();
			instance = null;
		}
	}
	
	/// <summary>
	/// Creates wrapper instance on native site
	/// </summary>
	public bool create() {
#if UNITY_EDITOR
		return true;
#elif UNITY_IPHONE
		return ios_create(developerKey);
	
#elif UNITY_ANDROID
//		AndroidJNIHelper.debug = true;
		using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity")) {
				using (AndroidJavaClass dmc = new AndroidJavaClass("us.dicepl.unitywrapper.Wrapper")) {
					dmo = dmc.CallStatic<AndroidJavaObject>("create", new object [] {jo, developerKey});
					if (dmo.Call<bool>("isValid")) {
						return true;
					} else {
						dmo = null;
						return false;
					}
				}
			}
		}
#else
		return false;
#endif
	}
	/// <summary>
	/// Disconnects all connected and/or authenticated dice. 
	/// Closes also pending connections.
	/// </summary>
	public void disconnectAll () {
#if UNITY_EDITOR

#elif UNITY_IPHONE
		ios_disconnectAll();
#elif UNITY_ANDROID
		if (dmo != null) {
			dmo.Call("disconnectAll");
		}
#endif
	}
	/// <summary>
	/// Destroys wrapper instance on native site
	/// </summary>
	public void destroy () {
#if UNITY_EDITOR

#elif UNITY_IPHONE
		ios_destroy();
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("destroy");
			dmo = null;
		}
#endif
	}
	/// <summary>
	/// Initiates the scan process to look for new dice devices
	/// </summary>
	/// <returns>
	/// true on success, false on error
	/// </returns>
	public bool startScan() {
#if UNITY_EDITOR

#elif UNITY_IPHONE

	ios_startScan();
	return true;
#elif UNITY_ANDROID

		if (dmo != null) {
			return dmo.Call<bool>("startScan");
		}
#endif
		return false;
	}
	/// <summary>
	/// Cancels an ongoing bluetooth scan (if in progress)
	/// </summary>
	public void stopScan() {
#if UNITY_EDITOR

#elif UNITY_IPHONE

	ios_stopScan();
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("stopScan");
		}
#endif
	}
	/// <summary>
	/// Registers <see cref="IDicePlusConnectorListener"/> to be informed about bluetooth scan related events
	/// </summary>
	/// <param name='listener'>
	/// registered listener
	/// </param>
	public void registerListener(IDicePlusConnectorListener listener) {
		HashSet<IDicePlusConnectorListener> tmplisteners = new HashSet<IDicePlusConnectorListener>(listeners);
		tmplisteners.Add(listener);
		listeners = tmplisteners;	
	}
	/// <summary>
	/// Unregisters <see cref="IDicePlusConnectorListener"/>
	/// </summary>
	/// <param name='listener'>
	/// unregistered listener
	/// </param>
	public void unregisterListener(IDicePlusConnectorListener listener) {
		HashSet<IDicePlusConnectorListener> tmplisteners = new HashSet<IDicePlusConnectorListener>(listeners);
		tmplisteners.Remove(listener);
		listeners = tmplisteners;
	}

	void onNewDie(string str) {
		string[] words = str.Split('#');
		string addres = words[0];

		DicePlus dicePlus;
		if (dice.ContainsKey(addres)) {
			dicePlus = dice[addres];
		} else {
			dicePlus = new DicePlus(this, addres);
			dice.Add(addres, dicePlus);
		}
		dicePlus.rssi = int.Parse(words[1]);
		foreach (IDicePlusConnectorListener listener in listeners) {
			listener.onNewDie(dicePlus);
		}
 	}
	
	BluetoothState? btState = null;
	
	void onBluetoothStateChanged(string str) {
		BluetoothState state = BluetoothState.NOT_READY;
		if (str.Equals("ready")) {
			state = BluetoothState.READY;
		} else if (str.Equals("unsupported")) {
			state = BluetoothState.UNSUPPORTED;
		}
		if ((btState == null) || (btState != state)) {
			btState = state;
			foreach (IDicePlusConnectorListener listener in listeners) {
				listener.onBluetoothStateChanged(state);
			}
		}
	}
	/// <summary>
	/// Returns current state of bluetooth adapter
	/// </summary>
	/// <returns>
	/// true if bluetooth is enabled and ready, false otherwise
	/// </returns>
	public BluetoothState? getBluetoothState() {
		return btState;
	}

	void onScanStarted(string str) {
		foreach (IDicePlusConnectorListener listener in listeners) {
			listener.onScanStarted();
		}
	}

	void onScanFinished(string str) {
		bool fail = str.Equals("true");
		foreach (IDicePlusConnectorListener listener in listeners) {
			listener.onScanFinished(fail);
		}
	}
		
	void onAccelerometerReadout(string str) {
		string[] words = str.Split('#');
		if (dice[words[0]] == null) return;
		if (words[1].StartsWith("error:")) {
			dice[words[0]].onAccelerometerReadout(0, Vector3.zero, 0, words[1]);
			return;
		}
		dice[words[0]].onAccelerometerReadout(long.Parse(words[1]), new Vector3 (int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4])), int.Parse(words[5]), null);
		
	}
	void onOrientationReadout(string str) {
		string[] words = str.Split('#');
			
		if (dice[words[0]] == null) return;
		if (words[1].StartsWith("error:")) {
			dice[words[0]].onOrientationReadout(0, Vector3.zero, words[1]);
			return;
		}
		dice[words[0]].onOrientationReadout(long.Parse(words[1]), new Vector3 (int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4])), null);
		
	}

	void onTemperatureReadout(string str) {
		string[] words = str.Split('#');
		
		if (dice[words[0]] == null) return;
		if (words[1].StartsWith("error:")) {
			dice[words[0]].onTemperatureReadout(0, 0, words[1]);
			return;
		}
		
		dice[words[0]].onTemperatureReadout(long.Parse(words[1]), float.Parse(words[2]), null);
	}
	
	void onBatteryState(string str) {
		string[] words = str.Split('#');
		
		if (dice[words[0]] == null) return;
		if (words[1].StartsWith("error:")) {
			dice[words[0]].onBatteryState(0, 0, false, words[1]);
			return;
		}
		
		dice[words[0]].onBatteryState((BatteryState)int.Parse(words[1]), int.Parse(words[2]), (int.Parse(words[3]) != 0), null);
	}
	void onTouchReadout(string str) {
		string[] words = str.Split('#');
		
		if (dice[words[0]] == null) return;
		if (words[1].StartsWith("error:")) {
			dice[words[0]].onTouchReadout(0, 0, 0, words[1]);
			return;
		}
		
		dice[words[0]].onTouchReadout(long.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]), null);
	}
	
	void onProximityReadout(string str) {
		string[] words = str.Split('#');

		if (dice[words[0]] == null) return;
		if (words[1].StartsWith("error:")) {
			dice[words[0]].onProximityReadout(0, new List<int>(), words[1]);
			return;
		}
		
		List<int> list = new List<int>();
		for (int i = 2; i < words.Length; i++) {
			list.Add(int.Parse(words[i]));
		}
		dice[words[0]].onProximityReadout(long.Parse(words[1]), list, null);
	}
	
	void onRunLedAnimationStatus(string str) {
		string[] words = str.Split('#');
		
		if (dice[words[0]] == null) return;
		dice[words[0]].onRunLedAnimationStatus((words[1].StartsWith("error:")?words[1]:null));
	}	
	
	void onRoll(string str) {
		string[] words = str.Split('#');
			
		if (dice[words[0]] == null) return;
		if (words[1].StartsWith("error:")) {
			dice[words[0]].onRoll(0, 0, 0, 10, words[1]);
			return;
		}
		dice[words[0]].onRoll(long.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4]), null);
		
	}

	void onFaceReadout(string str) {
		string[] words = str.Split('#');

		if (dice[words[0]] == null) return;
		if (words[1].StartsWith("error:")) {
			dice[words[0]].onFaceReadout(0, 0, words[1]);
			return;
		}
		dice[words[0]].onFaceReadout(long.Parse(words[1]), int.Parse(words[2]), null);

	}

	void onTapReadout(string str) {
		string[] words = str.Split('#');

		if (dice[words[0]] == null) return;
		if (words[1].StartsWith("error:")) {
			dice[words[0]].onTapReadout(0, Vector3.zero, words[1]);
			return;
		}
		dice[words[0]].onTapReadout(long.Parse(words[1]), new Vector3 (int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4])), null);

	}	
	
	void onMagnetometerReadout(string str) {
		string[] words = str.Split('#');
			
		if (dice[words[0]] == null) return;
		if (words[1].StartsWith("error:")) {
			dice[words[0]].onMagnetometerReadout(0, Vector3.zero, 0, words[1]);
			return;
		}
		dice[words[0]].onMagnetometerReadout(long.Parse(words[1]), new Vector3 (int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4])), int.Parse(words[5]), null);
		
	}	
	
	void onLedState(string str) {
		string[] words = str.Split('#');
		
		if (dice[words[0]] == null) return;
		if (words[1].StartsWith("error:")) {
			dice[words[0]].onLedState(0, 0, 0, 0, words[1]);
			return;
		}
		dice[words[0]].onLedState(long.Parse(words[1]), (LedFace)long.Parse(words[2]), long.Parse(words[3]), int.Parse(words[4]), null);
	}

	void onConnectionLost(string str) {
		string[] words = str.Split('#');
				
		DicePlus dicePlus = dice[words[0]];
		if (dicePlus == null) return;

		foreach (IDicePlusConnectorListener listener in listeners) {
			listener.onConnectionLost(dicePlus);
		}
	}	
	
	void onConnectionEstablished(string str) {
		string[] words = str.Split('#');
				
		DicePlus dicePlus = dice[words[0]];
		if (dicePlus == null) return;

		foreach (IDicePlusConnectorListener listener in listeners) {
			listener.onConnectionEstablished(dicePlus);
		}
	}	
	
	void onConnectionFailed(string str) {
		string[] words = str.Split('#');
				
		DicePlus dicePlus = dice[words[0]];
		if (dicePlus == null) return;

		foreach (IDicePlusConnectorListener listener in listeners) {
			listener.onConnectionFailed(dicePlus, int.Parse(words[1]), words[2]);
		}
	}
	
	void onSubscriptionChangeStatus(string str) {
		string[] words = str.Split('#');
		
		if (dice[words[0]] == null) return;
		dice[words[0]].onSubscriptionChangeStatus((DataSource)int.Parse(words[1]), (words[2].StartsWith("error:")?words[2]:null));
	}
	
	void onSetModeStatus(string str) {
		string[] words = str.Split('#');
		
		if (dice[words[0]] == null) return;
		dice[words[0]].onSetModeStatus((words[1].StartsWith("error:")?words[1]:null));
	}
	void onPowerMode(string str) {
		string[] words = str.Split('#');
		
		if (dice[words[0]] == null) return;
		if (words[1].StartsWith("error:")) {
			dice[words[0]].onPowerMode(0, PowerMode.NORMAL, words[1]);
			return;
		}
		dice[words[0]].onPowerMode(long.Parse(words[1]), (PowerMode)int.Parse(words[2]), null);
	}	
	void onSleepStatus(string str) {
		string[] words = str.Split('#');
		
		if (dice[words[0]] == null) return;
		dice[words[0]].onSleepStatus((words[1].StartsWith("error:")?words[1]:null));
	}

	void onDiceStatistics(string str) {
		string[] words = str.Split('#');
		if (dice[words[0]] == null) return;
		if (words[1].StartsWith("error:")) {
			dice[words[0]].onDiceStatistics(new DiceStatistics(), words[1]);
			return;
		}
		
		List<long> list = new List<long>();
		for (int i = 9; i < words.Length; i++) {
			list.Add(long.Parse(words[i]));
		}
		
		dice[words[0]].onDiceStatistics(new DiceStatistics(long.Parse(words[2]), long.Parse(words[1]), list, long.Parse(words[6]), long.Parse(words[3]), long.Parse(words[7]), long.Parse(words[4]), long.Parse(words[5]), long.Parse(words[8])), null);
	}
	
	void onPStorageReset(string str) {
		string[] words = str.Split('#');
		
		if (dice[words[0]] == null) return;
		dice[words[0]].onPStorageReset((words[1].StartsWith("error:")?words[1]:null));
	}

	void onPStorageStrValueRead(string str) {
		char [] delimiters = {'#'};
		string[] words = str.Split(delimiters, 3);
		
		if (dice[words[0]] == null) return;
		dice[words[0]].onPStorageValueRead(int.Parse(words[1]), words[2]);
	}

	void onPStorageVec3ValueRead(string str) {
		string[] words = str.Split('#');
		
		if (dice[words[0]] == null) return;
		dice[words[0]].onPStorageValueRead(int.Parse(words[1]), new Vector3(int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4])));
	}

	void onPStorageIntValueRead(string str) {
		string[] words = str.Split('#');
		
		if (dice[words[0]] == null) return;
		dice[words[0]].onPStorageValueRead(int.Parse(words[1]), int.Parse(words[2]));
	}

	void onPStorageCommunicationInitialized(string str) {
		string[] words = str.Split('#');
		
		if (dice[words[0]] == null) return;
		dice[words[0]].onPStorageCommunicationInitialized(int.Parse(words[1]));
	}

	void onPStorageOperationFailed(string str) {
		string[] words = str.Split('#');
		
		if (dice[words[0]] == null) return;
		dice[words[0]]. onPStorageOperationFailed((words[1].StartsWith("error:")?words[1]:null));
	}

	void onPStorageValueWrite(string str) {
		string[] words = str.Split('#');
		
		if (dice[words[0]] == null) return;
		dice[words[0]].onPStorageValueWrite(int.Parse(words[1]));
	}
	
	/// <summary>
	/// Sends request to the given dice to reset all persistent storage settings
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	resetPStorage(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_resetPStorage(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("resetPStorage", new object [] {dicePlus.address});
		}
#endif
	}
	/// <summary>
	/// Sends request to the given dice to initialize persistent storage communication
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	initializePStorageCommunication(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_initializePStorageCommunication(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("initializePStorageCommunication", new object [] {dicePlus.address});
		}
#endif
	} 
	/// <summary>
	/// Sends request to the given dice to get persistent storage record value
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='handle'>
	/// the handle to requested record
	/// </param>
	public void	readPStorageValue(DicePlus dicePlus, int handle){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_readPStorageValue(dicePlus.address, handle);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("readPStorageValue", new object [] {dicePlus.address, handle});
		}
#endif
	} 
	
	/// <summary>
	/// Sends request to the given dice to set persistent storage record value
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='handle'>
	/// the handle to requested record
	/// </param>
	/// <param name='intvalue'>
	/// integer value to be set
	/// </param>
	public void	writePStorageValue(DicePlus dicePlus, int handle, int intvalue){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_writePStorageIntValue(dicePlus.address, handle, intvalue);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("writePStorageIntValue", new object [] {dicePlus.address, handle, intvalue});
		}
#endif
	} 
	
	/// <summary>
	/// Sends request to the given dice to set persistent storage record value
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
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
	public void	writePStorageValue(DicePlus dicePlus, int handle, int x, int y, int z){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_writePStorageVecValue(dicePlus.address, handle, x, y, z);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("writePStorageVecValue", new object [] {dicePlus.address, handle, x, y, z});
		}
#endif
	} 
	
	/// <summary>
	/// Sends request to the given dice to set persistent storage record value
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='handle'>
	/// the handle to requested record
	/// </param>
	/// <param name='str'>
	/// string value to be set
	/// </param>
	public void	writePStorageValue(DicePlus dicePlus, int handle, string str){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_writePStorageStrValue(dicePlus.address, handle, str);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("writePStorageStrValue", new object [] {dicePlus.address, handle, str});
		}
#endif
	}
	
	/// <summary>
	/// Gets persistent storage record description for given handle
	/// </summary>
	/// <returns>
	/// persistent storage record description, null if handle was invalid
	/// </returns>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='handle'>
	/// the handle to requested record
	/// </param>
	public PStorageRecordDescription getPStorageRecordDescription(DicePlus dicePlus, int handle) {
		string result = null;
#if UNITY_EDITOR

#elif UNITY_IPHONE

		result = ios_getPStorageRecordDescription(dicePlus.address, handle);
#elif UNITY_ANDROID

		if (dmo != null) {
			result = dmo.Call<string>("getPStorageRecordDescription", new object [] {dicePlus.address, handle});
		}
#endif	
		PStorageRecordDescription retVal;
		if (result == null || result.Equals("") || !PStorageRecordDescription.TryParse(result, out retVal)) {
			return null;
		} else {
			return retVal;
		}
	}

	/// <summary>
	/// Subscribes to accelerometer readouts with default values. 
	/// Frequency - 10 Hz, readout filter type - AccelerometerReadoutType.READOUT_RAW
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	subscribeAccelerometerReadouts(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_subscribeAccelerometerReadouts(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribeAccelerometerReadouts", new object [] {dicePlus.address});
		}
#endif
	} 		
	
	/// <summary>
	/// Subscribes to accelerometer readouts and configure sensor
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='readFrequency'>
	/// readout frequency (in Hz), values: 1 - 60
	/// </param>
	/// <param name='readoutType'>
	/// a readout filter type
	/// </param>
	public void	subscribeAccelerometerReadouts(DicePlus dicePlus, int readFrequency, AccelerometerReadoutType readoutType){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_subscribeAndConfigureAccelerometerReadouts(dicePlus.address, readFrequency, (int)readoutType);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribeAndConfigureAccelerometerReadouts", new object [] {dicePlus.address, readFrequency, (int)readoutType});
		}
#endif
	} 
	
	/// <summary>
	/// Unsubscribes from accelerometer readouts
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	unsubscribeAccelerometerReadouts(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_unsubscribeAccelerometerReadouts(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("unsubscribeAccelerometerReadouts", new object [] {dicePlus.address});
		}
#endif
	} 
	
	/// <summary>
	/// Subscribes to periodic battery state events. Sends current battery state query
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	subscribeBatteryState(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

	ios_subscribeBatteryState(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribeBatteryState", new object [] {dicePlus.address});
		}
#endif
	} 
	
	/// <summary>
	/// Unsubscribes from battery state events
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	unsubscribeBatteryState(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

	ios_unsubscribeBatteryState(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("unsubscribeBatteryState", new object [] {dicePlus.address});
		}
#endif
	}
	
	/// <summary>
	/// Subscribes the face change events
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	subscribeFaceReadouts(DicePlus dicePlus){
		#if UNITY_EDITOR

		#elif UNITY_IPHONE

		ios_subscribeFaceReadouts(dicePlus.address);
		#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribeFaceReadouts", new object [] {dicePlus.address});
		}
		#endif
	} 
	/// <summary>
	/// Unsubscribes the face change events
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	unsubscribeFaceReadouts(DicePlus dicePlus){
		#if UNITY_EDITOR

		#elif UNITY_IPHONE

		ios_unsubscribeFaceReadouts(dicePlus.address);
		#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("unsubscribeFaceReadouts", new object [] {dicePlus.address});
		}
		#endif
	} 
	/// <summary>
	/// Subscribes the tap events
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	subscribeTapReadouts(DicePlus dicePlus){
		#if UNITY_EDITOR

		#elif UNITY_IPHONE

		ios_subscribeTapReadouts(dicePlus.address);
		#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribeTapReadouts", new object [] {dicePlus.address});
		}
		#endif
	} 
	/// <summary>
	/// Unsubscribes the tap events
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	unsubscribeTapReadouts(DicePlus dicePlus){
		#if UNITY_EDITOR

		#elif UNITY_IPHONE

		ios_unsubscribeTapReadouts(dicePlus.address);
		#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("unsubscribeTapReadouts", new object [] {dicePlus.address});
		}
		#endif
	} 

	/// <summary>
	/// Subscribes to touch/release state events. Sends current touch/release state query.
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	subscribeTouchReadouts(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

	ios_subscribeTouchReadouts(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribeTouchReadouts", new object [] {dicePlus.address});
		}
#endif
	} 
	
	/// <summary>
	/// Unsubscribes from touch/release state events
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	unsubscribeTouchReadouts(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

	ios_unsubscribeTouchReadouts(dicePlus.address);	
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("unsubscribeTouchReadouts", new object [] {dicePlus.address});
		}
#endif
	} 
	
	/// <summary>
	/// Subscribes to proximity readouts
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	subscribeProximityReadouts(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_subscribeProximityReadouts(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribeProximityReadouts", new object [] {dicePlus.address});
		}
#endif
	} 	
	
	/// <summary>
	/// Subscribes to proximity readouts and configures sensor
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='freq'>
	/// readout frequency (in Hz), values: 1 - 10
	/// </param>
	public void	subscribeProximityReadouts(DicePlus dicePlus, int freq){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_subscribeAndConfigureProximityReadouts(dicePlus.address, freq);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribeAndConfigureProximityReadouts", new object [] {dicePlus.address, freq});
		}
#endif
	} 
	
	/// <summary>
	/// Unsubscribes from proximity sensor readouts
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	unsubscribeProximityReadouts(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_unsubscribeProximityReadouts(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("unsubscribeProximityReadouts", new object [] {dicePlus.address});
		}
#endif
	} 
	
	/// <summary>
	/// Subscribes to LED state events - run on LED animation start and finish
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	subscribeLedState(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_subscribeLedState(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribeLedState", new object [] {dicePlus.address});
		}
#endif
	} 	
	
	/// <summary>
	/// Unsubscribes from LED state events
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	unsubscribeLedState(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_unsubscribeLedState(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("unsubscribeLedState", new object [] {dicePlus.address});
		}
#endif
	} 
	
	/// <summary>
	/// Subscribes to magnetometer readouts with default values. 
	/// Frequency - 10 Hz, readout filter type - MagnetometerReadoutType.READOUT_RAW
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	subscribeMagnetometerReadouts(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_subscribeMagnetometerReadouts(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribeMagnetometerReadouts", new object [] {dicePlus.address});
		}
#endif
	} 	
	
	/// <summary>
	/// Subscribes to magnetometer readouts and configures sensor.
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='freq'>
	/// readout frequency (in Hz), values: 1 - 30
	/// </param>
	/// <param name='type'>
	/// a readout type
	/// </param>
	public void	subscribeMagnetometerReadouts(DicePlus dicePlus, int freq, MagnetometerReadoutType type){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_subscribeAndConfigureMagnetometerReadouts(dicePlus.address, freq, (int)type);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribeAndConfigureMagnetometerReadouts", new object [] {dicePlus.address, freq, (int)type});
		}
#endif
	} 	

	/// <summary>
	/// Unsubscribes from magnetometer readouts.
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	unsubscribeMagnetometerReadouts(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_unsubscribeMagnetometerReadouts(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("unsubscribeMagnetometerReadouts", new object [] {dicePlus.address});
		}
#endif
	} 
	
	/// <summary>
	/// Subscribes to temperature readouts.
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	subscribeTemperatureReadouts(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_subscribeTemperatureReadouts(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribeTemperatureReadouts", new object [] {dicePlus.address});
		}
#endif
	} 	
	
	/// <summary>
	/// Unsubscribes from temperature readouts.
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	unsubscribeTemperatureReadouts(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_unsubscribeTemperatureReadouts(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("unsubscribeTemperatureReadouts", new object [] {dicePlus.address});
		}
#endif
	} 

	/// <summary>
	/// Subscribes to orientation readouts
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	subscribeOrientationReadouts(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_subscribeOrientationReadouts(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribeOrientationReadouts", new object [] {dicePlus.address});
		}
#endif
	} 	
	
	/// <summary>
	/// Subscribes to orientation readouts and configures sensor.
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='freq'>
	/// readout frequency (in Hz), values: 1 - 20
	/// </param>
	public void	subscribeOrientationReadouts(DicePlus dicePlus, int freq){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_subscribeAndConfigureOrientationReadouts(dicePlus.address, freq);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribeAndConfigureOrientationReadouts", new object [] {dicePlus.address, freq});
		}
#endif
	} 	
	
	/// <summary>
	/// Unsubscribes from orientation readouts
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	unsubscribeOrientationReadouts(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_unsubscribeOrientationReadouts(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("unsubscribeOrientationReadouts", new object [] {dicePlus.address});
		}
#endif
	} 	
	
	/// <summary>
	/// Subscribes from PowerMode changes. Sends current PowerMode query.
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	subscribePowerMode(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_subscribePowerMode(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribePowerMode", new object [] {dicePlus.address});
		}
#endif
	} 	
	
	/// <summary>
	/// Unsubscribes from PowerMode changes
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	unsubscribePowerMode(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_unsubscribePowerMode(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("unsubscribePowerMode", new object [] {dicePlus.address});
		}
#endif
	} 
	
	/// <summary>
	/// Subscribes to roll events
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	subscribeRolls(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_subscribeRolls(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("subscribeRolls", new object [] {dicePlus.address});
		}
#endif
	} 	
	
	/// <summary>
	/// Unsubscribes from roll events
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	unsubscribeRolls(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_unsubscribeRolls(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("unsubscribeRolls", new object [] {dicePlus.address});
		}
#endif
	} 

	/// <summary>
	/// Starts connecting process. Any current process of finding new dice will be stopped.
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	connect(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_connect(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("connect", new object [] {dicePlus.address});
		}
#endif
	} 
	
	/// <summary>
	/// Disconnects specified dice.
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	disconnect(DicePlus dicePlus){
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_disconnect(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("disconnect", new object [] {dicePlus.address});
		}
#endif		
	} 
	
	/// <summary>
	/// Sends setMode packet to the given die
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='mode'>
	/// mode to set
	/// </param>
	public void	setMode(DicePlus dicePlus, DieMode mode) {
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_setMode(dicePlus.address, (int)mode);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("setMode", new object [] {dicePlus.address, (int)mode});
		}
#endif	
	}
	
	/// <summary>
	/// Sends sleep request to the given dice
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	sleep(DicePlus dicePlus) {
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_sleep(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("sleep", new object [] {dicePlus.address});
		}
#endif	
	}
	
	/// <summary>
	/// Sends the die statistics request to the given dice.
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	public void	getDiceStatistics(DicePlus dicePlus) {
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_getDiceStatistics(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("getDiceStatistics", new object [] {dicePlus.address});
		}
#endif	
	}
	
	/// <summary>
	/// Gets software version.
	/// </summary>
	/// <returns>
	/// number representing software version, null when die has not yet connected and authenticated connection.
	/// </returns>
	/// <param name='dicePlus'>
	/// the dice
	/// </param>
	public SoftwareVersion getSoftwareVersion(DicePlus dicePlus) {
		string result = null;
#if UNITY_EDITOR

#elif UNITY_IPHONE

		result = ios_getSoftwareVersion(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			result = dmo.Call<string>("getSoftwareVersion", new object [] {dicePlus.address});
		}
#endif	
		if (result == null || result.Equals("")) {
			return null;
		} else {
			return SoftwareVersion.Parse(result);
		}
	}
	
	/// <summary>
	/// Gets dice unique identifier
	/// </summary>
	/// <returns>
	/// unique identifier string, null when die has not yet connected and authenticated connection.
	/// </returns>
	/// <param name='dicePlus'>
	/// the die
	/// </param>
	public string getUID(DicePlus dicePlus) {
		string result = null;
		#if UNITY_EDITOR

		#elif UNITY_IPHONE

		result = ios_getUID(dicePlus.address);
		#elif UNITY_ANDROID

		if (dmo != null) {
			result = dmo.Call<string>("getUID", new object [] {dicePlus.address});
		}
		#endif	
		if (result == null || result.Equals("")) {
			return null;
		} else {
			return result;
		}
	}
	
	/// <summary>
	/// Gets hardware version.
	/// </summary>
	/// <returns>
	/// number representing hardware version, null when die has not yet connected and authenticated connection.
	/// </returns>
	/// <param name='dicePlus'>
	/// the die
	/// </param>
	public HardwareVersion getHardwareVersion(DicePlus dicePlus) {
		string result = null;
#if UNITY_EDITOR

#elif UNITY_IPHONE

		result = ios_getHardwareVersion(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			result = dmo.Call<string>("getHardwareVersion", new object [] {dicePlus.address});
		}
#endif	
		if (result == null || result.Equals("")) {
			return null;
		} else {
			return HardwareVersion.Parse(result);;
		}
	}
	
	/// <summary>
	/// Gets number of leds die has.
	/// </summary>
	/// <returns>
	/// number of leds, null when die has not yet connected and authenticated connection
	/// </returns>
	/// <param name='dicePlus'>
	/// the dice
	/// </param>
	public int? getLedCount(DicePlus dicePlus) {
		string result = null;
#if UNITY_EDITOR

#elif UNITY_IPHONE

		result = ios_getLedCount(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			result = dmo.Call<string>("getLedCount", new object [] {dicePlus.address});
		}
#endif	
		int retVal = new int();
		if (result == null || result.Equals("") || !int.TryParse(result, out retVal)) {
			return null;
		} else {
			return retVal;
		}
	}
	
	/// <summary>
	/// Gets the status.
	/// </summary>
	/// <returns>
	/// dies module status
	/// </returns>
	/// <param name='dicePlus'>
	/// the dice
	/// </param>
	public int? getStatus(DicePlus dicePlus) {
		string result = null;
		#if UNITY_EDITOR

		#elif UNITY_IPHONE

		result = ios_getStatus(dicePlus.address);
		#elif UNITY_ANDROID

		if (dmo != null) {
			result = dmo.Call<string>("getStatus", new object [] {dicePlus.address});
		}
		#endif	
		int retVal = new int();
		if (result == null || result.Equals("") || !int.TryParse(result, out retVal)) {
			return null;
		} else {
			return retVal;
		}
	}

	/// <summary>
	/// Gets number of faces die has. 
	/// </summary>
	/// <returns>
	/// number of faces, null when die has not yet connected and authenticated connection.
	/// </returns>
	/// <param name='dicePlus'>
	/// the dice
	/// </param>
	public int? getFaceCount(DicePlus dicePlus) {
		string result = null;
#if UNITY_EDITOR

#elif UNITY_IPHONE

		result = ios_getFaceCount(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			result = dmo.Call<string>("getFaceCount", new object [] {dicePlus.address});
		}
#endif	
		int retVal = new int();
		if (result == null || result.Equals("") || !int.TryParse(result, out retVal)) {
			return null;
		} else {
			return retVal;
		}
	}
	
	/// <summary>
	/// Gets device model number
	/// </summary>
	/// <returns>
	/// device model model, null when die has not yet connected and authenticated connection.
	/// </returns>
	/// <param name='dicePlus'>
	/// the dice
	/// </param>
	public long? getModelNumber(DicePlus dicePlus) {
		string result = null;
#if UNITY_EDITOR

#elif UNITY_IPHONE
		result = ios_getModelNumber(dicePlus.address);
#elif UNITY_ANDROID
		if (dmo != null) {
			result = dmo.Call<string>("getModelNumber", new object [] {dicePlus.address});
		}
#endif	
		long retVal = new long();
		if (result == null || result.Equals("") || !long.TryParse(result, out retVal)) {
			return null;
		} else {
			return retVal;
		}
	}
	
	/// <summary>
	/// Sends blink LED animation request
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
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
	public void runBlinkAnimation(DicePlus dicePlus, LedFace ledMask, int priority, 
			Color color, int ledOnPeriod, int ledCyclePeriod, int blinkNumber) {
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_runBlinkAnimation(dicePlus.address, (int)ledMask, priority, (int)(color.r*255), (int)(color.g*255), (int)(color.b*255), ledOnPeriod, ledCyclePeriod, blinkNumber);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("runBlinkAnimation", new object [] {dicePlus.address, (int)ledMask, priority, (int)(color.r*255), (int)(color.g*255), (int)(color.b*255), ledOnPeriod, ledCyclePeriod, blinkNumber});
		}
#endif	
	}
	/// <summary>
	/// Sends fade LED animation request
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
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
	public void runFadeAnimation(DicePlus dicePlus, LedFace ledMask, int priority, 
			Color color, int fadeInOutTime, int pauseTime) { 
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_runFadeAnimation(dicePlus.address, (int)ledMask, priority, (int)(color.r*255), (int)(color.g*255), (int)(color.b*255), fadeInOutTime, pauseTime);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("runFadeAnimation", new object [] {dicePlus.address, (int)ledMask, priority, (int)(color.r*255), (int)(color.g*255), (int)(color.b*255), fadeInOutTime, pauseTime});
		}
#endif	
	}
	/// <summary>
	/// Sends standard LED animation request
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='ledMask'>
	/// which LEDs to run animation on
	/// </param>
	/// <param name='priority'>
	/// animation with lower value overrides other, values: 0 - 255
	/// </param>
	/// <param name='animation'>
	/// standard animation type
	/// </param>
	public void runStandardAnimation(DicePlus dicePlus, LedFace ledMask, int priority, 
			DicePlusConnector.AnimationType animation) {	
#if UNITY_EDITOR

#elif UNITY_IPHONE

		ios_runStandardAnimation(dicePlus.address, (int)ledMask, priority, (int)animation);
#elif UNITY_ANDROID

		if (dmo != null) {
			dmo.Call("runStandardAnimation", new object [] {dicePlus.address, (int)ledMask, priority, (int)animation});
		}
#endif	
	}
	/// <summary>
	/// Checks if the die is connected and authenticated
	/// </summary>
	/// <returns>
	/// true, if the die is authenticated; false otherwise.
	/// </returns>
	/// <param name='dicePlus'>
	/// the dice
	/// </param>
	public bool	hasConnectionEstabilished(DicePlus dicePlus) {
#if UNITY_EDITOR

#elif UNITY_IPHONE

		return ios_connected(dicePlus.address);
#elif UNITY_ANDROID

		if (dmo != null) {
			return dmo.Call<bool>("hasConnectionEstabilished", new object [] {dicePlus.address});
		}
#endif	
		return false;
	}
}
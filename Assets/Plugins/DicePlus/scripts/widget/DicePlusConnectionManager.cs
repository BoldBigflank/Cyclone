using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;


public class DicePlusConnectionManager: EmptyDicePlusListener, IDicePlusConnectorListener {
	
	#region Lifetime events
	
	void Awake () {
		scanningEnabled = scanningEnabledAtStart;

		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
			return;
		}
		if (persistSceneTransitions) {
			DontDestroyOnLoad(transform.gameObject);
		}
	}
	
	void OnDestroy() {
		if (this == instance) {
			instance = null;
		}
	}
		
	void Start () {
				
		DicePlusConnector.Instance.registerListener(this);
		Reset();
	}
	
	void OnApplicationPause(bool pause) {
		if (!pause) {
			Reset();
		}
	}
	
	void Reset() {
		if (DicePlusAnimator.Instance == null) {
			return;
		}
#if UNITY_EDITOR
		setState(State.ROLLER);
		DicePlusAnimator.Instance.runRollerAnimation();
		OrbitingButtonsManager.Instance.showButtons(false);
#else
		if (state == State.ROLLER) {
			return;
		}
		/*
		if (state == State.CONNECTING) {
			state = State.DISCONNECTED;
			UnityEngine.Debug.Log("zmianana na disconnected ");
		}
		*/
		diceSet.Clear();
//		triedDiceSet.Clear();
		if (!DicePlusAnimator.Instance.isHidden()) {
			if (DicePlusConnector.Instance.getBluetoothState() == null || DicePlusConnector.Instance.getBluetoothState() == DicePlusConnector.BluetoothState.READY) {
				DicePlusAnimator.Instance.runSearchingAnimation(true);
			} else {
				DicePlusAnimator.Instance.runBluetoothDisabled();
			}
		}
#endif
	}
		
	#endregion
	
	
	public bool scanningEnabledAtStart = true;
	private bool scanningEnabled = true;
	/// <summary>
	/// Setting it to true, hides widget disregarding its current state. Setting it back to false, allows widget to change its visibility state
	/// </summary>
	/// <param name='forceHideConnector'>
	/// force hide
	/// </param>
	public void setForceHideConnector(bool forceHide) {
		DicePlusAnimator.Instance.setForceHideConnector(forceHide);
	}
	/// <summary>
	/// Setting it to true, hides widget buttons disregarding its current state. Setting it back to false, allows widget buttons to change its visibility state
	/// </summary>
	/// <param name='forceHideConnector'>
	/// force hide
	/// </param>
	public void setForceHideButtons(bool forceHide) {
		OrbitingButtonsManager.Instance.setForceHideButtons(forceHide);
	}
	/// <summary>
	/// Disables info window
	/// </summary>
	public void disableInfoWindow() {
		InfoWindowManager.Instance.disableInfoWindow();
	}

	/// <summary>
	/// Allows bluetooth scans
	/// </summary>
	public void enableScanning() {
		if (!scanningEnabled) {
			scanningEnabled = true;
			if (state != State.ROLLER && state != State.CONNECTED && state != State.NO_BLUETOOTH) {
				DicePlusAnimator.Instance.runSearchingAnimation(true);
			}
		}
	}
	/// <summary>
	/// Starts another scan process if number of connected dice is smaller than maximum dice number game may run with
	/// </summary>
	public void startScan() {
		if ((state == State.CONNECTED) && (connectedDice.Count < maxDiceCount)) {
			DicePlusAnimator.Instance.hideConnector(false, 0.25f);
			setState(State.DISCONNECTED);
			DicePlusAnimator.Instance.runSearchingAnimation(true);
		}
	}
	
	/// <summary>
	/// Stops scan and dissallows all further bluetooth scans
	/// </summary>
	public void disableScanning() {
		scanningEnabled = false;
		if (DicePlusConnector.Instance != null) {
			DicePlusConnector.Instance.stopScan();
		}
	}
	
	/// <summary>
	/// Returns wheter bluetooth scanning may be performed by widget
	/// </summary>
	/// <returns>
	/// True if scanning is currently allowed, false otherwise
	/// </returns>
	public bool isScanningEnabled() {
		return scanningEnabled;
	}
	
	//dice connection queue
	HashSet<DicePlus> diceSet = new HashSet<DicePlus>();
	
	/// <summary>
	/// Enables roll simulation button
	/// </summary>
	public void enableRoller() {
		if (state != State.CONNECTED) {
			DicePlusAnimator.Instance.runRollerAnimation();
			setState(State.ROLLER);
			OrbitingButtonsManager.Instance.showButtons(false);
			
			DicePlusConnector.Instance.stopScan();
		}
	}
	/// <summary>
	/// Disables the roll simulation button
	/// </summary>
	public void disableRoller() {
		if (DicePlusConnector.Instance.getBluetoothState() == DicePlusConnector.BluetoothState.READY) {
			DicePlusAnimator.Instance.runSearchingAnimation(true);
		} else {
			DicePlusAnimator.Instance.runBluetoothDisabled();
		}
		setState(State.DISCONNECTED);
		OrbitingButtonsManager.Instance.showButtons(true);
		
		diceSet.Clear();
	}
	/// <summary>
	/// Show/hide widget buttons
	/// </summary>
	public void toggleButtons() {
		OrbitingButtonsManager.Instance.toggleButtons();
	}
	
	public enum State {
		DISCONNECTED,
		SEARCHING,
		CONNECTING,
		CONNECTED,
		ROLLER,
		NO_BLUETOOTH
	};

	[HideInInspector]
	public State state = State.DISCONNECTED;
	
	public void setState(State newState) {
		State prevState = state;
		if (newState == State.ROLLER) {
			if (state != State.CONNECTED) {
				this.state = newState;
				if (prevState != State.ROLLER) {
					WidgetEventDispather.Instance.notifyRollerEnabled(true);
				}
			}
		} else {
			if (DicePlusConnector.Instance.getBluetoothState() == DicePlusConnector.BluetoothState.READY) {
				this.state = newState;
			} else {
				this.state = State.NO_BLUETOOTH;
			}
			if (prevState == State.ROLLER) {
				WidgetEventDispather.Instance.notifyRollerEnabled(false);
			}
		}
		if (prevState != newState) {
		}
	}
	
	public bool isRollerEnabled() {
		return state == State.ROLLER;
	}
	
	HashSet<DicePlus> connectedDice = new HashSet<DicePlus>();
	/// <summary>
	/// Gets set of all dice which are connected and authenticated to this game
	/// </summary>
	/// <returns>
	/// Currently connected dice set
	/// </returns>
	public HashSet<DicePlus> getConnectedDice() {
		return new HashSet<DicePlus>(connectedDice);
	}
	
	/// <summary>
	/// Should game object persist scene transitions
	/// </summary>
	public bool persistSceneTransitions = true;
	
	private static DicePlusConnectionManager instance;
	public static DicePlusConnectionManager Instance {
		get {
			return instance;
		}
	}
	
	#region Roller
	/// <summary>
	/// Registers roll simulation listener.
	/// </summary>
	/// <param name='listener'>
	/// Listener
	/// </param>
	public void registerRollerListener(IDicePlusListener listener) {
		HashSet<IDicePlusListener> tmplisteners = new HashSet<IDicePlusListener>(rollerListeners);
		tmplisteners.Add(listener);
		rollerListeners = tmplisteners;
	}
	/// <summary>
	/// Unregisters roll simulation listener.
	/// </summary>
	/// <param name='listener'>
	/// Listener
	/// </param>
	public void unregisterRollerListener(IDicePlusListener listener) {
		HashSet<IDicePlusListener> tmplisteners = new HashSet<IDicePlusListener>(rollerListeners);
		tmplisteners.Remove(listener);
		rollerListeners = tmplisteners;
	}
	
	private HashSet<IDicePlusListener> rollerListeners = new HashSet<IDicePlusListener>();
	
	int counter = 0;
	/// <summary>
	/// Simulate roll
	/// </summary>
	public void roll() {
		if (++counter % 100 == 0) {
			StartCoroutine(OrbitingButtonsManager.Instance.buyMeCorutine());
		}
		int face = Random.Range(1,7);
		foreach (IDicePlusListener listener in rollerListeners) {
			listener.onRoll(null, 0, 500, face, 0, null);
		}
	}
	
	#endregion
	
	/// <summary>
	/// The maximum dice number the game runs with
	/// </summary>
	public int maxDiceCount = 1;
	/// <summary>
	/// The minimum dice number the game runs with
	/// </summary>
	public int minDiceCount = 1;
	
	int rssi = int.MaxValue;
	/// <summary>
	/// Wait that long to find closest DICE+
	/// </summary>
	public float findClosestTime = 3f;
	float rssiSearchTimoutTime;
		
	public void onBluetoothStateChanged(DicePlusConnector.BluetoothState btstate) {
		if (btstate != DicePlusConnector.BluetoothState.READY) {
			//TODO remove jak sdk beda zminione
			DicePlusConnector.Instance.disconnectAll();
		}
		if (state != State.ROLLER) {
			if (btstate == DicePlusConnector.BluetoothState.READY) {
				DicePlusAnimator.Instance.runSearchingAnimation(true);
				setState(State.DISCONNECTED);
			} else {
				DicePlusAnimator.Instance.runBluetoothDisabled();
				setState(State.NO_BLUETOOTH);
			}
		}
	}
	
	public void onNewDie(DicePlus ndp) {
		
		if (
			!connectedDice.Contains(ndp)) {
			diceSet.Add(ndp);
			
			if (ndp.rssi >= rssi - 5) {
				setState(State.DISCONNECTED);
				DicePlusConnector.Instance.stopScan();
			}

		}
	}
	
	public void onScanStarted(){
	}
	
	public void onScanFinished(bool fail) {
		if (state == State.SEARCHING) {
			//triedDiceSet.Clear();
			if (connectedDice.Count >= minDiceCount) {
				setState(State.CONNECTED);
				DicePlusAnimator.Instance.hideConnector(true, 0.25f);
			} else {
				OrbitingButtonsManager.Instance.showButtons(true);
				setState(State.DISCONNECTED);
			}
		}
	}
	
	void Update() {
		checkConnection();
	}
	
	DicePlus currentlyConnectedDie = null;

	private void checkConnection() {
		if (state == State.SEARCHING) {
			if (rssiSearchTimoutTime < Time.time && diceSet.Count > 0) {
				setState(State.DISCONNECTED);
				DicePlusConnector.Instance.stopScan();
			}
		}
		if (state == State.DISCONNECTED) {
			if (diceSet.Count == 0) {
				if (scanningEnabled) {
					if (DicePlusConnector.Instance.startScan()) {
						rssiSearchTimoutTime = Time.time + findClosestTime;
						setState(State.SEARCHING);
					} else {
						OrbitingButtonsManager.Instance.showButtons(true);
						setState(State.DISCONNECTED);
					}
				}
			} else {
				int highestRssi = int.MinValue;
				DicePlus toBeConnected = null;
				foreach(DicePlus dp in diceSet) {
					if (dp.rssi > highestRssi) {
						highestRssi = dp.rssi;
						toBeConnected = dp;
					}
				}
				if (toBeConnected != null) {
					currentlyConnectedDie = toBeConnected;
					DicePlusConnector.Instance.connect(toBeConnected);
					setState(State.CONNECTING);
					diceSet.Remove(toBeConnected);
				}
			}
		}
	}
	
	int battery = 100;
	public override void onBatteryState(DicePlus dicePlus, DicePlusConnector.BatteryState batteryState, int percentage, bool low, string errorMsg) {
		if (percentage < battery && low) {
			if (state != State.CONNECTED) {
				DicePlusAnimator.Instance.runLowBatteryAnimation();
			} else {
				StartCoroutine(DicePlusAnimator.Instance.lowBatteryCorutine());
			}
			battery = percentage;
		}
	}
	
	/// <summary>
	/// Minimal major DICE+ software version game requires
	/// </summary>
	public int major = 1;
	/// <summary>
	/// Minimal minor DICE+ software version game requires
	/// </summary>
	public int minor = 0;

	
	public void onConnectionEstablished(DicePlus dpe) {
		if (dpe.Equals(currentlyConnectedDie)) {
			currentlyConnectedDie = null;
		}
		
		SoftwareVersion sv = dpe.getSoftwareVersion();
		if ((sv.getMajor() > major) || (sv.getMajor() == major && sv.getMinor() >= minor)) {
			connectedDice.Add(dpe);
			if (connectedDice.Count >= maxDiceCount) {
				setState(State.CONNECTED);
			} else {
				setState(State.DISCONNECTED);
			}
			DicePlusAnimator.Instance.runConnectedAnimation();
			dpe.subscribeRolls();
			dpe.subscribeBatteryState();
			dpe.registerListener(this);
			rssi = dpe.rssi;
		} else {
			setState(State.DISCONNECTED);
			DicePlusAnimator.Instance.runVersionMissmatchAnimation(false);
			dpe.disconnect();
		}
	}
	
	public void onConnectionLost(DicePlus dpl) {
		
		handleConnectionError(dpl);
	}
	
	public void onConnectionFailed(DicePlus dpf, int errorCode, string excpMsg) {
		if (errorCode == DicePlusConnector.ERR_VERSION_MISSMATCH) {
			DicePlusAnimator.Instance.runVersionMissmatchAnimation(true);
		}
		handleConnectionError(dpf);
	}
	
	void handleConnectionError(DicePlus dpf) {
		if (dpf.Equals(currentlyConnectedDie)) {
			if (state == State.CONNECTING) {
				setState(State.DISCONNECTED);
			}
			currentlyConnectedDie = null;
		}
		if (connectedDice.Contains(dpf)) {
			connectedDice.Remove(dpf);
		}
		if (state == State.CONNECTED) {
			if (connectedDice.Count < maxDiceCount) {
				DicePlusAnimator.Instance.runDisconnectedAnimation();
				setState(State.DISCONNECTED);
			}
		} else if (state != State.ROLLER) {
			OrbitingButtonsManager.Instance.showButtons(true);
//			triedDiceSet.Add(dpf);
		}
	}
}
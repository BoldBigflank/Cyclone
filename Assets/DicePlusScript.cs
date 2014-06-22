using UnityEngine;
using System.Collections;

public class DicePlusScript : EmptyDicePlusListener, IDicePlusConnectorListener {
	public GUIText RollResult;
	GameObject player;
	float colorTimer;

	// Use this for initialization
	void Start () {
		DicePlusConnector.Instance.registerListener(this);
		player = GameObject.FindGameObjectWithTag("Player");
		colorTimer = 0.0F;
	}
	
	// Update is called once per frame
	void Update () {
		colorTimer -= Time.deltaTime;
	}

	public override void onRoll(DicePlus dicePlus, long time, int duration, int face, int invalidityFlags, string errorMsg){
		// Accepting only valid rolls.
		if(invalidityFlags == 0) {
			RollResult.text = face.ToString();
		}
	}

	public override void onOrientationReadout(DicePlus dicePlus, long time, Vector3 v, string errorMsg){
		// X axis goes through 1
		// Y axis goes through 5
		// Z axis goes through 3

		player.GetComponent<MoveAround>().SetYaw(v.x);
		if(colorTimer <= 0.0F){
			dicePlus.runFadeAnimation(DicePlusConnector.LedFace.LED_ALL, 0, GameController.newColor, 150, 500);
			colorTimer = 1.0F;
		}

	}

	public override void onLedState(DicePlus dicePlus, long time, DicePlusConnector.LedFace ledMask, long animationId, int type, string errorMsg){
//		Debug.Log ("onLedState" + ledMask);
		if(ledMask == DicePlusConnector.LedFace.LED_ALL){
//			dicePlus.runFadeAnimation(DicePlusConnector.LedFace.LED_ALL, 0, GameController.newColor, 50, 900);
		}
	}

	public void onNewDie (DicePlus dicePlus)
	{
//		throw new System.NotImplementedException ();
	}

	public void onScanFinished (bool fail)
	{
//		throw new System.NotImplementedException ();
	}

	public void onScanStarted ()
	{
//		throw new System.NotImplementedException ();
	}

	public void onConnectionLost (DicePlus dicePlus)
	{
		dicePlus.unsubscribeOrientationReadouts();
		GameController.dicePlusConnected = false;
	}

	public void onConnectionEstablished (DicePlus dicePlus)
	{
		GameController.dicePlusConnected = true;
		dicePlus.registerListener(this);
		dicePlus.subscribeOrientationReadouts(20);
//		dicePlus.subscribeLedState();
		// TODO: Subscribe to taps to start a new game

	}

	public void onConnectionFailed (DicePlus dicePlus, int errorCode, string excpMsg)
	{
//		throw new System.NotImplementedException ();
	}

	public void onBluetoothStateChanged (DicePlusConnector.BluetoothState state)
	{
//		throw new System.NotImplementedException ();
	}
}

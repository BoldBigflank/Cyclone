using UnityEngine;

public class EmptyDicePlusConnectorListener : MonoBehaviour, IDicePlusConnectorListener
{
	public virtual void onNewDie(DicePlus dicePlus) {
	}
	public virtual void onScanFinished(bool fail) {
	}
	public virtual void onScanStarted() {
	}
	public virtual void onConnectionLost(DicePlus dicePlus) {
	}
	public virtual void onConnectionEstablished(DicePlus dicePlus) {
	}
	public virtual void onConnectionFailed(DicePlus dicePlus, int errorCode, string excpMsg)   {
	}
	public virtual void onBluetoothStateChanged(DicePlusConnector.BluetoothState state) {
	}
}



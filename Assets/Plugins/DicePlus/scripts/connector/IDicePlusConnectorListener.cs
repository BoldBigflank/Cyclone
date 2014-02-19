/// <summary>
/// Listener to connection related events
/// </summary>
public interface IDicePlusConnectorListener
{
	/// <summary>
	/// New die was found
	/// </summary>
	/// <param name='dicePlus'>
	/// found dice
	/// </param>
	void onNewDie(DicePlus dicePlus);
	/// <summary>
	/// Called when scan stopped
	/// </summary>
	/// <param name='fail'>
	/// true on scan failure, false otherwise
	/// </param>
	void onScanFinished(bool fail);
	/// <summary>
	/// Called when scan started
	/// </summary>
	void onScanStarted();
	/// <summary>
	/// Called when connection has been lost
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	void onConnectionLost(DicePlus dicePlus);
	/// <summary>
	/// Called when connection has been estabilished (both connected and authorized)
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	void onConnectionEstablished(DicePlus dicePlus);
	/// <summary>
	/// Called when connection process failed
	/// </summary>
	/// <param name='dicePlus'>
	/// the die to which to send command/request
	/// </param>
	/// <param name='excpMsg'>
	/// exception message
	/// </param>
	void onConnectionFailed(DicePlus dicePlus, int errorCode, string excpMsg);
	/// <summary>
	/// Called when bluetooth adapter state changes
	/// </summary>
	/// <param name='dicePlus'>
	/// is bluetooth adapter ready
	/// </param>
	void onBluetoothStateChanged(DicePlusConnector.BluetoothState state);
}



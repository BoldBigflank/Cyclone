/// <summary>
/// Listener to widget generated events 
/// </summary>
public interface WidgetEventListener
{
	/// <summary>
	/// Called when info window is hidden/shown
	/// </summary>
	/// <param name='enabled'>
	/// new info window state, true if enabled, false otherwise
	/// </param>
	void onInfoWindowStateChange(bool enabled);
	/// <summary>
	/// Called when user started dragging widget
	/// </summary>
	void onWidgetDraggingStart();
	/// <summary>
	/// Called when user ended dragging widget
	/// </summary>
	void onWidgetDraggingEnd();
	/// <summary>
	/// Called when widget is hidden/shown
	/// </summary>
	/// <param name='hidden'>
	/// new widget state, true if enabled, false otherwise
	/// </param>
	void onWidgetStateChange(bool hidden);
	/// <summary>
	/// Called when roller is enabled/disabled
	/// </summary>
	/// <param name='hidden'>
	/// new roller state, true if enabled, false otherwise
	/// </param>
	void onRollerStateChange(bool isEnabled);
}

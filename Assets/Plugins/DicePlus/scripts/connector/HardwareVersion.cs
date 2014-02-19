/// <summary>
/// Dice hardware version data
/// </summary>
public class HardwareVersion {
	private int major;
	private int minor;
	
	public HardwareVersion(int major, int minor) {
		this.major = major;
		this.minor = minor;
	}
	/// <summary>
	/// Gets the major version number
	/// </summary>
	/// <returns>
	/// major version number
	/// </returns>
	public int getMajor() {
		return major;
	}
	/// <summary>
	/// Gets the minor version number
	/// </summary>
	/// <returns>
	/// minor version number
	/// </returns>
	public int getMinor() {
		return minor;
	}
	
	public override string ToString() {
		return ""+major+"."+minor;
	}
	/// <summary>
	/// Creates object with provided formated string
	/// </summary>
	/// <param name='str'>
	/// formated string
	/// </param>
	public static HardwareVersion Parse(string str) {
		string[] words = str.Split('.');
		int major = 0;
		int minor = 0;
		if (words.Length == 2 && int.TryParse(words[0], out major) && int.TryParse(words[1], out minor)) {
			return new HardwareVersion(major, minor);			
		} else {
			return null;
		}
	}
}
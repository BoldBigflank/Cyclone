/// <summary>
/// Dice software version data
/// </summary>
public class SoftwareVersion {
	private int major;
	private int minor;
	private int build;
	
	public SoftwareVersion(int major, int minor, int build) {
		this.major = major;
		this.minor = minor;
		this.build = build;
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
	/// <summary>
	/// Gets the build number
	/// </summary>
	/// <returns>
	/// build number
	/// </returns>
	public int getBuild() {
		return build;
	}
	
	public override string ToString() {
		return ""+major+"."+minor+"."+build;
	}
	
	/// <summary>
	/// Creates object with provided formated string
	/// </summary>
	/// <param name='str'>
	/// formated string
	/// </param>
	public static SoftwareVersion Parse(string str) {
		string[] words = str.Split('.');
		int major = 0;
		int minor = 0;
		int build = 0;
		if (words.Length == 3 && int.TryParse(words[0], out major) && int.TryParse(words[1], out minor) && int.TryParse(words[2], out build)) {
			return new SoftwareVersion(major, minor, build);			
		} else {
			return null;
		}
	}
}
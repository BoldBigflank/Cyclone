using UnityEngine;

/// <summary>
/// Persistent storage record description
/// </summary>
public class PStorageRecordDescription
{
	/// <summary>
	/// Persistent storage value type
	/// </summary>
	public enum Type {
		/// <summary>
		/// integer type
		/// </summary>
		INTEGER = 1,
		/// <summary>
		/// vector type
		/// </summary>
		VECTOR3 = 2,
		/// <summary>
		/// string 32 type
		/// </summary>
		STRING32 = 3,
	};
	
	/// <summary>
	/// Flags.
	/// </summary>
	public class Flags {
		/// <summary>
		/// readable
		/// </summary>
		public static int PS_FLAGS_RD =                        (1 << 0);
		/// <summary>
		/// writable
		/// </summary>
		public static int PS_FLAGS_WR =                        (1 << 1);
		/// <summary>
		/// default value present
		/// </summary>
		public static int PS_FLAGS_DEF =                       (1 << 2);
		/// <summary>
		/// lower limiting value present
		/// </summary>
		public static int PS_FLAGS_LLIMIT =                    (1 << 3);
		/// <summary>
		/// upper limiting value present
		/// </summary>
		public static int PS_FLAGS_ULIMIT =                    (1 << 4);
	}

	public PStorageRecordDescription(int handle, Type type, int flags, string name,
			string description, string unit, int minValue,
			int maxValue, object defValue) {
		this.handle = handle;
		this.type = type;
		this.flags = flags;
		this.name = name;
		this.description = description;
		this.unit = unit;
		this.minValue = minValue;
		this.maxValue = maxValue;
		this.defValue = defValue;
	}
	
	/// <summary>
	/// Returns handle to record
	/// </summary>
	/// <returns>
	/// handle to record
	/// </returns>
	public int getHandle() {
		return handle;
	}
	
	/// <summary>
	/// Returns type of record
	/// </summary>
	/// <returns>
	/// type of record
	/// </returns>
	public Type getType() {
		return type;
	}
	
	/// <summary>
	/// Returns record name
	/// </summary>
	/// <returns>
	/// record name
	/// </returns>
	public string getName() {
		return name;
	}
	
	/// <summary>
	/// Returns record description
	/// </summary>
	/// <returns>
	/// record description
	/// </returns>
	public string getDescription() {
		return description;
	}

	/// <summary>
	/// Returns unit of record value
	/// </summary>
	/// <returns>
	/// unit of record value
	/// </returns>
	public string getUnit() {
		return unit;
	}
	
	/// <summary>
	/// Returns lower limit for record value
	/// </summary>
	/// <returns>
	/// lower limit for record value
	/// </returns>
	public int getMinValue() {
		return minValue;
	}
	
	/// <summary>
	/// Returns upper limit for record value
	/// </summary>
	/// <returns>
	/// upper limit for record value
	/// </returns>
	public int getMaxValue() {
		return maxValue;
	}
	
	/// <summary>
	/// Returns default record value
	/// </summary>
	/// <returns>
	/// default record value
	/// </returns>
	public object getDefValue() {
		return defValue;
	}

	private int handle;
	private Type type;
	private int flags;
	/// <summary>
	/// Returns records flag bitmask
	/// </summary>
	/// <returns>
	/// records flag bitmask 
	/// </returns>
	public int getFlags() {
		return flags;
	}

	private string name;
	private string description;
	private string unit;
	
	private int minValue;
	private int maxValue;
	
	private object defValue;
	
	/// <summary>
	/// Returns whether record value is writable
	/// </summary>
	/// <returns>
	/// is record value writable
	/// </returns>
	public bool isWritable() {
		return (flags & Flags.PS_FLAGS_WR) != 0;
	}
	
	/// <summary>
	/// Returns whether record has upper value limit
	/// </summary>
	/// <returns>
	/// has upper value limit
	/// </returns>
	public bool hasUpperLimit() {
		return (flags & Flags.PS_FLAGS_ULIMIT) != 0;
	}
	
	/// <summary>
	/// Returns whether record has lower value limit
	/// </summary>
	/// <returns>
	/// has lower value limit
	/// </returns>
	public bool hasLowerLimit() {
		return (flags & Flags.PS_FLAGS_LLIMIT) != 0;
	}
	
	/// <summary>
	/// Returns whether record value is readable
	/// </summary>
	/// <returns>
	/// is record value readable
	/// </returns>
	public bool isReadable() {
		return (flags & Flags.PS_FLAGS_RD) != 0;
	}
	
	/// <summary>
	/// Returns whether record has default value
	/// </summary>
	/// <returns>
	/// has default value
	/// </returns>
	public bool hasDefaultValue() {
		return (flags & Flags.PS_FLAGS_DEF) != 0;
	}
	/// <summary>
	/// Creates new value object initialized with provided string 
	/// </summary>
	/// <returns>
	/// true if parse was succesful, false otherwise
	/// </returns>
	/// <param name='str'>
	/// value 
	/// </param>
	/// <param name='retVal'>
	/// output value - initialized object, null if provided string was invalid
	/// </param>
	public static bool TryParse(string str, out PStorageRecordDescription retVal) {
		char [] delimiters = new char [] {'#'};
		string [] words = str.Split(delimiters, 9);
		if (words.Length == 9) {
			try {
				Type ntype = (Type)int.Parse(words[1]);
				object nobject = null;
				switch (ntype) {
				case Type.INTEGER :
					nobject = int.Parse(words[8]);
					break;
				case Type.VECTOR3 :
					string [] nums = words[8].Split(',');
					nobject = new Vector3(int.Parse(nums[0]), int.Parse(nums[1]), int.Parse(nums[2]));
					break;
				case Type.STRING32 :
					nobject = words[8];
					break;
				};
				retVal = new PStorageRecordDescription(int.Parse(words[0]), ntype, int.Parse(words[2]), words[3], words[4], words[5], int.Parse(words[6]), int.Parse(words[7]), nobject);
				return true;
			} catch {
				retVal = null;
				return false;
			}
		} else {
			retVal = null;
			return false;
		}
		
	}
	
	public override string ToString() 
	{
	     return name + " " + unit + " " + description + " " + type + " " + handle + " " + flags + " " + minValue + " " + maxValue + " " + defValue;
	}
}


using System.Collections.Generic;

/// <summary>
/// Agregates all statistic data about dice
/// </summary>
public class DiceStatistics
{
	/** Number of valid rolls */
	public long validRolls;

	/** Total number of rolls */
	public long totalRolls;

	/** Number of valid rolls per face */
	public List<long> rolls;

	/** connected time in seconds */
	public long connectedTime;
	/** authorization counter */
	public long timesAuthorized;
	/** overall roll time in milliseconds */
	public long rollTime;
	/** number of charging cycles */
	public long chargingCycles;
	/** total charging time */
	public long chargingTime;
	/** number of times dice was activated */
	public long wakeupCount;

	public DiceStatistics() {
	}

	public DiceStatistics (long validRolls, long totalRolls, List<long> rolls, long connectedTime, long timesAuthorized, long rollTime, long chargingCycles, long chargingTime, long wakeupCount)
	{
		this.validRolls = validRolls;
		this.totalRolls = totalRolls;
		this.rolls = rolls;
		this.connectedTime = connectedTime;
		this.timesAuthorized = timesAuthorized;
		this.rollTime = rollTime;
		this.chargingCycles = chargingCycles;
		this.chargingTime = chargingTime;
		this.wakeupCount = wakeupCount;
	}
	
}



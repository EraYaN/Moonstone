package javaBot.Nano.Rescue;

import com.muvium.apt.PeriodicTimer;

/**
 * Takes a string as input parameter.
 * Each value sets the L and R servo and waits for a certain number of cycles.
 * The table is defined in UVMDemo.java
 */	

public class TestBehavior extends Behavior
{
	private int 	state = 0;
	private int		index;
	private BaseController	joBot;
	private int		vl = 0;
	private int		vr = 0;
	private int		vc = 0;
	private int		count = 0;

	/**
	 * This table defines a series of movements the robot must
	 * execute repeatedly.
	 * When the time is set to 0, execution stops.
	 * When the last entry is nonzero, the table is restarted.
	 */
	private String testMacro = 
	// Time indicated in 1/10 sec
	//							  Left  Right Time
		"\u0064\u0064\u0010" + // 100 , 100 , 16 
		"\u0064\u0032\u0009" + // 100 ,  50 ,  9 
		"\u0064\u0064\u0009" + // 100 ,  80 ,  9 
		"\u0000\u0000\u0000";  //   0 ,   0 ,  0   Stop
	
	public TestBehavior(BaseController initJoBot, PeriodicTimer tick,
			int servicePeriod)
	{
		super(initJoBot, tick, servicePeriod);
		joBot = initJoBot;
	}

	public void doBehavior() {
		if (state == 0) {
			vl = (byte) testMacro.charAt(index++);
			vr = (byte) testMacro.charAt(index++);
			vc = (byte) testMacro.charAt(index++); 
			System.out.println("vl=" + String.valueOf(vl) + 
					           " vr=" + String.valueOf(vr) + 
					           " vc=" + String.valueOf(vc));
			if (index >= testMacro.length())
				index = 0;
			state = 1;
			count = 0;
		}
		
		if (state == 1) {
			joBot.drive(vl, vr);
			if (vc == 0)
				return;
			if (count++ >= vc)
				state = 0;
		}		
	}
}

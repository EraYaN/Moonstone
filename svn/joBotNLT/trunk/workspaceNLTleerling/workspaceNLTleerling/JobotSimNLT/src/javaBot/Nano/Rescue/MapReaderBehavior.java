/*
 * Created on Aug 13, 2004
 * The MapReaderbehavior (dip=6-7) class sits on the base behavior class
 * @author James Caska
 */
package javaBot.Nano.Rescue;

import com.muvium.apt.PeriodicTimer;

/**
 * Lesson - 4 (verdieping)
 * This behavior (DIP=3) uses a hexedecimal representation of the 
 * X,Y and time values to drive the robot behavior.
 * This is called a macro command and greatly simplifies
 * the programming of movements.
 * It takes a string as input parameter.
 * Each value sets the L and R servo and waits for a certain number of cycles.
 */	
public class MapReaderBehavior extends Behavior
{
	private int 	state = 0;
	private String	map;
	private int		index;
	private BaseController	joBot;
	private int		vl = 0;
	private int		vr = 0;
	private int		vc = 0;
	private int		count = 0;

	public MapReaderBehavior(BaseController initJoBot, PeriodicTimer tick,
			int servicePeriod, String initMap)
	{
		super(initJoBot, tick, servicePeriod);
		map = initMap;
		joBot = initJoBot;
	}

	public void doBehavior() {
		if (state == 0) {
			vl = (byte) map.charAt(index++);
			vr = (byte) map.charAt(index++);
			vc = (byte) map.charAt(index++); 
			System.out.println("vl=" + String.valueOf(vl) + 
					           " vr=" + String.valueOf(vr) + 
					           " vc=" + String.valueOf(vc));
			if (index >= map.length())
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

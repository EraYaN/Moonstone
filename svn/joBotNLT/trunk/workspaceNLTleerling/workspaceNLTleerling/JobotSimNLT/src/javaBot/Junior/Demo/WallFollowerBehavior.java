/*
 * Created on Aug 13, 2004
 * Copyright: (c) 2006
 * Company: Dancing Bear Software
 *
 * @version $Revision: 1.1 $
 *
 * The WallHugbehavior (8) class sits on the Behavior base class
 * WallHug tries to keep the robot at a fixed distance from the wall
 * It uses just one sensor for this.
 * An improved version of this behavior could use all sensors and
 * first try to find out which side is closest to the wall
 *
 * The WallHug behavior is a state machine that tries to keep the robot
 * at a fixed distance from the wall, using only two sensors
 */
package javaBot.Junior.Demo;

import com.muvium.apt.PeriodicTimer;

/**
 * WallHugBehavior is a simple wall follower using one sensor.
 * We read the sensors facing the wall and keep the distance to the wall equal.
 */

public class WallFollowerBehavior extends Behavior
{
	private BaseController	joBot;
	private int WALL_DIST = 200;
	
	/**
	 * Creates a new WallHugBehavior object.
	 *
	 * @param initJoBot TODO PARAM: DOCUMENT ME!
	 * @param initServiceTick TODO PARAM: DOCUMENT ME!
	 * @param servicePeriod TODO PARAM: DOCUMENT ME!
	 */
	public WallFollowerBehavior(BaseController initJoBot, PeriodicTimer initServiceTick,
			int servicePeriod)
	{
		super(initJoBot, initServiceTick, servicePeriod);
		joBot = initJoBot;	}

	/**
	 * TODO METHOD: DOCUMENT ME!
	 */
	public void doBehavior()
	{
		int sl = 0;
		int sr = 0;
		int diff = 0;
		int speed = 0;

		// Find minumum distance
		sl = joBot.getSensorValue(0);
		sr = joBot.getSensorValue(1);
		joBot.setStatusLeds(false, false, false);
		diff = sr * 100 / WALL_DIST;
		speed = 100 - (diff * 50 / 100);
//		System.out.print("Diff=");
//		System.out.println(diff);

		if (sr < WALL_DIST) {
			joBot.setLed(BaseController.LED_GREEN, true);
			joBot.drive(100, 75);	// Go left
		}
		
		if (sr >= WALL_DIST) {
			joBot.setLed(BaseController.LED_YELLOW, true);
			joBot.drive(speed, 100);	// Go right
		}

	}
}

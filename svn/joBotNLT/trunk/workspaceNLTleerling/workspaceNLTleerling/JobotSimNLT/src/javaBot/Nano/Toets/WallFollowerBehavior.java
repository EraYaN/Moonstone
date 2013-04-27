package javaBot.Nano.Toets;

import com.muvium.apt.PeriodicTimer;

/**
 * Opdracht - 10 Basis wall follower
 * WallFollowerBehavior is a simple wall follower using one sensor.
 * We read the sensors facing the wall and keep the distance 
 * to the wall the same.
 */

public class WallFollowerBehavior extends Behavior
{
	private BaseController	joBot;
	private int WALL_DIST = 20;
	private int speed = 80;	

	public WallFollowerBehavior(BaseController initJoBot, PeriodicTimer initServiceTick,
			int servicePeriod)
	{
		super(initJoBot, initServiceTick, servicePeriod);
		joBot = initJoBot;
		}

	public void doBehavior()
	{
		int ds = 0;
		int diff = 0;

		// Find minumum distance
		ds = joBot.getSensorValue(BaseController.SENSOR_DS);
		joBot.setStatusLeds(false, false, false);

		if (ds < WALL_DIST) {   // If wall lost
			joBot.setLed(BaseController.LED_GREEN, true);
			joBot.drive(speed, speed/2);	// Go left
		}
		
		if (ds >= WALL_DIST) {    // If wall found
			joBot.setLed(BaseController.LED_YELLOW, true);
			joBot.drive(speed/4, speed);	// Go right
		}

	}
}

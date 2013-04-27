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
 * LineFollowerBehavior is a simple line follower using two sensors.
 */

public class LineFollowerBehavior extends Behavior
{
	private BaseController	joBot;
	int state = 0;
	int count = 0;
	int sl = 0;
	int sr = 0;
	int speed = 50;
	
	public LineFollowerBehavior(BaseController initJoBot, PeriodicTimer initServiceTick,
			int servicePeriod)
	{
		super(initJoBot, initServiceTick, servicePeriod);
		joBot = initJoBot;	}

	public void doBehavior()
	{

		if (state == 0) {
			System.out.println("Line Follower Demo");
			joBot.drive(speed, speed);
			state = 1;
		}

		if (state == 1) {	
			sl = lineFollower(speed, 150, 300, 0, 0);
			if (sl > 0) {
				System.out.print("Found yellow = ");
				System.out.println(sl);
				joBot.tone(1);
				state = 2;
			}
		}
		
		if (state == 2) {
			joBot.drive(speed/2, speed);
			count++;
			if (count == 30) {
				System.out.print("Took corner");
				System.out.println();
				joBot.tone(2);
				count = 0;
				state = 3;
			}
		}
		
		if (state == 3) {
			sl = lineFollower(speed, 600, 900, 150, 250);
//			sl = lineFollower(speed, 600, 900, 0, 0, 0, 0);
			if (sl > 0) {
				System.out.print("Found black = ");
				System.out.println(sl);
				joBot.tone(3);
				state = 4;
			}
		}
		
		if (state == 4) {
			joBot.drive(speed, speed);
			count++;
			if (count == 30) {
				System.out.print("Jump yellow end");
				System.out.println();
				joBot.tone(4);
				count = 0;
				state = 5;
			}
		}
		
		if (state == 5) {
			sl = lineFollower(speed, 150, 300, 900, 1000);
			if (sl > 0) {
				System.out.print("Found yellow = ");
				System.out.println(sl);
				joBot.tone(5);
				state = 6;
			}
		}

		if (state == 6) {
			joBot.drive(0,0);
		}
			
	}

	private int lineFollower(int speed, int minRd, int maxRd, 
										int minStR, int maxStR) {
		//	int minStL, int maxStL,
		sl = joBot.getSensorValue(2);
		sr = joBot.getSensorValue(3);
		joBot.setStatusLeds(false, false, false);

		if (sl >= minRd && sl <= maxRd) {
			joBot.drive(0, speed);	// Go left
			joBot.setLed(BaseController.LED_GREEN, true);
		}

		if (sr >= minRd && sr <= maxRd) {
			joBot.drive(speed, 0);	// Go right
			joBot.setLed(BaseController.LED_YELLOW, true);
		}
		
//		if (sl >= minStL && sl <= maxStL)
//			return sl;
//		
//		if (sr >= minStR && sr <= maxStR)
//			return sr;
//		
		return 0;
	}
}

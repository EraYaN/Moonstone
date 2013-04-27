package javaBot.Nano.Toets;

import com.muvium.apt.PeriodicTimer;

/**
 * The LineFollowerBehavior uses the reflection sensors to detect
 * the color of the surface under the robot.
 * It will follow the black line, first with one sensor.
 */

public class LineFollowerBehavior extends Behavior
{
	private BaseController	joBot;
	private int	state	= 0;
//	private int subState = 0;
	private int speed	= 50;
	private int sl	    = 0;
	private int sr		= 0;
	// Default values for simulator	
	private int lYel	= 900;   // 
	private int rYel	= 900;
	private int lGrn	= 400;
	private int rGrn	= 400;
	private int lBlk	= 270;
	private int rBlk	= 270;
	

	public LineFollowerBehavior(BaseController initJoBot, PeriodicTimer
			initServiceTick,int servicePeriod)
	{
		super(initJoBot, initServiceTick, servicePeriod);
		joBot = initJoBot;
	}
	
	public void doBehavior() {
		if (state == 0) {
			System.out.println("Test Line Follower");
			joBot.setFieldLeds(true);
			// Read calibration data if calibrated, else use defaults
			if (joBot.store.peekInt16(BaseController.STORE_L_YEL) > 500) {
				lYel = joBot.store.peekInt16(BaseController.STORE_L_YEL);
				rYel = joBot.store.peekInt16(BaseController.STORE_R_YEL);
				lGrn = joBot.store.peekInt16(BaseController.STORE_L_GRN);
				rGrn = joBot.store.peekInt16(BaseController.STORE_R_GRN);
				lBlk = joBot.store.peekInt16(BaseController.STORE_L_BLK);
				rBlk = joBot.store.peekInt16(BaseController.STORE_R_BLK);
			} else {
				System.out.println("Using defaults");
				joBot.printLCD("No Calib");
			}
			
			System.out.print(" YL=" + Integer.toString(lYel));
			System.out.print(" YR=" + Integer.toString(rYel));
			System.out.print(" GL=" + Integer.toString(lGrn));
			System.out.print(" GR=" + Integer.toString(rGrn));
			System.out.print(" BL=" + Integer.toString(lBlk));
			System.out.print(" BR=" + Integer.toString(rBlk));
			System.out.println();
			state = 1;
		}

		if (state == 1) {
				sl = lineFollowerBlack(speed, lBlk, rBlk, lYel, 0);
				if (sl > 0) {
					System.out.print("Found yellow = ");
					System.out.println(sl);
					joBot.tone(1);
					state = 2;
				}
			}

		if (state == 2) {
			joBot.drive(0, 0);
		}
	}

/**
 * The LineFollower gets as parameters the value of the left and right
 * line color. It also get the value of the left and right stop color.
 * It returns the value of the stop condition.
 */
	
	private int lineFollowerBlack(int speed, int lineLd, int lineRd, 
			int stopLd,	int stopRd) {
		joBot.setStatusLeds(false, false, false);
		joBot.drive(speed, speed);
		sl = joBot.getSensorValue(BaseController.SENSOR_FL);
		sr = joBot.getSensorValue(BaseController.SENSOR_FR);
		
		if (sl < lineLd) {
			joBot.drive(0, speed); // Go Left
			joBot.setLed(BaseController.LED_GREEN, true);
		}

		if (sr < lineRd) {
			joBot.drive(speed, 0); // Go Right
			joBot.setLed(BaseController.LED_YELLOW, true);
		}

		if ((stopLd > 0) && (sl >= stopLd)) {
			joBot.setLed(BaseController.LED_BLUE, true);
			return sl;
		}
		
		if ((stopRd > 0 ) && (sr >= stopRd)) {
			joBot.setLed(BaseController.LED_BLUE, true);
			return sr;
		}
		
		return 0;
	}
	
}



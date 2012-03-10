package javaBot.Nano.Rescue;

import com.muvium.apt.PeriodicTimer;

/**
 * Opdracht - 6 Eenvoudige Line Follower met enkele sensor
 * 
 * De lijnvolger gebruikt de reflectiesensoren (fieldsensoren)
 * om de kleur van het veld onder de robot te bepalen.
 * Hij volgt de zwarte lijn, eerst met een enkele sensor.
 * In opdracht 7 ga je de lijn met twee sensoren volgen.
 * Daarnaast ga je ook een subroutine gebruiken.
 * Als laatste ga je ook de gele lijn volgen.
 */

public class LineFollowerBehaviorRobin extends Behavior
{
	private BaseController	joBot;
	private int	state	= 0;
	private int speed	= 30;
	private int sensorFL	    = 0;
	private int sensorFR = 0;
	private int valueBlack	= 400;   // Value of black on your field
	private int valueYellow = 900; // Value of yellow on your field
	private int stateLeft = 0;
	private int stateRight = 0;
	private int previousStateLeft = 0;
	private int previousStateRight = 0;

	public LineFollowerBehaviorRobin(BaseController initJoBot, PeriodicTimer
			initServiceTick,int servicePeriod)
	{
		super(initJoBot, initServiceTick, servicePeriod);
		joBot = initJoBot;
	}

	// Maak een eenvoudige lijnvolger met een enkele sensor
	// Opdracht 6
	
	public void doBehavior() {
		if (state == 0) {
			System.out.println("Line Follower by Erwin en Robin");
			joBot.setStatusLeds(false, false, false);
			joBot.drive(speed, speed);	// Rijd rechtuit
			joBot.setFieldLeds(true);	// Zet field leds aan
			state = 1;
		}

		if (state == 1) {	
			sensorFL = joBot.getSensorValue(BaseController.SENSOR_FL); // Links sensor
			sensorFR = joBot.getSensorValue(BaseController.SENSOR_FR); // Rechts sensor
		

			if (sensorFL >= valueBlack || sensorFR < valueBlack) {
				joBot.drive(speed, speed/-2);	// Go right
				joBot.setLed(BaseController.LED_GREEN, true);
				stateLeft = 1;
				
			}else{
				stateLeft=0;
			}
				
			if (sensorFR >= valueBlack || sensorFL < valueBlack) {
				joBot.drive(speed/-2, speed);	// Go left
				joBot.setLed(BaseController.LED_GREEN, true);
				stateRight = 1;
			}else{
				stateRight =0;
			}
			
			if(stateLeft + stateRight > 1){
				joBot.drive(speed*2, speed*2);
				joBot.setLed(BaseController.LED_GREEN, false);
				joBot.setLed(BaseController.LED_RED, true);
			}
			
		}	
		//reset
		previousStateLeft = stateLeft;
		previousStateRight = stateRight;
	}
}



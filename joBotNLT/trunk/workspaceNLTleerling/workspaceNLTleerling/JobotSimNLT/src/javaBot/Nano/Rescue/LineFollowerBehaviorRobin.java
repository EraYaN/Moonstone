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
	private int	state		= 0;
	private String dir		= "none";
	private String prevdir 	= "none";
	private int speedL		= 0;
	private int speedR		= 0;
	private int sensorFL	= 0;
	private int sensorFR 	= 0;
	private int valueBlack	= 400;   // Value of black on your field
	private int valueYellow = 700; // Value of yellow on your field
	
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
			System.out.println("Line Follower by Robin");
			joBot.setStatusLeds(false, false, false);
			joBot.setFieldLeds(true);	// Zet field leds aan
			state = 1;
		}

		if (state == 1) {	
			sensorFL = joBot.getSensorValue(BaseController.SENSOR_FL); // Links sensor
			sensorFR = joBot.getSensorValue(BaseController.SENSOR_FR); // Rechts sensor		

			if (sensorFL >= valueBlack && sensorFR < valueBlack) {
				speedL = 40;
				speedR = 10;
				joBot.setLed(BaseController.LED_GREEN, true);
				dir = "Right";
			}
				
			if (sensorFR >= valueBlack && sensorFL < valueBlack) {
				speedL = 10;
				speedR = 40;
				joBot.setLed(BaseController.LED_GREEN, true);
				dir = "Left";
			}
			
			if(sensorFL < valueBlack && sensorFR < valueBlack) {
				speedL = 50;
				speedR = 50;
				joBot.setLed(BaseController.LED_GREEN, false);
				joBot.setLed(BaseController.LED_RED, true);
				dir = "Forward";
			}
			
			if(sensorFR > valueBlack && sensorFL > valueBlack) {
				
				if(prevdir == "Right") {
					speedL = 30;
					speedR = -30;
				}
				
				if (prevdir == "Left") {
					speedL = -30;
					speedR = 30;
				}
				
			}
			
			if(sensorFR >= valueYellow || sensorFL >= valueYellow) {
					state = 2;
			}
		}
		
		if(state == 2) {
			
			sensorFL = joBot.getSensorValue(BaseController.SENSOR_FL); // Links sensor
			sensorFR = joBot.getSensorValue(BaseController.SENSOR_FR); // Rechts sensor		

			if (sensorFL <= valueYellow && sensorFR > valueYellow) {
				speedL = 40;
				speedR = 10;
				joBot.setLed(BaseController.LED_GREEN, true);
				dir = "Right";
			}
				
			if (sensorFR < valueYellow && sensorFL >= valueYellow) {
				speedL = 10;
				speedR = 40;
				joBot.setLed(BaseController.LED_GREEN, true);
				dir = "Left";
			}
			
			if(sensorFL >= valueYellow && sensorFR >= valueYellow) {
				speedL = 80;
				speedR = 80;
				joBot.setLed(BaseController.LED_GREEN, false);
				joBot.setLed(BaseController.LED_RED, true);
				dir = "Forward";
				
			}
			
			if(sensorFR < valueYellow && sensorFL < valueYellow) {
				
				if(prevdir == "Right") {
					speedL = 30;
					speedR = -30;
				}
				
				if (prevdir == "Left") {
					speedL = -30;
					speedR = 30;
				}
				
			}
			
			if(sensorFR <= valueBlack || sensorFL >= valueBlack) {
				state = 1;
			}
			
			if(sensorFR == 1000 && sensorFL == 1000) {
				state = 3;
			}
				
		}
		
		if(state == 3) {
			speedL = 0;
			speedR = 0;
		}
		
		//drive this shit
		joBot.drive(speedL, speedR);
		
		System.out.println(state);
		
		
		//remember previous direction
		prevdir = dir;
		
		
	}
}



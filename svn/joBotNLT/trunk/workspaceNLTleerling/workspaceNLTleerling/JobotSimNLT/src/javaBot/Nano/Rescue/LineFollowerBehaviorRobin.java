package javaBot.Nano.Rescue;

import com.muvium.apt.PeriodicTimer;

public class LineFollowerBehaviorRobin extends Behavior
{
	private BaseController	joBot;
	private int	state		= 0;
	private int prevstate	= 0;
	private String dir		= "none";
	private String prevdir 	= "none";
	private int speedL		= 0;
	private int speedR		= 0;
	private int sensorFL	= 0;
	private int sensorFR 	= 0;
	private int valueBlack	= 400;   // Value of black on your field
	private int valueYellow = 700; // Value of yellow on your field
	private long startTime	= 0;
	private long finishTime	= 0;
	private long stopTime	= 0;
	
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
			
			if(prevstate != state) {
				joBot.printLCD("Idle");
			}
			
			System.out.println("Line Follower by Robin");
			joBot.setStatusLeds(false, false, false);
			joBot.setFieldLeds(true);	// Zet field leds aan
			state = 1;
			
			
		}

		//Driving on black lines
		if (state == 1) {
			
			if(prevstate != state) {
				joBot.printLCD("Black line");
			}
			
			if(prevstate == 0) {
				startTime = System.nanoTime();
			}
			
			sensorFL = joBot.getSensorValue(BaseController.SENSOR_FL); // Links sensor
			sensorFR = joBot.getSensorValue(BaseController.SENSOR_FR); // Rechts sensor		

			if (sensorFL >= valueBlack && sensorFR < valueBlack) {
				speedL = 80;
				speedR = 20;
				joBot.setLed(BaseController.LED_GREEN, true);
				dir = "Right";
			}
				
			if (sensorFR >= valueBlack && sensorFL < valueBlack) {
				speedL = 20;
				speedR = 80;
				joBot.setLed(BaseController.LED_GREEN, true);
				dir = "Left";
			}
			
			if(sensorFL < valueBlack && sensorFR < valueBlack) {
				speedL = 70;
				speedR = 70;
				joBot.setLed(BaseController.LED_GREEN, false);
				joBot.setLed(BaseController.LED_RED, true);
				dir = "Forward";
			}
			
			if(sensorFR > valueBlack && sensorFL > valueBlack) {
				
				if(prevdir == "Right") {
					speedL = 40;
					speedR = -40;
				}
				
				if (prevdir == "Left") {
					speedL = -40;
					speedR = 40;
				}
				
			}
			
			if(sensorFR >= valueYellow || sensorFL >= valueYellow) {
					state = 2;
			}
		}
		
		//Driving on yellow lines
		if(state == 2) {
			
			if(prevstate != state) {
				joBot.printLCD("Yellow line");
			}
			
			sensorFL = joBot.getSensorValue(BaseController.SENSOR_FL); // Links sensor
			sensorFR = joBot.getSensorValue(BaseController.SENSOR_FR); // Rechts sensor		

			if (sensorFL <= valueYellow && sensorFR > valueYellow) {
				speedL = 80;
				speedR = 20;
				joBot.setLed(BaseController.LED_GREEN, true);
				dir = "Right";
			}
				
			if (sensorFR < valueYellow && sensorFL >= valueYellow) {
				speedL = 20;
				speedR = 80;
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
			
			if(sensorFR <= valueBlack || sensorFL >= valueBlack) {
				state = 1;
			}
			
			if(sensorFR >= 1000 && sensorFL >= 1000) {
				state = 3;
			}
				
		}
			
		//Stopping when arriving at the yellow plain
		if(state == 3) {
			
			if(prevstate != state) {
				joBot.printLCD("Stop");
				stopTime = System.nanoTime();
				finishTime = (stopTime - startTime)/1000000000;
				System.out.print("Finished in " + finishTime + " seconds.");
			}
			
			speedL = 0;
			speedR = 0;
		}
		
		//drive this shit
		joBot.drive(speedL, speedR);	
		
		//remember previous direction and state
		prevdir = dir;
		prevstate = state;
		
		
	}
}



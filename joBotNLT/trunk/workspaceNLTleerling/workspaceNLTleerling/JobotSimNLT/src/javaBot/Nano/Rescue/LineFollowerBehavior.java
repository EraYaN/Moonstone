package javaBot.Nano.Rescue;

import com.muvium.apt.PeriodicTimer;

public class LineFollowerBehavior extends Behavior
{
	private BaseController	joBot;
	private int	state	= 0;
	private float speed	= 50;
	private int sensorFL	    = 0;
	private int sensorFR = 0;
	private int valueBlack	= 0;   // Value of black on your field 0-400
	private int valueGreen	= 400;   // Value of green on your field 400-900
	private int valueGoal = valueGreen;
	private int valueYellow = 900; // Value of yellow on your field 900-1000
	private int stateLeft = 0;
	private int stateRight = 0;
	private int previousStateLeft = 0;
	private int previousStateRight = 0;
	private int yellowCount = 0;

	public LineFollowerBehavior(BaseController initJoBot, PeriodicTimer
			initServiceTick,int servicePeriod)
	{
		super(initJoBot, initServiceTick, servicePeriod);
		joBot = initJoBot;
	}	
	
	public void doBehavior() {
		if (state == 0) {
			System.out.println("Line Follower by Erwin");
			joBot.setStatusLeds(false, false, false);
			joBot.drive(0, 0);	// Rijd rechtuit
			joBot.setFieldLeds(true);	// Zet field leds aan
			state = 1;
		}

		if (state == 1) {	
			float speedL = speed;
			float speedR = speed;
			sensorFL = joBot.getSensorValue(BaseController.SENSOR_FL); // Links sensor
			sensorFR = joBot.getSensorValue(BaseController.SENSOR_FR); // Rechts sensor
			if((sensorFL>valueYellow||sensorFR>valueYellow)&&yellowCount==0){
				yellowCount = 1;
				valueGoal = valueYellow;
				joBot.setStatusLeds(true, false, false);
					
			}
			if((sensorFL<valueBlack||sensorFR<valueBlack)&&yellowCount==1){
				yellowCount = 2;
				valueGoal = valueBlack;					
			}
			if((sensorFL>valueYellow||sensorFR>valueYellow)&&yellowCount==2){
				yellowCount = 3;
				valueGoal = valueYellow;
				state = 2;					
			}
			if(valueGoal==valueYellow){
				sensorFL = 1000-sensorFL;	
				sensorFR = 1000-sensorFR;
			} else {
				if(sensorFR>=valueGoal && sensorFL >= valueGoal){
					if(previousStateRight==1){						
						speedL *= -0.25f;
						speedR *= 1.5;
					}else if(previousStateLeft==1){
						speedL *= 1.5;
						speedR *= -0.25f;
					}
				} else {		
					if (sensorFL >= valueGoal || sensorFR < valueGoal) {					
						// Go right
						speedL *= 1;
						speedR *= 0.25f;
						if(previousStateLeft == 1){
							speedL *= 1.25f;
						}
						if(previousStateRight == 1){
							speedR *= 2f;
						}
						
						stateLeft = 1;
						
					}else{
						stateLeft = 0;
					}
						
					if (sensorFR >= valueGoal || sensorFL < valueGoal) {
						//go left
						speedL *= 0.25f;
						speedR *= 1;
						if(previousStateRight == 1){
							speedR *= 1.25f;
						}
						if(previousStateLeft == 1){
							speedL *= 2f;
						}
						
						stateRight = 1;
					}else{
						stateRight = 0;
					}
				}
				if(sensorFR>=valueGoal && sensorFL >= valueGoal){
					if(previousStateRight==1){						
						speedL *= 0.25f;
						speedR *= 1;
					}
				}
			}
			
			if(stateLeft + stateRight > 1){
				//joBot.drive(speed*2, speed*2);
				
			}
			joBot.printLCD("SpeedL = "+speedL);
			joBot.printLCD("SpeedR = "+speedR);
			joBot.drive(Math.round(speedL), Math.round(speedR));
		}
		if (state == 2) {	
			// reached the end
			joBot.printLCD("We zijn");
			joBot.printLCD("aangekomen");
			joBot.drive(0, 0);
		}
		
		//reset
		previousStateLeft = stateLeft;
		previousStateRight = stateRight;
	}
}



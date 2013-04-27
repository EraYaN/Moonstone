package javaBot.Nano.Rescue;

import com.muvium.apt.PeriodicTimer;

public class LineFollowerBehavior extends Behavior
{
	private BaseController	joBot;
	private int	state	= 0;
	private float speed	= 60;
	private int sensorFL	    = 0;
	private int sensorFR = 0;
	private int valueBlack	= 0;   // Value of black on your field 0-400
	private int valueGreen	= 400;   // Value of green on your field 400-900
	private int valueGoal = valueGreen;
	private int valueYellow = 800; // Value of yellow on your field 700-1000
	private int stateLeft = 0;
	private int stateRight = 0;
	private int previousStateLeft = 0;
	private int previousStateRight = 0;
	private int yellowCount = 0;
	private int delayOne = 0;
	private int delayTwo = 0;
	private long start;
	private long finish;

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
			start = System.nanoTime();
		}

		if (state == 1) {	
			float speedL = speed;
			float speedR = speed;
			sensorFL = joBot.getSensorValue(BaseController.SENSOR_FL); // Links sensor
			sensorFR = joBot.getSensorValue(BaseController.SENSOR_FR); // Rechts sensor
			if(yellowCount == 1){
				delayOne++;			
			}
			if((sensorFL>valueYellow||sensorFR>valueYellow)&&yellowCount==0){
				yellowCount = 1;
				valueGoal = valueYellow;
				joBot.setStatusLeds(true, false, false);
				joBot.printLCD("Yellow 1");
				delayOne = 0;					
			}else			
			if((sensorFL<valueGreen||sensorFR<valueGreen)&&yellowCount==1&&delayOne>20){
				yellowCount = 2;
				valueGoal = valueGreen;	
				delayOne = 0;
				joBot.printLCD("Black 2");
				speedR *= 4f;
				speedL *= 0.5f;
			}else
			if((sensorFL>valueYellow||sensorFR>valueYellow)&&yellowCount==2){
				yellowCount = 3;
				valueGoal = valueYellow;
				joBot.printLCD("Yellow 2");
				state = 2;					
			}
			/*if(valueGoal==valueYellow||valueGoal == 1000-valueYellow){
				sensorFL = 1200-sensorFL;	
				sensorFR = 1200-sensorFR;
				valueGoal = 1200-valueYellow;
			}*/						
			if(sensorFR >= valueGoal && sensorFL >= valueGoal){
				if(previousStateRight==1){						
					speedL *= -0.25f;
					speedR *= 1.5f;
				}else if(previousStateLeft==1){
					speedL *= 1.5f;
					speedR *= -0.25f;
				}
			} else {	
				if (((sensorFL >= valueGoal || sensorFR < valueGoal)&&valueGoal!=valueYellow)||((sensorFL <= valueGoal || sensorFR > valueGoal)&&valueGoal==valueYellow)) {					
					// Go right
					speedL *= 1;
					speedR *= 0.5f;
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
					
				if (((sensorFR >= valueGoal || sensorFL < valueGoal)&&valueGoal!=valueYellow)||((sensorFR <= valueGoal || sensorFL > valueGoal)&&valueGoal==valueYellow)) {
					//go left
					speedL *= 0.5f;
					speedR *= 1f;
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
			
			
			if(stateLeft + stateRight > 1){
				//joBot.drive(speed*2, speed*2);
				
			}
			//joBot.printLCD("SpeedL = "+speedL);
			//joBot.printLCD("SpeedR = "+speedR);
			joBot.drive(Math.round(speedL), Math.round(speedR));
		}
		if (state == 2) {	
			// reached the end			
			if(delayTwo>5){
				joBot.drive(0, 0);
				state = 3;
				delayTwo = 0;
				joBot.printLCD("Done!");
				finish = System.nanoTime();
				float timeElapsed = ((float)finish-start)/1000000000.0f;
				joBot.printLCD(Float.toString(timeElapsed)+" s");
				
			} else {
				delayTwo++;	
				joBot.drive(Math.round(speed*1.5f), Math.round(speed*1.5f));
			}
		}
		if(state == 3){			
			joBot.drive(0, 0);
			
		}
		
		//reset
		previousStateLeft = stateLeft;
		previousStateRight = stateRight;
	}
}



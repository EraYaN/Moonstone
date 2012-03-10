package javaBot.Nano.Rescue;

import com.muvium.apt.PeriodicTimer;

/**
 * Opdracht 4A - Vooruit rijden
 * Opdracht 4B - Twee seconden lang rijden
 * Opdracht 4C - Vooruit rijden en stop na 2 sec
 * Opdracht 4D - Een bocht maken
 * Opdracht 4E - maak een subroutine
 * Opdracht 4F - Rijd in een vierkantje
 * Opdracht 5C - Rijd tot aan de zwarte lijn 
 * Opdracht 5E - Doorzoek het moeras
 **/

public class DriveBehavior05 extends Behavior {
	private BaseController joBot;
	private int state = 20;
	private int count = 0;
	int fl = 0;

	public DriveBehavior05(BaseController initJoBot,
			PeriodicTimer initServiceTick, int servicePeriod) {
		super(initJoBot, initServiceTick, servicePeriod);
		joBot = initJoBot;
	}
	public void jobotDrive(int l, int r, int t){
		joBot.drive(l,r);
		if(count++ >= t){
			count = 0;
			joBot.drive(0,0);
			state=20;
		}
	}
	public void doBehavior() {

		if (state == 10) {
			jobotDrive(50,50,10);
			joBot.setStatusLeds(false, false, false);
			if(joBot.getFieldSensorValue(BaseController.SENSOR_FL)==1000){
				joBot.setStatusLeds(false, true, false);
			}
			state=11;
		}
		
		if(state == 11){
			jobotDrive(-50,0,10);
			joBot.setStatusLeds(false, false, false);
			jobotDrive(-50,10,8);
			joBot.setStatusLeds(true, false, false);
			
		}
		
		
		
		
		
		if(state==20){
			if(joBot.getSensorValue(BaseController.SENSOR_FL)>=450){
				state=23;
			}
			if(joBot.getSensorValue(BaseController.SENSOR_FL)>(joBot.getSensorValue(BaseController.SENSOR_FR))){
				state=21;
			}
			if((joBot.getSensorValue(BaseController.SENSOR_FR))>(joBot.getSensorValue(BaseController.SENSOR_FL))){
				state=22;
			}			

			if((joBot.getSensorValue(BaseController.SENSOR_FL)<=350)&&(joBot.getSensorValue(BaseController.SENSOR_FR)<=350)){
				state=23;
			}
//			if((joBot.getSensorValue(BaseController.SENSOR_FR)<200)&&(joBot.getSensorValue(BaseController.SENSOR_FL)>=418)){
//				state=24;
//			}

		}
		
		if(state==21){
			jobotDrive(50,0,1);
		}
		
		if(state==22){
				jobotDrive(0,50,1);
		}
		
		if(state==23){
			jobotDrive(50,50,1);
			joBot.printLCD("Afsnijdweg!");
		}
		
		if(state==24){
			jobotDrive(100,0,5);
			joBot.setStatusLeds(false, false, true);
			joBot.printLCD("KAK");
		}
		
		
		
		
		
		
		
	}
}

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

public class DriveBehavior04 extends Behavior {
	private BaseController joBot;
	private int state = 10;
	private int count = 0;
	int fl = 0;

	public DriveBehavior04(BaseController initJoBot,
			PeriodicTimer initServiceTick, int servicePeriod) {
		super(initJoBot, initServiceTick, servicePeriod);
		joBot = initJoBot;
	}
	private void jobotDrive (int curState, int newState, int l, int r, int t){
		if(state==curState){
			joBot.drive(l,r);
			count++;
			if(count>=t){
				state=newState;
				count=0;
			}
		}
	}		

	public void doBehavior() {

		if (state == 0) {
			joBot.drive(100, 100);
			joBot.setStatusLeds(false, false, false); // Turn leds off
			
			count++;
			
			joBot.printLCD("State=0");
			if(count>=20){
				joBot.setStatusLeds(true, false, false);
				state=1;
				count=0;
			}}
			if(state==1){
				joBot.drive(100, 50);
				count++;
				if(count>=12){
					state=3;
				}
				joBot.printLCD("State=1");
			}
				
			
			if(state==3){
				joBot.drive(0,0);
				joBot.printLCD("State=3");
			}
			
			if(state==10){
				jobotDrive(10,11,0,100,6);				
			}
			if(state==11){
				jobotDrive(11,12,70,50,20);
			}
			if(state==12){
				jobotDrive(12,13,50,60,15);
			}
			if(state==13){
				jobotDrive(13,14,68,50,20);
			}
			if(state==14){
				jobotDrive(14,3,75,50,20);
			}
			if(count<4){
				if(state==20){
					jobotDrive(20,21,100,100,20);
				}
				if(state==21){
					jobotDrive(21,20,100,0,10);
					count++;
				}
			}
			
				
							}
			}


/*
 * Created on Aug 13, 2004 
 * The Calibratebehavior (dip=1) class sits on the base Behavior class
 * It keeps the servos to their neutral position 
 * Reads the sensors and sets the status leds to
 * indicate whisch sensor is read
 * To ease testing it reacts only to close objects
 * @author Peter van Lith
 *  
 */
package javaBot.UVM;

import com.muvium.apt.*;

public class CalibrateServoBehavior extends Behavior {
	int i = 0;
	int pos_een = 0; 
	int pos_twee = 0;
	
	public CalibrateServoBehavior(BaseController initJoBot,
			PeriodicTimer initServiceTick, int servicePeriod) {
		super(initJoBot, initServiceTick, servicePeriod);
	}

	public void doBehavior() {
		if(i == 0){
			pos_een = 0;
			pos_twee = 0;
		}
		if(i == 10){
			pos_een = 100; 
			pos_twee = 100;
		}
		if(i == 20){
			pos_een = 0;
			pos_twee = 0;
		}
		if(i == 30){
			pos_een = -100;
			pos_twee = -100;
		}
		if(i == 50){
			pos_een = 0;
			pos_twee = 0;
		}
		if (i==60)
			i = -1;
		joBot.drive(pos_een, pos_twee, 0);
		i++;
	}
}
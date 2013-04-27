package javaBot.Nano.Toets;

import com.muvium.apt.PeriodicTimer;

public class FleeBehavior extends Behavior
{
	private BaseController	joBot;
	int ds = 0;
	
	/**
	 * Opdracht - 2C Test Flee Behavior
	 * 
	 * FleeBehavior zorgt dat de robot achteruit rijdt als er 
	 * iets voor de sensor wordt gehouden
	 * 
	 */
	
	public FleeBehavior(BaseController initJoBot, PeriodicTimer initServiceTick,
			int servicePeriod)
	{
		super(initJoBot, initServiceTick, servicePeriod);
		joBot = initJoBot;	}

	public void doBehavior()
	{
		ds = joBot.getSensorValue(BaseController.SENSOR_DS);
		joBot.setStatusLeds(false, false, false);	// Turn off leds

		joBot.drive(0, 0);

		if (ds > 200) {
			joBot.setLed(BaseController.LED_GREEN, true);	// Show sensor sees something
			joBot.drive(-100, -100);	
		}
	}
}

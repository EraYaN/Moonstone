package javaBot.Nano.Rescue;

/** 
 * CuriousBeheavior is een aangepaste versie van FleeBehavior.
 * Leerlingen passen het gedrag aan in een aantal stappen:
 * 
 * Opdracht 2C - Bekijk deze code 
 * 
 * Opdracht 2D - Pas de code aan:
 * 	Zorg dat de robot naar je toe komt rijden.
 * 
 * Opdracht 2G - Zorg dat de robot niet te dichtbij komt.
 * 	Blijf op een afstand van 10 cm. Zoek uit welke waarde dat is.
 * 	De robot zelf is ongeveer 18 cm, de sensor lijn is 25 cm.
 * 	Merk op dat de code wat anders is dan in FleeBehavior.
 *  Er zijn z.g. State variabelen toegevoegd.
 *  Dit is gedaan ter voorbereiding van latere lessen.
 */

import com.muvium.apt.PeriodicTimer;

public class CuriousBehavior extends Behavior
{
	// Declaraties voor opdracht 2
	private BaseController	joBot;
	private int state = 0;
	int ds = 0;
	
	public CuriousBehavior(BaseController initJoBot, PeriodicTimer initServiceTick,
			int servicePeriod)
	{
		super(initJoBot, initServiceTick, servicePeriod);
		joBot = initJoBot;	}

	public void doBehavior()
	{		
		// Anders dan in FleeBehavior wordt hier de state getest.
		// Deze dient voor opdracht 2 op nul (0) te staan. 
		// In volgende lessen gaan we deze variabele gebruiken.
		// De rest van de code is hetzelfde als in FleeBehavior.
		
		if (state == 0) {
			ds = joBot.getSensorValue(BaseController.SENSOR_DS);
			joBot.setStatusLeds(false, false, false);  // Turn leds off
			joBot.drive(0, 0);
			
			// Zorg voor opdracht 2G dat de robot niet te dichtbij komt
			// Dat doe je door gebruik te maken van een extra test die
			// kijkt of de waarde niet hoger wordt dan bij 10cm afstand
			// Om die samen te voegen met de sensor waarde, gebruik je de
			// && (and) operator als volgt:
			// if ((ds > 200) && (ds < ???))  // Vul hier de juiste waarde in
			
			if (ds > 200) {
				joBot.setLed(BaseController.LED_GREEN, true);
				// Show sensor sees something
				joBot.drive(-100, -100);	
			}
		}
		
		//==============================================================
		// Opdracht 5
		// =============================================================
		// Om de gevoeligheid van de sensoren aan te passen
		// gebruik je de calibratiewaarden en verander je de
		// getallen in de bovenstaande If statements
		// Opdracht 5D - Verander gevoeligheid

	}
}



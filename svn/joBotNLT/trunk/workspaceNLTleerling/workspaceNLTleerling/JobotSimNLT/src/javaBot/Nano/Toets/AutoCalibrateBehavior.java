package javaBot.Nano.Toets;

import com.muvium.apt.PeriodicTimer;

/**
 * Autocalibrate is een demo, waarin de robot al rijdend
 * informatie over de kleur verzamelt en opslaat.
 * Na afloop wordt de informatie gebruikt bij het volgen van 
 * de lijn op het rescue veld.
 * 
 * Veld        Yel   Grn        Blk
 * --------------------------------------
 * Simulator   1023  418 (40%)  170 (40%)
 *              100   40  -60    20 -80 (20)
 * Papier       362  180 (49%)   96 (53%)
 *              100   49  -51    26 -74 (23)
 * Canvas       377  248 (65%)  160 (64%)
 *              100   65  -35    42 -56 (09)
 */

public class AutoCalibrateBehavior extends Behavior
{
	int state = 0;
	private BaseController	joBot;	
	private String table = "CLFLMCDSVMFRCR----";
	int sl = 0;
	int sr = 0;
	int pl = 0;
	int pr = 0;
	int x = 0;
	int count = 0;

	public AutoCalibrateBehavior(BaseController initJoBot, PeriodicTimer initServiceTick,
			int servicePeriod)
	{
		super(initJoBot, initServiceTick, servicePeriod);
		joBot = initJoBot;
	}

	public void doBehavior()
	{
		if (state == 0) {
			System.out.println("Nano AutoCalibrate V2.2.1");
			joBot.setStatusLeds(false, false, false);
			sl = joBot.getSensorValue(BaseController.SENSOR_VM) * 48/50 ; // 6v
			joBot.printLCD ("V=" + Integer.toString(sl));
			joBot.drive(30,30);    // Start driving
			state = 1;
			pl = 1500;
			pr = 1500;
		} else if (state < 8) {
			sl = joBot.getSensorValue(BaseController.SENSOR_DS);
			if (sl > 500)
				state = 7;     // Stop if obstacle detected
		}

		if (state == 1 || state == 2) {     // Read yellow
			sl = joBot.getSensorValue(BaseController.SENSOR_FL);
			if ((pl - sl) > (pl * 10 / 100)) {
				printSensorValue(BaseController.SENSOR_FL, sl);	
				joBot.store.pokeInt16(BaseController.STORE_L_YEL,sl);
				joBot.setLed(BaseController.LED_YELLOW, true);
				state++;
				pl = sl;
			}

			sr = joBot.getSensorValue(BaseController.SENSOR_FR);
			if ((pr - sr) > (pr * 10 / 100)) {
				printSensorValue(BaseController.SENSOR_FR, sr);
				joBot.store.pokeInt16(BaseController.STORE_R_YEL,sr);
				joBot.setLed(BaseController.LED_YELLOW, true);
				state++;
				pr = sr;
			}
		}

		if (state == 3 || state == 4) {     // Read green
			sl = joBot.getSensorValue(BaseController.SENSOR_FL);
			if ((pl - sl) > (pl * 40 / 100)) {
				printSensorValue(BaseController.SENSOR_FL, sl);	
				joBot.store.pokeInt16(BaseController.STORE_L_GRN,sl);
				joBot.setLed(BaseController.LED_GREEN, true);
				state++;
				pl = sl;
			}

			sr = joBot.getSensorValue(BaseController.SENSOR_FR);
			if ((pr - sr) > (pr * 40 / 100)) {
				printSensorValue(BaseController.SENSOR_FR, sr);
				joBot.store.pokeInt16(BaseController.STORE_R_GRN,sr);
				joBot.setLed(BaseController.LED_GREEN, true);
				state++;
				pr = sr;
			}
		}

		if (state == 5 || state == 6) {     // Read black
			sl = joBot.getSensorValue(BaseController.SENSOR_FL);
			if ((pl - sl) > (pl * 40 / 100)) {
				printSensorValue(BaseController.SENSOR_FL, sl);	
				joBot.store.pokeInt16(BaseController.STORE_L_BLK,sl);
				joBot.setLed(BaseController.LED_BLUE, true);
				state++;
				pl = sl;
			}

			sr = joBot.getSensorValue(BaseController.SENSOR_FR);
			if ((pr - sr) > (pr * 40 / 100)) {
				printSensorValue(BaseController.SENSOR_FR, sr);
				joBot.store.pokeInt16(BaseController.STORE_R_BLK,sr);
				joBot.setLed(BaseController.LED_BLUE, true);
				state++;
				pr = sr;
			}
		}

		if (state == 7) {     // Ready
			joBot.drive(0, 0);
			System.out.println("Ready");
			joBot.printLCD("Ready");
			state = 8;
		}
	}

	/**
	 * PrintSensorValue shows the value of the current
	 * sensor in the SysOut device. 
	 * This is either the simulator or the uvmIDE output
	 * When an LCD is available it also prints the 
	 * value of the selected sensor on the LCD screen.
	 */
	
	private void printSensorValue(int sensor, int value){
		if (sensor < 0) return; // Sensor does not exist
		System.out.print(table.charAt(sensor*2));
		System.out.print(table.charAt(sensor*2+1));
		System.out.print("=");
		System.out.print(value);
		System.out.print(" ");	
		byte[] lineBuffer = "  =".getBytes(); 
		lineBuffer[0] =((byte) table.charAt(sensor*2));
		lineBuffer[1] =((byte) table.charAt(sensor*2+1));
		joBot.printLCD ((new String(lineBuffer)) + 
				Integer.toString(value) + "        ");     				
	}
	
}

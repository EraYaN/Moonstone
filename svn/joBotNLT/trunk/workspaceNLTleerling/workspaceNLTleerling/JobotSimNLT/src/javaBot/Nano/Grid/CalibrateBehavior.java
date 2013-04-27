package javaBot.Nano.Grid;

import com.muvium.apt.PeriodicTimer;

/**
 * Opdracht 6 - Meten van de sensorwaardes op het echte veld
 * 
 * CalibrateBehavior tests the various sensors on board
 * It reads all sensors and displays the sensor values.
 * 
 * Sensor Levels:
 * Power 6v (8) 560=5.5v, 250=2.5v Ratio is 9.5
 * The 6v supply should not get under 2.5 volts
 * 
 * Sensor	L Low	L High	R Low	R High
 * Dist	   
 * Ground
 * IR
 */

	public class CalibrateBehavior extends Behavior {
		int state = 0;
		private BaseController	joBot;	
		private String table = "--FL--DSVMFR------";
		int s1 = 0;
		int sx = BaseController.SENSOR_VM; 
		boolean waitSwitch = false;
		
		public CalibrateBehavior(BaseController initJoBot, PeriodicTimer initServiceTick,
				int servicePeriod)
		{
			super(initJoBot, initServiceTick, servicePeriod);
			joBot = initJoBot;
		}

		public void doBehavior()
		{
			if (state == 0) {
				System.out.println("Nano Cal V323");
				joBot.setFieldLeds(true);
				state = 1;
			}
			
			int i = -1;
			s1 = joBot.getSensorValue(BaseController.SENSOR_VM) * 10/95 ; // 6v
			printSensorValue(BaseController.SENSOR_VM, s1);

			s1 = joBot.getSensorValue(BaseController.SENSOR_DS);
			printSensorValue(BaseController.SENSOR_DS, s1);
			if (s1 > 300) i = BaseController.SENSOR_DS;
						
			s1 = joBot.getSensorValue(BaseController.SENSOR_FL);
			printSensorValue(BaseController.SENSOR_FL, s1);
			if (s1 > 300) i = BaseController.SENSOR_FL;
			
			s1 = joBot.getSensorValue(BaseController.SENSOR_FR);
			printSensorValue(BaseController.SENSOR_FR, s1);
			if (s1 > 300) i = BaseController.SENSOR_FR;
						
			checkButtons();
			System.out.println();
					
			joBot.setLed(BaseController.LED_YELLOW, 
					i == BaseController.SENSOR_FL || 
					i == BaseController.SENSOR_IL); 	// Show sensor left detected
			joBot.setLed(BaseController.LED_GREEN, 
					i == BaseController.SENSOR_FR || 
					i == BaseController.SENSOR_IR);	// Show sensor right detected
			joBot.setLed(BaseController.LED_BLUE, 
					i == BaseController.SENSOR_DS); 	// Dist detected
			joBot.drive(0, 0);
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
			if (sensor == sx) {
				byte[] lineBuffer = "  =".getBytes();        
				lineBuffer[0] =((byte) table.charAt(sensor*2));
				lineBuffer[1] =((byte) table.charAt(sensor*2+1));
				joBot.printLCD ((new String(lineBuffer)) + 
						Integer.toString(value) + "        ");      
			}				
		}
		
		/**
		 * CheckButtons tests if the left or right button
		 * was pressed.
		 * If so the current sensor number is modified.
		 */
		
		private void checkButtons() {
			if (joBot.getSensorValue(BaseController.SENSOR_IL) < 255) {
				nextButton();
			}
			if (joBot.getSensorValue(BaseController.SENSOR_IR) < 255) {
				sx = BaseController.SENSOR_VM;
			}
		}
		
		private void nextButton() {
			if (waitSwitch) return;
			waitSwitch = true;
			while (sx <= BaseController.SENSOR_MX) {
				if (sx++ >= BaseController.SENSOR_MX)
					sx = 0;
				if (table.charAt(sx*2) != '-') {
					waitSwitch = false;
					break;
				}
			}
		}
}

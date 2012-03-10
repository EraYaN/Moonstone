package javaBot.Nano.Rescue;

import com.muvium.UVMRunnable;
import com.muvium.apt.PeriodicTimer;
import com.muvium.apt.PeripheralFactory;
import com.muvium.apt.TimerEvent;
import com.muvium.apt.TimerListener;
import com.muvium.io.PortIO;

/**
 * The joBot Junior version uses only two motors and has between zero and eight
 * sensors.  
 * The possible sensors are: 
 * 0 - Touch sensor left 
 * 1 - Field sensor left 
 * 3 - Distance sensor
 * 4 - Battery sensor
 * 5 - Field sensor right 
 * 6 - Touch sensor right 
 * The motors are connected as follows: 
 * 0 - Motor left 
 * 1 - Motor right  
 * 
 * UVMDemo is a behavioral demonstration which incorporates various behaviors which 
 * are set by current state. The current state can be set either remotely using the WebService or
 * by DIP switch which will override it. The current behavior will continue to
 * execute until it changes manually.  A possible next step would be to
 * include an intelligent supervisor which  determines which behavior should
 * be presently executing.
 *
  */

public class UVMDemo extends UVMRunnable implements AgentWebService,
		TimerListener {
	private BaseController joBot;
	private PeriodicTimer stateModelTick;
	private PeriodicTimer behaviorServiceTick;
	private Behavior currentBehavior;

	//State Model definitions
	public static final int STATE_IDLE 			= 0;
	public static final int STATE_CALIBRATE 	= 1; // Shows values of all sensors
	public static final int STATE_DRIVE 		= 2;
	public static final int STATE_CURIOUS		= 5; 
	public static final int STATE_LINE_FOLLOWER_ROBIN	= 4;
	public static final int STATE_LINE_FOLLOWER = 8;
	public static final int STATE_MAX_DIP 		= 9;
	//private String DIP = "0=Idle ,1=Calib,2=Drive,3=Dummy,4=LineFollowerRobin,5=Curious,6=Dummy,7=Dummy,8=Line";
	private String[] DIPs = {"Idle","Calib","Drive","Dummy","LineFollowerRobin","Curious","Dummy","Dummy","Line"};

	/**
	 * Dip Switch Settings  The DIP Switch sets the mode and can be set either
	 * manually or remotely. The currentDIP setting is updated either by a
	 * webService call OR by changing the DIP value.
	 */
	private int currentState = -1;
	private int previousState = -1;

	/**
	 * WebService Interfaces WebServices are remotely accessible Remote Method
	 * Invocation. The AgentWebService is a WSDL ( Web Service Description
	 * Language) Interface which defines the following methods which are
	 * exposed by muvium as WebService Methods     
	 * public int getSensor(int sensor); 
	 * public int getState();     
	 * public void setState(int dip);
	 * public void vector(int vx, int vy, int omega);
	 * public void drive(int vx, int vy, int vz);
	 */

	/**
	 * [WebService]
	 */
	public void vector(int vx, int vy, int omega) {
		joBot.drive(vx, vy);
	}

	/**
	 * [WebService]
	 */
	public void drive(int vx, int vy, int vz) {
		joBot.drive(vx, vy);
	}

	public void drive(int vl, int vr) {
		joBot.drive(vl, vr);
	}

	/**
	 * [WebService] getSensor returns the current ADC sensor reading of the
	 * <code>sensor</code> sensor which represents what the robot currently
	 * sees from this sensor
	 */
	public int getSensor(int sensor) {
		return joBot.getSensorValue(sensor);
	}

	/**
	 * [WebService] getDIP returns the current value of the robot DIP switch
	 * allowing the a remote representation of the Robot to updated
	 */
	public int getState() {
		return currentState;
	}

	/**
	 * [WebService] setState overrides the currentDIP value which allows the
	 * robot mode  to be set remotely
	 */
	public void setState(int newState) {
		int state = 0;
		if (newState == -1)
			state = previousState;
		else
			state = newState;
		processState(state);
	}

	/**
	 * [WebService] reportState sets the reporting level in the robot
	 * This allows the remote machine to define if test output is generated
	 */
	public void reportState(int level) {
		joBot.reportState(level);
	}

	/**
	 * HeartBeat and check for manual state change using physical DIP switches
	 */
	public void Timer(TimerEvent e) {
		// Display a HeartBeat so we know things are still running
		joBot.heartBeat();

		// Check to see if the DIP Switch has changed
		// On the proto the DIP is on the low order bits
		// On the standard module the DIP is on the high order bits
		int dip = PortIO.getPort(PortIO.PORTB) & 0xF0;
		if (dip == 0)
			dip = PortIO.getPort(PortIO.PORTB) ^ 0x0F;
		else		
			dip = (PortIO.getPort(PortIO.PORTB) >> 4) ^ 0x0F;
		
		if (dip != previousState) {
			setState(dip);
			previousState = dip;
		}
	}

	/**
	 * ProcessState stops the current behavior and starts a new one.
	 */
	public void processState(int dipValue) {
		currentState = dipValue;
		showDIP (dipValue);
		
		if (currentBehavior != null) {
			currentBehavior.stop();
			currentBehavior = null;
		}
		
		switch (dipValue) {
		case STATE_IDLE:
			joBot.setStatusLeds(false, false, false, false);
			joBot.setFieldLeds(false);
			drive(0, 0);
			break;
		case STATE_CALIBRATE: // 1
		//		Opdracht 5 - Meten van de sensorwaardes in de simulator
		//		Opdracht 6 - Meten van de sensorwaardes op het echte veld
		//		Opdracht 9 - Meten van de batterijspanning
			currentBehavior = new CalibrateBehavior(joBot, behaviorServiceTick, 500);
			break;
		case STATE_DRIVE: // 2
		//		Opdracht 4A/B - Vooruit rijden
		//		Opdracht 4C - Stop na 2 seconden
		//		Opdracht 4D - Maak een bocht
		//		Opdracht 4E - Gebruik een subroutine
		//		Opdracht 4F - Rijd in een vierkantje
		//		Opdracht 5C - Rijd tot aan de zwarte lijn
		//		Opdracht 5E - Doorzoek het moeras
			currentBehavior = new DriveBehavior(joBot, behaviorServiceTick, 100);
			break;
		case STATE_LINE_FOLLOWER_ROBIN: // 4
			//		Opdracht 6  - Simpele Line Follower met een sensor
			//		Opdracht 7A - Line Follower met twee sensoren
			//		Opdracht 7B - Gebruik maken van een subroutine
			//		Opdracht 7B - Het volgen van de gele lijn
			//      Opdracht 7C - Ontwijk obstakel
			//		Opdracht 11 - Maak een lijnvolger met obstakeldetectie
				currentBehavior = new LineFollowerBehaviorRobin(joBot,
						behaviorServiceTick, 100);
				break;
		case STATE_CURIOUS: // 5
		//		Opdracht 5D - Maak sensoren gevoeliger
			currentBehavior = new CuriousBehavior(joBot, behaviorServiceTick, 100);
			break;
		case STATE_LINE_FOLLOWER: // 8
		//		Opdracht 6  - Simpele Line Follower met een sensor
		//		Opdracht 7A - Line Follower met twee sensoren
		//		Opdracht 7B - Gebruik maken van een subroutine
		//		Opdracht 7B - Het volgen van de gele lijn
		//      Opdracht 7C - Ontwijk obstakel
		//		Opdracht 11 - Maak een lijnvolger met obstakeldetectie
			currentBehavior = new LineFollowerBehavior(joBot,
					behaviorServiceTick, 100);
			break;
		
		default:
			drive(0, 0);
			break;
		}
	}

	public void run() {
		System.out.println("UVMdemo Nano NLT Rescue V323");
		try {
// 			When PortB is not cleared, the real robot will not be able to read the DIP switch, 
//			since its common input is high. 
// 			When PortB is cleared, the simulator does not read any data so it should be left this way.
//			The function setPortBDIP in NanoRobot is responsible for this.
            boolean powerPortB = true;
            com.muvium.debug.Debug.beginDebug();
            powerPortB = false;
            com.muvium.debug.Debug.endDebug();
            if( powerPortB ) {
    			PortIO.setPort(0x00, PortIO.PORTB); //Set DIP power to ON
            }
			PortIO.setTris(0xF1, PortIO.PORTB); //Upper 4 bits are DIP, B3 = DIP Power, B2 = Right Field, B1 = Left Field, B0 = IR Receiver
			PortIO.setProperty(PortIO.PORTB, PortIO.PROPERTY_PORT_PULLUP_ON);
			PortIO.setTris(0x0F, PortIO.PORTE);
			joBot = new BaseController(getPeripheralFactory(), this);
			joBot.setStatusLeds(false, false, false, false);
			drive(0, 0);
			joBot.tone(0);
			joBot.printLCD("Resc.323");

			/**
			 * The stateModel determines the current behavior and refreshes the
			 * status lights
			 */
			stateModelTick = getPeripheralFactory().createPeriodicTimer(this,
					500, PeripheralFactory.EVENT_PRIORITY_BACKGROUND);
			behaviorServiceTick = getPeripheralFactory().createPeriodicTimer(
					null, 1000, PeripheralFactory.EVENT_PRIORITY_URGENT);
			previousState = -1;

			try {
				stateModelTick.start();
			} catch (Exception e) {
			}

			while (true) {
				Thread.sleep(100);
			}
		} catch (Exception e) {
			joBot.setStatusLeds(true, true, true, true);
		}
	}

	public int getCurrentState() {
		return currentState;
	}
	
	private void showDIP (int dipValue) {
		if (dipValue == 0){
		    for(int x = 0; x < DIPs.length; x++){
				System.out.print(x+" = "+DIPs[x]+", ");
			}
		    System.out.println();
		}	        
		if (dipValue < STATE_MAX_DIP) {
			System.out.println("Current State: "+DIPs[dipValue]+"("+dipValue+")");
			joBot.printLCD(DIPs[dipValue]);
		}
		else {
			System.out.println("DIP=" + Integer.toString(dipValue));
		    joBot.printLCD ("DIP=" + Integer.toString(dipValue));
		}
	}

}

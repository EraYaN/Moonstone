package javaBot;

import javaBot.JoBotNanoController;
import org.openVBB.robotKit.interfaces.Units;
import org.openVBB.sim.rti.OpenVBBRTIImpl;

public class JoBotNanoRobot extends NanoRobot {

	private static final String	IMAGE_FILE			= Simulator
															.getRelativePath("./resources/jobotnano.gif");
	private static final String	CONFIG_XML			= Simulator
															.getRelativePath("./Nano/Test/joBotNano.xml");
	private static final int	DIAMETER_IN_IMAGE	= 100;

	public static final double	BASE_RADIUS			= 0.070;
	public static final double	WHEEL_RADIUS		= 0.035;
	public static final int		RED_LED				= 0;
	public static final int		YELLOW_LED			= 1;
	public static final int		GREEN_LED			= 2;
	public static final int		BLUE_LED			= 3;
	
	JoBotServoMotor[]			servoMotors;

	/**
	 * Creates a new UVMRobot instance
	 */
	public JoBotNanoRobot(String name, double positionX, double positionY)
	{
		/*
		 * friction = 7.0
		 * diameter = 2.0
		 * mass = 3.0
		 */
		super(name, 7.0, positionX, positionY, BASE_RADIUS * 2, 3);

		try
		{
			// If > 1.8, Jobot will get stuck to the wall with his butt
			// If < 1.8, you will see a space between the Jobot and the walls.
			this.distanceFromWallX = 1.2;
			this.distanceFromWallY = 1.2;
			// Create the sensors
			setSensors(new Sensor[10]);
			
			// addSensor parameters: sensorClassName, radius, position [, direction]
			getSensors()[0] = addSensor("ContactSensor", BASE_RADIUS, 300, 120);			// Left
			getSensors()[1] = addSensor("FieldSensor", BASE_RADIUS, 100);					// Left
			getSensors()[2] = addSensor("DummySensor", 0,0);			
			getSensors()[3] = addSensor("DistanceShortSensor", BASE_RADIUS, 0, 90);  	
			getSensors()[4] = addSensor("DummySensor", 0,0);			
			getSensors()[5] = addSensor("FieldSensor", BASE_RADIUS, 80);					// Right
			getSensors()[6] = addSensor("ContactSensor", BASE_RADIUS, 240, 60);				// Right
			getSensors()[7] = addSensor("DummySensor", 0,0);			
			getSensors()[8] = addSensor("DummySensor", 0,0);			
			getSensors()[9] = addSensor("DummySensor", 0,0);			
		}
				
		catch( ReflectionException e )
		{
			e.printStackTrace();
		}
		// Create the leds
		setLeds(new Led[4]);
		getLeds()[GREEN_LED] = new Led(java.awt.Color.GREEN, 0);
		getLeds()[RED_LED] = new Led(java.awt.Color.RED, 0);
		getLeds()[YELLOW_LED] = new Led(java.awt.Color.YELLOW, 0);
		getLeds()[BLUE_LED] = new Led(java.awt.Color.BLUE, 0);
		
		setLCD(new LCD());

		loadApp(CONFIG_XML);
		vbbRTI.start();

		if (servoMotors != null)
		{
			setActors(new Actor[5]);
			getActors()[0] = servoMotors[0];
			getActors()[1] = servoMotors[1];
			getActors()[2] = servoMotors[2];
			getActors()[3] = servoMotors[3];
			getActors()[4] = servoMotors[4];
		}
	}

	/**
	 * TODO METHOD: DOCUMENT ME!
	 */
	protected String getConfigXMLDoc()
	{
		return CONFIG_XML;
	}

	/**
	 * This returns the sensor values
	 */
	public double getADCSample(int channel)
	{
		if (getSensors() == null)
		{
			System.out.println("Sample requested by null sensorValues");

			return 0;
		}

		// if (channel == 3) Debug.printDebug("Reading channel " + channel);
		return getSensors()[channel].getValue() / 1000;
	}

	/* (non-Javadoc)
	 * @see javaBot.PhysicalObject#createGraphicalRepresentation()
	 */
	public GraphicalRepresentation createGraphicalRepresentation()
	{
		return new GraphicalRepresentation(this, IMAGE_FILE, DIAMETER_IN_IMAGE, true);
	}

	public void digitalOutputPinChanged(boolean vout, int pinId)
	{
		if (vout == true)
		{
			getLeds()[pinId].setValue(1.0);
		}
		else
		{
			getLeds()[pinId].setValue(0.0);
		}
	}

	/**
	 * Loads the application
	 */
	public void loadApp(String configXML)
	{
		System.out.println("Loading from config file: " + configXML);

		/**
		 * If there is an existing simulation RTI then we need to rip it up and
		 * release the resources associated with the RTI so we can start over
		 */
		if (vbbRTI != null)
		{
			vbbRTI.ripUp();
		}

		vbbRTI = new OpenVBBRTIImpl();

		controller = new JoBotNanoController(this, .1, vbbRTI, configXML);

		/**
		 * Register JoBotServoMotors as ServoPulseListeners
		 */
		servoMotors = new JoBotServoMotor[2];

		for (int i = 0; i < servoMotors.length; i++)
		{
			servoMotors[i] = new JoBotServoMotor(0.0, 156, 
								1 * Units.MILLISECONDS, 2 * Units.MILLISECONDS,
								"S148");
			controller.registerServoPulseListener(servoMotors[i], i);
		}

		/**
		 * Register ADCSampleListeners to read the sensors
		 */
		controller.registerADCSampleListener(this, 0);
		controller.registerADCSampleListener(this, 1);
		controller.registerADCSampleListener(this, 2);
		controller.registerADCSampleListener(this, 3);
		controller.registerADCSampleListener(this, 4);
		controller.registerADCSampleListener(this, 5);
		controller.registerADCSampleListener(this, 6);
		controller.registerADCSampleListener(this, 7);
		controller.registerADCSampleListener(this, 8);
		controller.registerADCSampleListener(this, 9);

		/**
		 * Register DigitalOutputListeners to read update the LED values
		 */
		controller.registerDigitalOutputListener(this, RED_LED);
		controller.registerDigitalOutputListener(this, YELLOW_LED);
		controller.registerDigitalOutputListener(this, GREEN_LED);
		controller.registerDigitalOutputListener(this, BLUE_LED);
		controller.setPortBDIP(15); // Initialize the DIP settings		
		controller.registerLCDListener(this);
	}

	/* (non-Javadoc)
	 * @see javaBot.MovableObject#update(double)
	 * To make sure the user does not have to remember the motors are
	 * driven as if they are both running forward.
	 * Here the command for the right wheel is reversed to reflect
	 * the fact that both wheels run in opposite directions.
	 */
	public void updatePosition(double elapsed)
	{
		double wl = 0 - servoMotors[0].getValue();
		double wr = servoMotors[1].getValue();

		if (servoMotors != null)
		{
			double rvl = (WHEEL_RADIUS * wl) / 3.24 * Math.PI;
			double rvr = (WHEEL_RADIUS * wr) / 3.24 * Math.PI;
			double rv = (rvl + rvr) / 2;
			double rr = (rvl - rvr) / BASE_RADIUS;
			setDrivingVelocityX(rv * Math.sin(rotation));
			setDrivingVelocityY(0 - (rv * Math.cos(rotation)));
			this.setRotationSpeed(rr);
		}
	}

	public boolean getGreenLed()
	{
		return (getLeds()[GREEN_LED].getValue() != 0);
	}

	public boolean getRedLed()
	{
		return (getLeds()[RED_LED].getValue() != 0);
	}

	public boolean getYellowLed()
	{
		return (getLeds()[YELLOW_LED].getValue() != 0);
	}

	public boolean getBlueLed()
	{
		return (getLeds()[BLUE_LED].getValue() != 0);
	}
	
}

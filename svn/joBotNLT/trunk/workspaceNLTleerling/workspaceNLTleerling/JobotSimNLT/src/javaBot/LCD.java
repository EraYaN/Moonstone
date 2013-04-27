package javaBot;

/**
 * Represents a basic LCD for use on the robot and is displayed in the
 * RobotGUI.
 */
public class LCD extends Actor {

	public String lcdText	= "";
	public String lcdText1  = "";

	public LCD(String def)
	{
		super("LCD", null, 0);
	}

	/**
	 * Creates a new LCD.
	 */
	public LCD() {
		super("LCD", null, 0);
	}

	public void setText(String value) {
		lcdText1 = lcdText;
		lcdText = value;
	}
	
}

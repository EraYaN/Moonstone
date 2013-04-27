/*
 * Created on Jun 8, 2004
 */
package javaBot;

import java.awt.Color;
/**
 * Simulates the sensor on the robot to determine the current location on the
 * RoboCup junior playingfield (PlayingField.java). Currently the sensor
 * simply returns the value of the gradient (0%-100%) which is not based on a
 * real sensor. <br>
 * <br>
 * The sensor itself doesn't do any work, the PlayingField returns the correct
 * value by calling giveSensoryInformationTo(Robot r).
 */
public class FieldSensor extends Sensor
{
	/**
	 * Creates a new FieldSensor object.
	 *
	 * @param position The position relative to the robot the sensor placed on
	 * @param angle The angle relative to the robot under which the sensor
	 *        placed
	 */
	public FieldSensor(Location position, double angle)
	{
		// create sensor with position, angle and default value 0
		super("FieldSensor", position, angle, 0);
	}
	
	public Line sensorLine(double posX, double posY, double rotationRobot)
	{
		double diam = Math.sqrt(Math.pow(getPosition().getX(), 2)
				+ Math.pow(getPosition().getY(), 2));
		diam = 0.05;
		Location point = new Location(posX + Math.cos(getAngle() + rotationRobot) * diam, posY
				+ Math.sin(getAngle() + rotationRobot) * diam);
		return new Line(point, getAngle() + rotationRobot, 0.01, Color.CYAN);
	}

}

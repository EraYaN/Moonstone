/*
 * Created on Jun 9, 2004
 */
package javaBot;

import java.awt.Color;

/**
 * Used as a simulation of the AVR Camera.
 * It uses two IR sensors to calculate the ball position within the field.
 * The distance and opening angle must be made equal to that of the camera.
 */
public class AVRSensor extends Sensor
{
	/** How wide is the field of view of this sensor */
	double	fieldOfView;

	/** What's the max distance the sensor can look */
	double	maxDist;

	/**
	 * Create a new AVRSensor
	 *
	 * @param position The position relative to the robot the sensor placed on
	 * @param angle The angle relative to the robot under which the sensor
	 *        placed
	 */
	public AVRSensor(Location position, double angle)
	{
		// create sensor with position, angle and default value 0
		super("AVRSensor", position, angle, 0);

		// Give sensor field of view and a max distance of 120 cm
		this.maxDist = 1.2;
		this.fieldOfView = .50 / 2 * Math.PI;
	}

	/**
	 * Create a line going through the sensor to the object and check if this
	 * line is within the fieldOfVision, if so return a value representing the
	 * location of the object relative to the sensor's angle
	 *
	 * @param robot location of the robot
	 * @param rotationRobot rotation of the robot
	 * @param obj the location of the object
	 *
	 * @return value representing the location of the object
	 */
	public double checkFOV(Location robot, double rotationRobot, Location obj)
	{
		// Determine angle of sensor in the world
		double angleSensor = (rotationRobot % (2 * Math.PI)) + getAngle();

		// Show field of view
		Line l = new Line(new Location(0, 0), getPosition());
		Location point = new Location(robot.getX()
				- (Math.cos(l.getAngle() + rotationRobot) * l.getLength()), robot.getY()
				- (Math.sin(l.getAngle() + rotationRobot) * l.getLength()));
		Line f1 = new Line(point, angleSensor + (fieldOfView / 2), maxDist);
		f1.setLineColor(Color.CYAN);

		Line f2 = new Line(point, angleSensor - (fieldOfView / 2), maxDist);
		f2.setLineColor(Color.CYAN);

		// Determine angle of object to sensor in the world
		l = new Line(point, obj);
		l.setLineColor(Color.DARK_GRAY);

		double angleToObject = l.getAngle();

		double angularDifference = Math.abs(Math.IEEEremainder(angleToObject - angleSensor,
				2 * Math.PI));

		// Debug.printDebug("angular difference " + angularDifference);
		if (angularDifference <= (fieldOfView / 2))
		{
			if (l.getLength() <= maxDist)
			{
				// Return a value that is dependent on the location of the object
				// return 1 - angularDifference/(fieldOfView/2);
				return 1023 - (l.getLength() * 1000);
			}
		}

		return getDefaultValue();
	}
}

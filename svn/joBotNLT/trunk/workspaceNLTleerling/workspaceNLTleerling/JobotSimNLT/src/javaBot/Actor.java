/*
 * Created on Jun 18, 2004
 * Copyright: (c) 2006
 * Company: Dancing Bear Software
 *
 * @version $Revision: 1.1 $
 *
 */
package javaBot;

/**
 * @version $Revision: 1.1 
 * $ last changed Feb 17, 2006
 */
public abstract class Actor
{
	//Under wich angle is this actor positioned
	private double		angle;

	//What is the relative position of the actor on the robot
	private Location	position;

	//current value
	private double		value;

	//Just for GUI purposes
	private String		name;

	//Default Constructor
	Actor()
	{
	}

	//Constructor: Creates a new Actor object under specified angle and postion
	Actor(String name, Location position, double angle)
	{
		this.name = name;
		this.position = position;
		this.angle = angle;
	}

	public double getAngle()
	{
		return angle;
	}

	public double getValue()
	{
		return value;
	}

	public void setAngle(double angle)
	{
		this.angle = angle;
	}

	public void setValue(double value)
	{
		this.value = value;
	}

	public Location getPosition()
	{
		return position;
	}

	public void setPosition(Location location)
	{
		position = location;
	}

	public void reset()
	{
		value = 0;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}
	
	public double getPercentageSpeed()
	{
		return getValue();
	}
}

/*
 * Created on 20 Feb 2006
 * Copyright: (c) 2006
 * Company: Dancing Bear Software
 * @version $Revision: 1.1 $
 * Simulated brick used for RoboCup junior simulation.
 * @version $Revision: 1.1 $ Copied from Wall - 26/07/2008
 *  **/
package javaBot;

import java.awt.Polygon;
import java.awt.Rectangle;
import java.awt.Color;

// ToDo: Uitzoeken waarom rotatie niet werkt
//       Afgeleid van Wall, waarin dit wel werkt.

public class BrickOld extends NonMovableObject implements IDragable {
	// Info for the image to display for the brick
	private static final String IMAGE_FILE = Simulator
			.getRelativePath("./resources/brick.gif");
	private static final int DIAMETER_IN_IMAGE = 50;
	private Line[] brick;
	private double width;
	private double height;
	private double x;
	private double y;

	/**
	 * Constructor creates default brick, set properties afterwards if required
	 * Image is 120x120 pixels. Brick is 73x73 pixels, borders are 23 pixels
	 */
	public BrickOld() {
		super("Brick");
		diameter = 0.1;
		location = new Location(World.WIDTH / 2, World.HEIGHT / 2);
		setLocation(location);
	}

	/**
	 * Give wall information to robot
	 *
	 * @param r Robot who want to have the information
	 * @return returnValue Sensor information
	 */
	public double[] giveSensoryInformationTo(Robot r) {
		double[] returnValue = r.newSensorValues();
		int w;

		for (int i = 0; i < r.getSensors().length; i++) {
			if (r.getSensors()[i] instanceof DistanceSensor) {
				DistanceSensor ds = (DistanceSensor) (r.getSensors()[i]);

				// Calc wall distances
				for (w = 0; w < 4; w++) {
					Line sl = ds.sensorLine(r.getLocation().getX(), r
							.getLocation().getY(), r.getRotation());
					double d = ds.convertDistanceToValue(sl
							.intersectDist(brick[w]));

					// If the current object is closer to the sensor than the
					// current reading, overwrite it.
					if ((d > 0) && (d > returnValue[i])) {
						returnValue[i] = d;
					}
				}
			}
		}
		return returnValue;
	}

	/**
	 *
	 * @return Return a GUI representation of a wall
	 */
	public GraphicalRepresentation createGraphicalRepresentation() {
		return new GraphicalRepresentation(this, IMAGE_FILE, DIAMETER_IN_IMAGE,
				true);
	}

	/**
	 * Set the location of the brick
	 *
	 * @param OrgLocation current location
	 */
	public void setLocation(Location OrgLocation) {
		location = OrgLocation;
		double brickWidth = getWidth();
		double brickHeight = getHeight();
		double brickX = getX();
		double brickY = getY();

		// create box
		Line[] lBrick;
		lBrick = new Line[4];
		lBrick[0] = new Line(new Location(brickX, brickY), new Location(brickX
				+ brickWidth, brickY));
		lBrick[1] = new Line(new Location(brickX, brickY), new Location(brickX,
				brickY + brickHeight));
		lBrick[2] = new Line(new Location(brickX, brickY + brickHeight),
				new Location(brickX + brickWidth, brickY + brickHeight));
		lBrick[3] = new Line(new Location(brickX + brickWidth, brickY),
				new Location(brickX + brickWidth, brickY + brickHeight));
		// rotate box
		brick = doRotate(lBrick, getRotation());
	}

	/**
	 * Rotate 4 lines, use rotation
	 *
	 * @param Orignele 4 lijnen array 
	 * @return New rotated lines
	 */
	private Line[] doRotate(Line[] original, double dRotation) {
		double dX;
		double dY;
		double dRotate = 0;
		double iLength = 0;
		double brickWidth = getWidth();
		double brickHeight = getHeight();
		double brickX = getX();
		double brickY = getY();

		Location lStartPosition;
		Line[] lReturn = new Line[original.length];

		// Rotate every line separately
		for (int iLineCounter = 0; iLineCounter < original.length; iLineCounter++) {
			// Line length
			iLength = original[iLineCounter].getLength();
			dRotate = dRotation;

			if (iLineCounter == 0) {
				// Set first line on startposition
				// Draw from new position with angle with origional line length
				dX = (brickX + (brickWidth / 2)); //center
				dY = (brickY + (brickHeight / 2)); //center
				lStartPosition = new Location(dX, dY);
				// Set middle point of object to left bottom
				lStartPosition = toBasicPoint(lStartPosition, dRotation);
			} else {
				// Second, Third, Fourth
				// Ask previous line end-position
				lStartPosition = lReturn[iLineCounter - 1].getQ();
				// Which line is this?
				// Then rotate the line number with 90 degrees
				// to provide the square characteristics
				dRotate = dRotate + (Deg2Rad(90 * iLineCounter));
			}
			lReturn[iLineCounter] = new Line(lStartPosition, dRotate, iLength);
			lReturn[iLineCounter].setLineColor(Color.CYAN);
		}
		return lReturn;
	}

	/**
	 * @param startPosition Location with startposition on the center of the object
	 * @param dRealRotation Rotation of object in Radialen
	 * @return
	 */
	private Location toBasicPoint(Location startPosition, double dRealRotation) {
		Line lBuffer;
		double iObjectHeight;
		double dAngle;

		/**
		 * dAngle = Angle e,d (Line e --> d)
		 * 
		 * a-----b
		 * |\    |
		 * | \   |
		 * |  e  |
		 * |     |
		 * |     |
		 * c-----d
		 * 
		 */
		dAngle = Math.tan((getWidth() / 2) / (getHeight() / 2));
		// Length e --> a
		iObjectHeight = Math.sqrt(Math.pow(getHeight() / 2, 2)
				+ Math.pow(getWidth() / 2, 2));
		// Create line from e to a
		lBuffer = new Line(new Location(0, 0), dRealRotation + dAngle
				+ Deg2Rad(90), iObjectHeight); // baken
		// Create line from a to c
		lBuffer = new Line(new Location(lBuffer.getQ().getX(), lBuffer.getQ()
				.getY()), dRealRotation + Deg2Rad(90 + 180), getHeight());
		// Set new location
		startPosition.setX(startPosition.getX() + lBuffer.getQ().getX());
		startPosition.setY(startPosition.getY() + lBuffer.getQ().getY());
		return startPosition;
	}

	/**
	 * Calculate grades to radius
	 *
	 * @param Deg grades
	 *
	 * @return Radius
	 */
	private double Deg2Rad(double Deg) {
		return Deg / (180 / Math.PI);
	}

	/**
	 * @return current height of brick
	 * Have to resize the dimensions, but is not clear why.
	 */
	public double getHeight() {
		height = (this.getGraphicalRepresentation().getHeight() - 20)
				/ Simulator.pixelsPerMeter;
		height *= 0.58; // Size is 70/120 pixels
//		System.out.print("Y=");
//		System.out.print(height);
		return height;
	}

	/**
	 * @return current width of brick
	 */
	public double getWidth() {
		width = (this.getGraphicalRepresentation().getWidth() - 0)
				/ Simulator.pixelsPerMeter;
		width *= 0.58; // Size is 70/120 pixels
//		System.out.print(",X=");
//		System.out.println(width);
		return width;
	}

// Have to add an offset but is not clear why
//When rotating this does not work either
	public double getX() {
		x = location.getX() - (getWidth() / 2) + 0.031;
		return x;
	}

	public double getY() {
		y = location.getY() - (getHeight() / 2) + 0.061;
		return y;
	}

	/**
	 * Function for setting the rotation of the wall
	 * Needed because the wall has to be recalculated
	 * @param rotation Rotation of wall
	 */
	public void setRotation(double rotation) {
		super.setRotation(rotation);
		setLocation(getLocation());
	}

	/**
	 * Handle collisions with the surrounding walls.
	 *
	 * @param object The object the collision will be calculated
	 */
	public void collideWith(MovableObject object) {
		Polygon p = new Polygon();
		for (int i = 0; i < brick.length; i++) {
			Location l = brick[i].getQ();
			int brickX = (int) (l.getX() * Simulator.pixelsPerMeter);
			int brickY = (int) (l.getY() * Simulator.pixelsPerMeter);
			p.addPoint(brickX, brickY);
		}

		Location l = object.getLocation();

		int objectX = (int) (l.getX() * Simulator.pixelsPerMeter);
		int objectY = (int) (l.getY() * Simulator.pixelsPerMeter);
		int objectDiameter = (int) (object.diameter * Simulator.pixelsPerMeter);

		Rectangle r = new Rectangle(objectX, objectY, objectDiameter,
				objectDiameter);

		if (p.intersects(r)) {
			object.setAccelerationX(0);
			object.setAccelerationY(0);
			object.setVelocityX(0);
			object.setVelocityY(0);
		}
	}
}

package javaBot.Nano.Toets;

/**
 * The MapReader Behavior is a state machine that reads either a drive or a
 * vector 'map' and every period changes the values of the drivers to
 * represent these values
 */

import com.muvium.apt.PeriodicTimer;
import com.muvium.apt.TimerEvent;
import com.muvium.apt.TimerListener;

/**
 * Created on 20-02-2006
 * Copyright: (c) 2006
 * Company: Dancing Bear Software
 *
 * @version $Revision$
 * last changed 20-02-2006
 */
public abstract class Behavior implements TimerListener
{
	private BaseController	joBot;
	private PeriodicTimer			serviceTick;

	/**
	 * Creates a new BehaviorJunior object.
	 *
	 * @param initJoBot TODO PARAM: DOCUMENT ME!
	 * @param initServiceTick TODO PARAM: DOCUMENT ME!
	 * @param period TODO PARAM: DOCUMENT ME!
	 */
	public Behavior(BaseController initJoBot, PeriodicTimer initServiceTick, int period)
	{
		serviceTick = initServiceTick;
		joBot = initJoBot;
		serviceTick.setPeriod(period);

		try
		{
			serviceTick.addTimerListener(this);
			serviceTick.start();
		}
		catch (Exception e)
		{
			//Failed to start
		}
	}

	/**
	 * TODO METHOD: DOCUMENT ME!
	 */
	public abstract void doBehavior();

	/**
	 * TODO METHOD: DOCUMENT ME!
	 *
	 * @param e TODO PARAM: param description
	 */
	public void Timer(TimerEvent e)
	{
		doBehavior();
	}

	/**
	 * TODO METHOD: DOCUMENT ME!
	 */
	public void stop()
	{
		serviceTick.removeTimerListener(this);
		serviceTick.stop();
		serviceTick = null;
	}

	/**
	 * TODO METHOD: DOCUMENT ME!
	 *
	 * @return $returnType$ TODO RETURN: return description
	 */
	public BaseController getJoBot()
	{
		return joBot;
	}
}

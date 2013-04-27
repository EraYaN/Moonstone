package javaBot;

import org.openVBB.interfaces.IopenVBBRTI;
import org.openVBB.robotKit.controllers.JoBotJPB2Controller;
import org.openVBB.robotKit.interfaces.StepTimeListener;
import com.muvium.driver.display.lcd.LCDPrinter;
import javaBot.lcdHandler;

public class JoBotNanoController extends JoBotJPB2Controller {

	public JoBotNanoController(StepTimeListener stepTimeListener, double stepTime,
			IopenVBBRTI vbbRTI, String className) {
		super(stepTimeListener, stepTime, vbbRTI, className);
	}
	
	public void registerLCDListener(NanoRobot theRobot) {
		lcdHandler.lcd = theRobot.createLCDHandler();
	}
	
	public void printLCD (String txt) {
		lcdHandler.writeCommand (LCDPrinter.HOME);
		lcdHandler.writeCommand (LCDPrinter.CLEAR);
		lcdHandler.println (txt);  
	}

}

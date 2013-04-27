package javaBot;

import com.muvium.driver.display.lcd.LCDPrinter;

public class lcdHandler {
	static LCDPrinter lcd = new LCDPrinter();
    
	public static void println(String text) {
		lcd.println(text);
	}
	
	public static void writeCommand(byte aCommand) {
		lcd.writeCommand(aCommand);
	}
}

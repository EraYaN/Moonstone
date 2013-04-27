/*
 * @(#)IGenericSensorProxy.java 
 */

package javaBot.sensors.ws.proxyImpls;

//**********************************************************
//<autogenerated>
//	This code was auto-generated by wsdlgen, Version 0.1
//	Changes to this file may cause incorrect behavior and will be lost if
//	the code is regenerated
//
//	This class is a mirror interface proxy class to a WebService Interface.
//	When this proxy class is instantiated with the Internet Address a muvium
//	device programmed with the matching WebService this proxy class will
//	transparently invoke the remote method on the muvium device.
//
//	<usage>
//	IGenericSensor proxy = new IGenericSensorProxy("10.1.1.7");
//	proxy.method();
//	</usage>
//</autogenerated>
//**********************************************************

import javaBot.sensors.ws.interfaces.ISensorUvm;

import com.muvium.web.services.protocols.HttpGetClientProtocol;

public class ISensorUvmProxy extends HttpGetClientProtocol implements ISensorUvm
{
	String	myIPAddress;

	public ISensorUvmProxy(String ipAddress)
	{
		myIPAddress = ipAddress;
	}

	public byte[] getSample(int sensor)
	{
		return invokeByteArray(myIPAddress, "//getSample", new Object[] {new Integer(sensor)});
	}

	public int getSensorValue(int sensor)
	{
		return invokeInt(myIPAddress, "//getSensorValue", new Object[] {new Integer(sensor)});
	}

	public int getState()
	{
		return invokeInt(myIPAddress, "//getState", new Object[] {null});
	}

	public void setState(int int0)
	{
		invokeInt(myIPAddress, "//setState", new Object[] {new Integer(int0)});
	}

}

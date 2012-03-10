/* ChaseBallBehaviorIR.java
 * Een simpele routine die een jobot een bal laat volgen
 * Als hij de bal niet ziet gaat hij rondjes draaien
 * Als hij hem wel ziet gaat hij naar de bal toe en geeft hem een duw
 * Deze versie gebruikt de IR sensor om een RoboCup Junior bal te vinden
 * 
 * Floor Sietsma
 * 0418803
 * fsietsma@science.uva.nl
 * 
 * Steven Klein
 * 0597872
 * sklein@science.uva.nl
 * 
 * Universiteit van Amsterdam
 * Bij het project Robotica en Systemen
 * Woensdag 1 februari 2006
 * 
 */

package javaBot.UVM;
import com.muvium.apt.*;

public class VoetBalBehavior extends Behavior {
	/*
	 * VoetbalBehavior (dip=13)
	 * The behavior base class sits on the behavior base class 
	 * @author Simon Kissing
	 */

		private int s0 = 0;
		private int s1 = 0;
		private int s2 = 0;
		private int s3 = 0;
		//private int s4 = 0;
		private int teller = 0;
		private int loopteller = 0;
		private boolean test;
		//private boolean vlrsens;

		public VoetBalBehavior(BaseController initJoBot,
				PeriodicTimer initServiceTick, int servicePeriod) {
			super(initJoBot, initServiceTick, servicePeriod);
			test = false;
		}

		public void doeiets()
		{
			int vx = 0;
			int vy = 0;
			int vz = 0;
			//int vloer = 0;
			
			
			// Het aanvragen van de sensoren
			s0 = joBot.getSensor(0);
			s1 = joBot.getSensor(1);
			s2 = joBot.getSensor(2);
			s3 = joBot.getSensor(3) + 1; 
			//s4 = joBot.getSensor(4);
				
	        //Als Sensor 3(IR) een waarde grooter dan 100 onvangt
			//dan:
			//gaat hij vooruit
			//geeft doormiddel van de leds weer welke sensor word aangesproken
			//en geeft de waarde door op het scherm om te controleren
			if (s3 > 100)
	        {
				s0 = 100;
	        	vx = 0;
	        	vy = s0;	
				vz = -s0;
				teller = 0;
				joBot.setStatusLeds(s2 > 500, s3 > 100, s1 > 500); 		// Show IR detected
				//System.out.print("afstandssensor 1:");
				//System.out.print(s1);
				//System.out.print("-");
				//System.out.print("afstandssensor 2:");
				//System.out.print(s2);
				//System.out.print("-");
				//System.out.print("lichtsensor:");
				//System.out.println(s3);
				
	        } 
			

	        //Als Sensor 3(IR) een kleinere waarde dan 100 ontvangt
			//dan draait hij rond tot hij de bal heeft gevonden
	        if (s3 < 100 && teller < 200)
	        {
				
	        	s0 = 100;
	        	vx = s0;
	        	vy = s0;
				vz = s0;
				test = false;
				//vlrsens = false;
				loopteller = 0;
				//vloer = 0;
				joBot.setStatusLeds(false, false, false);
				//System.out.print("lichtsensor-zonderbal:");
				//System.out.println(s3);
				//System.out.print("telleraantal:");
				//System.out.println(teller);
				teller++;
	        } 
	        
	        if (s3 < 100 && teller > 199)
			{ 
	        	s0 = 100;
	        	vx = 0;
	        	vy = s0;	
	        	vz = -s0;
	        	test = false;
	        	//vlrsens = false;
	        	joBot.setStatusLeds(false, false, false);
	        	loopteller++;
	        	//System.out.println(loopteller);
			}
	        
	        if (loopteller > 100)
	        {
	        	teller = 0;
	        }
	        
	        // Als Sensor 3(IR) een waarde ontvangt die groter dan 500 is
	        // dan zal deze om de bal heen draaien om de positie op het veld
	        // te bepalen
	        if (s3 > 500)
	        {
	        	
	        	if(test == false)
	        	{
		        	s0 = 100;
		        	vx = -s0;
		        	vy = 0;
		        	vz = s0;
		        	//vloer = s4;
		        	//System.out.print("vloersensor:");
		        	//System.out.println(vloer);
		        	test = true;
		        	//vlrsens = true;
	        	}
	        	
	           /* if (vlrsens == false)
	            {
	        	
	            // Als Sensor 4(vloer) groter is dan vloer
	            // zal hij naar de bal toe gaan
	            	
	        	if (s4 > vloer && vloer > 1)
	            {
	    			s2 = 100;
	            	vx = 0;
	            	vy = s2;
	    			vz = -s2;
	    			System.out.println("gebruikvloersensor1");
	        	    vloer = 0;
	    		} 
	           
	        	// Als Sensor 4(vloer) kleiner is dan vloer
	        	// zal hij zich om de bal heendraaien
	        	// hij print de waarde van 
	            	
	            	if (s4 < vloer && vloer > 1)
	            	
	            	{
	  			s2 = 100;
	    			vx = -s2;
	    			vy = 0;
	    			vz = s2;
	    			System.out.println("gebruikvloersensor2"); 
	    			vloer = 0;
	         	}  
	       } */
	    }  
	       
	       
	        //Als Sensor 2 een waarde groter dan 700 ontvangt
	        //zal de robot naar achter rijden om een object te ontwijken
	        if (s1 > 500 && s1 > s0 && s1  > s2)
	        {
				//vloer = 0;
	        	s1 = 100;
	        	vx = -s1;
				vy = 0;
				vz = s1;
	        }
	        
	        //Als Sensor 2 een waarde groter dan 700 ontvangt
	        //zal de robot naar achter rijden om een object te ontwijken
	        if (s2 > 500 && s2 > s0 && s2  > s1)
	        {
				s2 = 100;
	        	vx = s2;
	        	vy = -s2;
				vz = 0;
			} 
	       
	        
		   joBot.drive(vx, vy, vz);
		}
		
		public void doBehavior() {
			doeiets();
		}
	}



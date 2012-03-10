package javaBot;

//TODO: Check if current robot has a sound sensor
//TODO: If sound sensor, show and process graphics window
//TODO: Check if robot has a mouse sensor
//TODO: If mouse sensor, display image and coordinates
//TODO: If sound sensor, check if real or simulated sensor
//TODO: Generate sound patterns from simulated sensor
//TODO: Simulated sound sensor must read in patterns from file and display
//TODO: If ultrasonic sensor, display only first graph
//TODO: Define method to select simulated or real sensor
//TODO: Include Sampler and FFT in UVM robot
//TODO: Use samples and select files in simulator
//TODO: If graph is closed, stop collecting data
//TODO: Graph display only on request
//TODO: Include new sensors in system
//TODO: Bij wisseling van agent treden er problemen op
//TODO: Implement selection of robot type

/**
 * Ver 0.0 - 11-08-2004 Ver 0.1 - 03-07-2004 -     Implemented Servo values
 * Included DoCommand webservice interfaces (preliminary)
 */
import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.FlowLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.WindowAdapter;
import java.awt.event.WindowEvent;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;

import javaBot.sensors.SensorServer;

import javax.swing.ImageIcon;
import javax.swing.JButton;
import javax.swing.JCheckBox;
import javax.swing.JFileChooser;
import javax.swing.JFrame;
import javax.swing.JMenu;
import javax.swing.JMenuBar;
import javax.swing.JMenuItem;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.Timer;
import javax.swing.WindowConstants;
import javax.swing.filechooser.FileFilter;

/**
 * Shows various info items about the status of a robot and allows for
 * interaction with the robot.
 */
public class GraphGUI implements IGUI
{
	private static final int		SAMPLESIZE				= 128;

	//Window title
	private String					WINDOW_TITLE			= "RobotGUI";
	
	//standard visibility of window
	private static final boolean	GRAPH_DEFAULT_VISIBLE	= true;

	// Actual window
	private SensorServer			theServer;
	private JFrame					graph;

	// What robot this GUI belong to
	private Robot					robot;

	// Timer for timed value updates
	private Timer					t;

	//  Five colored leds
	private JButton[]				recStatus				= new JButton[5];
	private String[]				chars					= {"A", "E", "I", "O", "U"};

	//Graphs on window
	private GraphPlot				graphOrg				= new GraphPlot();
	private GraphPlot				graphFFT				= new GraphPlot();
	
//	Set sensor simulated or not
	private JCheckBox				sensorSimul				= new JCheckBox(
																	"Simulate external sensor", false);

	//Items voor in het menu
	private JMenuItem				bestand_open			= new JMenuItem("Open meting");
	private JMenuItem				bestand_save			= new JMenuItem("Save meting");

	//Knoppen om meting te starten en stoppen
	private ImageIcon				iconStart				= new ImageIcon(
																	"./src/javaBot/resources/play.gif");
	private ImageIcon				iconStop				= new ImageIcon(
																	"./src/javaBot/resources/pause.gif");
	private JButton					meting_running			= new JButton(iconStart);

	//Boolean die aangeeft of grafieken aan het meten zijn
	boolean							measuring				= false;
	private RobotGUIActionListener	robotGUIActionlistener	= new RobotGUIActionListener();
	private RobotGUI				robotGUI				= null;
	
	// Analyser
	
	private SpectrumAnalyser 		analyser 				= new SpectrumAnalyser();

	/**
	 * Default constructor
	 *
	 * @param robot The robot to link to this GUI
	 * @param robotGUI Reference to the robotGUI
	 */
	public GraphGUI(Robot robot, RobotGUI robotGUI)
	{
		super();
		this.robot = robot;
		WINDOW_TITLE += (" for " + robot.name);

		this.robotGUI = robotGUI;

		//Create and set up window for graphs
		graph = new JFrame("Audiograph");

		graph.setDefaultCloseOperation(WindowConstants.DO_NOTHING_ON_CLOSE);

		graph.addWindowListener(new WindowAdapter()
		{
			public void windowClosing(WindowEvent e)
			{
				setVisible(false);
			}
		});

		init();
	}

	/**
	 * Created on 20-02-2006
	 * Copyright: (c) 2006
	 * Company: Dancing Bear Software
	 *
	 * @version $Revision$
	 * last changed 20-02-2006
	 *
	 * ActionListener from the GraphGUI. Listens to the generated events
	 * and handles accordingly.
	 */
	private class RobotGUIActionListener implements ActionListener
	{
		/**
		 * actionPerformed method
		 *
		 * @param arg0 ActionEvent that generated by the window
		 */
		public void actionPerformed(ActionEvent arg0)
		{
			//if RobotGUI is closed, then the grapGUI also needs to be closed
			if( !robotGUI.isVisible() && !robotGUI.isVisibleInLeftFrame() ) graph.setVisible(false);
			
			if (arg0.getSource() == bestand_open)
			{
				openMeting();

				return;
			}
			else if (arg0.getSource() == bestand_save)
			{
				saveMeting();

				return;
			}

			if (arg0.getSource() == meting_running)
			{
				if (measuring)
				{
					stopMeasuring();
				}
				else
				{
					boolean isSensorSimulated = getSensorSimulated();
					theServer = new SensorServer(isSensorSimulated); // Start server
					measuring = true;
					meting_running.setIcon(iconStop);
				}

				return;
			}
			
			if (arg0.getActionCommand() == null)
			{
				// Refresh timer
				updateReadings();

				return;
			}
			
			
		}

	}
	
	private void stopMeasuring()
	{
		meting_running.setIcon(iconStart);
		measuring = false;
		theServer = null;
	}	

	/* Functie die een meting opent uit een ini-bestand
	 * afkomstig van de harde schijf
	 */
	private void openMeting()
	{
		//Venster om een bestand te selecteren om te openen
		JFileChooser fc = new JFileChooser();

		//Het filter dat ervoor zorgt dat je alleen 
		//ini-files kan selecteren
		myFileFilter filter = new myFileFilter();

		//String waarin net gelezen data van de file terechtkomt
		String line;

		//Variabelen die huidige indexen in de file bijhouden
		int index;

		//Variabelen die huidige indexen in de file bijhouden
		int eindindex;

		//Variabelen die huidige indexen in de file bijhouden
		int metingnr = 0;

		//Variabelen die bijhoudt heoveel samples dit bestand heeft
		//Momenteel ondersteunt het echter alleen files met 128 
		//samples
		int samples;

		//Arrays waarin de data wordt opgeslagen
		//Deze is ter grootte van het aantal samples
		byte[] meting = new byte[SAMPLESIZE];
		byte[] metingFFT = new byte[SAMPLESIZE];

		//Stel de filefilter in op het dialoogvenster 
		//waar de file wordt gekozen
		fc.setFileFilter(filter);

		//Haal de geselecteerde file op
		File selFile = fc.getSelectedFile();

		try
		{
			//Het opzetten van de leesfuncties
			FileReader fr = new FileReader(selFile);
			BufferedReader br = new BufferedReader(fr);

			//Het lezen van de eerste regel van het bestand
			line = br.readLine();

			//Eerste regel is een voorgedefineerde header
			if (!line.equals("[Meting]"))
			{
				//Komt deze niet overeen is de file ongeldig
				JOptionPane.showMessageDialog(null, "Invalid file formaat", "Open file failed",
						JOptionPane.ERROR_MESSAGE);
			}

			//Tweede regel zijn het aantal samplesin het bestand
			line = br.readLine();

			//Zoek het '=' teken op. Hierachter staat de waarde van
			//Het aantal samples
			for (index = 0; index < line.length(); index++)
			{
				if (line.charAt(index) == '=')
				{
					index++;

					break;
				}
			}

			//Zet deze string om in een getal
			samples = Integer.parseInt(line.substring(index));

			if (samples != 128)
			{
				//Het aantal samples moet momenteel 128 zijn
				JOptionPane.showMessageDialog(null, "Samplesize doesn't match 128",
						"Open file failed", JOptionPane.ERROR_MESSAGE);
			}

			//Derde regel zijn de samples van het originele data
			line = br.readLine();

			//Zoek eerst het '=' teken
			for (index = 0; index < line.length(); index++)
			{
				if (line.charAt(index) == '=')
				{
					index++;

					break;
				}
			}

			//Hierna begint de data            
			for (eindindex = index; eindindex < line.length(); eindindex++)
			{
				//Bij de komma is het getal afgelopen
				if (line.charAt(eindindex) == ',')
				{
					//En kan je de data op zijn plek in de array  
					//zetten
					meting[metingnr] = Byte.parseByte(line.substring(index, eindindex));
					index = eindindex + 1;
					metingnr++;
				}
			}

			//Hierna de data van de FFT. Deze leest op dezelfde wijze 
			//als hiervoor
			line = br.readLine();
			metingnr = 0;

			for (index = 0; index < line.length(); index++)
			{
				if (line.charAt(index) == '=')
				{
					index++;

					break;
				}
			}

			for (eindindex = index; eindindex < line.length(); eindindex++)
			{
				if (line.charAt(eindindex) == ',')
				{
					metingFFT[metingnr] = Byte.parseByte(line.substring(index, eindindex));
					index = eindindex + 1;
					metingnr++;
				}
			}
		}
		catch (Exception e)
		{
			//Bij een fout moet de procedure worden afgebroken
			JOptionPane.showMessageDialog(null, e.getMessage(), "Open file failed",
					JOptionPane.ERROR_MESSAGE);

			return;
		}

		//Stel de data in op de grafieken
		graphOrg.setPlotValues(meting);
		graphFFT.setPlotValues(metingFFT);
	}

	/* Functie die de hidige meting opslaat in een ini-file */
	private void saveMeting()
	{
		//Venster om een bestand te selecteren om te openen
		JFileChooser fc = new JFileChooser();

		//Het filter dat ervoor zorgt dat je alleen 
		//ini-files kan selecteren
		myFileFilter filter = new myFileFilter();

		//Stel filter in op het venster
		fc.setFileFilter(filter);

		//Haal de geselecteerde file op
		File selFile = fc.getSelectedFile();

		try
		{
			//Het opzetten van de leesfuncties
			FileWriter fr = new FileWriter(selFile);
			BufferedWriter br = new BufferedWriter(fr);

			//Schrijf eerste voorgedefinieerde header
			br.write("[Meting]");
			br.newLine();

			//Schrijf het aantal samples, momenteel standaard 128
			br.write("Samples=" + 128);
			br.newLine();

			//Schrijf originele data
			br.write("Org=" + graphOrg.getData(0));

			for (int i = 0; i < 128; i++)
			{
				br.write("," + graphOrg.getData(i));
			}

			br.newLine();

			//Schrijf FFT data
			br.write("FFT=" + graphFFT.getData(0));

			for (int i = 0; i < 128; i++)
			{
				br.write("," + graphFFT.getData(i));
			}

			br.newLine();

			//Sluit File
			br.close();
		}
		catch (Exception e)
		{
			//Bij storing procedure afbreken
			JOptionPane.showMessageDialog(null, e.getMessage(), "Open file failed",
					JOptionPane.ERROR_MESSAGE);
		}
	}

	/**
	 * Start the GUI
	 */
	public void init()
	{
		Debug.printInfo("Starting RobotGUI for " + robot.name);

		//Make panel for our graphs
		JPanel Graphs = new JPanel();

		//Make panel for our detectors
		JPanel Detectors = new JPanel();

		//Set up graph panel
		Graphs.setVisible(GRAPH_DEFAULT_VISIBLE);
		Graphs.setLayout(new java.awt.GridLayout(1, 2));

		// Initialize grahps to show

		graphOrg.setPlotStyle(GraphPlot.SIGNAL);
		graphOrg.setTracePlot(true);

		graphFFT.setPlotStyle(GraphPlot.SPECTRUM);
		graphFFT.setTracePlot(false);

		//And add Graphs onto it
		Graphs.add(graphOrg);
		Graphs.add(graphFFT);
		Graphs.setVisible(GRAPH_DEFAULT_VISIBLE);

		Detectors.setLayout(new java.awt.GridLayout(1, 5));

		//Maak indicatoren
		for (int i = 0; i < recStatus.length; i++)
		{
			recStatus[i] = new JButton(chars[i]);
			Detectors.add(recStatus[i]);
		}

		//Set up contentPane
		graph.getContentPane().setLayout(new BorderLayout());
		graph.getContentPane().add(Graphs, BorderLayout.CENTER);
		graph.getContentPane().add(Detectors, BorderLayout.SOUTH);

		JPanel vulling = new JPanel();
		graph.getContentPane().add(vulling, BorderLayout.NORTH);
		vulling.setLayout(new FlowLayout(FlowLayout.LEFT));
		vulling.add(meting_running);
		meting_running.setIcon(iconStart);

		//Make menu
		JMenuBar menu = new JMenuBar();
		graph.setJMenuBar(menu);

		JMenu bestand = new JMenu("File");
		menu.add(bestand);
		bestand.add(bestand_open);
		bestand_open.addActionListener(robotGUIActionlistener);
		bestand.add(bestand_save);
		bestand_save.addActionListener(robotGUIActionlistener);
		meting_running.addActionListener(robotGUIActionlistener);

//		Add checkbox for simulated sensor
		graph.getContentPane().add(sensorSimul, BorderLayout.SOUTH);
//		Listen to checkbox
		sensorSimul.addActionListener(robotGUIActionlistener);
		
		//And the SIZE
		graph.setSize(900, 300);
		graph.setVisible(GRAPH_DEFAULT_VISIBLE);

		// Initialize timer
		t = new Timer(50, robotGUIActionlistener);
		t.start();
	}

	// Update all the readings
	private void updateReadings()
	{
		// Alleen als je aan het meten bent
		if (measuring)
		{
			updateGraphData();
			//updateSpraak();
		}
	}

	//Functie die grafieken up to date houdt
	private void updateGraphData()
	{
		byte[] result = new byte[SAMPLESIZE];

		byte[] originalData;
		byte[] convertedData;
		// Needed to resolve signed/unsigned issue
		originalData = theServer.getSensorData(1);
		
		//originalData = new byte[64];
		// Convert the data
		convertedData = FastFourierTransform.convertData(originalData);
		//convertedData = originalData;
		// Kopieer in nieuwe array
		for (int i = 0; i < convertedData.length && i < SAMPLESIZE; i++)
		{
			result[i] = convertedData[i];
		}
		// De rest wordt 0 gemaakt.
		for (int i = originalData.length; i < SAMPLESIZE; i++)
		{
			result[i] = 0;
		}

		// Set de data in de grafiek
		graphOrg.setPlotValues(result);

		FastFourierTransform fft = new FastFourierTransform();

		
		/*
		 * Change:
		 * We not read the result into a special variable, so that it
		 * can be passed along to our own module for speech recognition.
		 * After that, it is plotted to the screen as normal.
		 */
		byte[] plotValues = fft.fftMag(result);
		
		analyser.update(plotValues);
		
		graphFFT.setPlotValues(plotValues);
	}

	//Functie die leds aanzet indien er een letter is herkend
	private void updateSpraak()
	{
		//Deze methode is overgenomen van het CVI project en
		//licht aangepast, commentaar zie aldaar
		int[] piekenDicht = {1, 1};
		int[] piekenVer = {45, 45};
		final double STORING = .5;
		final double STORING_VER = 0.35;
		final int WILLEKEUR = 5;

		recStatus[0].setBackground(Color.LIGHT_GRAY);
		recStatus[1].setBackground(Color.LIGHT_GRAY);
		recStatus[2].setBackground(Color.LIGHT_GRAY);
		recStatus[3].setBackground(Color.LIGHT_GRAY);
		recStatus[4].setBackground(Color.LIGHT_GRAY);

		byte[] speechData = graphFFT.getData();

		for (int i = 1; i < 22; i++)
		{
			if (speechData[i] > speechData[piekenDicht[0]])
			{
				piekenDicht[0] = i;
			}
			else if (speechData[i] > speechData[piekenDicht[1]])
			{
				piekenDicht[1] = i;
			}
		}

		for (int i = 23; i < 45; i++)
		{
			if (speechData[i] > speechData[piekenVer[0]])
			{
				piekenVer[0] = i;
			}
			else if (speechData[i] > speechData[piekenVer[1]])
			{
				piekenVer[1] = i;
			}
		}

		if (piekenVer[0] < STORING)
		{
			//Nothing
		}
		else if (piekenDicht[0] < STORING_VER)
		{
			//Nothing
		}
		else if (((piekenDicht[0] - piekenDicht[1]) > WILLEKEUR) && (piekenDicht[1] > STORING))
		{
			//Nothing
		}
		else if (((piekenVer[0] - piekenVer[1]) > WILLEKEUR) && (piekenVer[1] > STORING_VER))
		{
			//Nothing
		}
		else if ((piekenDicht[0] >= 10) && (piekenDicht[0] <= 15)
				&& (speechData[piekenDicht[0]] > STORING))
		{
			recStatus[0].setBackground(Color.GREEN);
		}
		else if ((piekenDicht[0] >= 5) && (piekenDicht[0] <= 8)
				&& (speechData[piekenDicht[0]] > STORING) && (piekenVer[0] >= 25)
				&& (piekenVer[0] <= 35) && (speechData[piekenVer[0]] > STORING_VER))
		{
			recStatus[1].setBackground(Color.GREEN);
		}
		else if ((piekenDicht[0] >= 2) && (piekenDicht[0] <= 4)
				&& (speechData[piekenDicht[0]] > STORING) && (piekenVer[0] >= 30)
				&& (piekenVer[0] <= 45) && (speechData[piekenVer[0]] > STORING_VER))
		{
			recStatus[2].setBackground(Color.GREEN);
		}
		else if ((piekenDicht[0] >= 2) && (piekenDicht[0] <= 4)
				&& (speechData[piekenDicht[0]] > STORING))
		{
			recStatus[4].setBackground(Color.GREEN);
		}
		else if ((piekenDicht[0] >= 5) && (piekenDicht[0] <= 8)
				&& (speechData[piekenDicht[0]] > STORING))
		{
			recStatus[3].setBackground(Color.GREEN);
		}
	}

	/**
	 * Sets the visibility of this GUI
	 *
	 * @param visible boolean if visible (true) or not (false)
	 */
	public void setVisible(boolean visible)
	{
		graph.setVisible(visible);
		if(!visible) 
		{
			stopMeasuring();
		}
	}

	/**
	 * Destroys this frame
	 */
	public void destroy()
	{
		graph.dispose();
		t.stop();
	}
	
	/**
	 * Get the status of the "sensor simulated" checkbox in the robotGUI
	 *
	 * @return boolean - true if the checkbox is set (sensorvalues will be generated by simulator)
	 * @see GraphGUI#robotGUIActionlistener
	 */
	public boolean getSensorSimulated()
	{
		return sensorSimul.isSelected();
	}

}


/**
 * Created on 20-02-2006
 * Copyright: (c) 2006
 * Company: Dancing Bear Software
 *
 * @version $Revision$
 * last changed 20-02-2006
 *
 * TODO CLASS: DOCUMENT ME! 
 */

class myFileFilter extends FileFilter
{
	/**
	 * TODO METHOD: DOCUMENT ME!
	 *
	 * @param f TODO PARAM: param description
	 *
	 * @return $returnType$ TODO RETURN: return description
	 */
	public boolean accept(File f)
	{
		if (f.isDirectory())
		{
			return true;
		}

		String filename = f.getName();

		return filename.endsWith(".ini");
	}

	/**
	 * TODO METHOD: DOCUMENT ME!
	 *
	 * @return String returns description
	 */
	public String getDescription()
	{
		return "Meting bestanden";
	}
}

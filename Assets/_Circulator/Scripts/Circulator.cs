/**
 * Circulator.cs
 * 
 * 0. download Arduino IDE from http://arduino.cc/ and install driver
 * 1. select Edit > Project Settings > Player, and set API Compatibility Level to ".NET 2.0" (NOT ".NET 2.0 Subset").
 * 2. create configuration file "com.conf" in Asset folder (for Editor) or PROJECT_Data folder (for exe)
 * 3. connect arduino and check COM port number by device manager
 * 4. write COM port number to configuration file. (ex. COM3)
 * 5. call Circulate(deviceNumber, commmand). circulate!
 */

using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Text;

public class Circulator : MonoBehaviour
{
	public string configFileName = "com.conf";
	public string defaultPortName = "COM3";
	public int maxCirculators = 2;
	public int baudRate = 9600;
	public int writeTimeout = 100;

	public enum Command {
		Stop     = 1,
		Weak     = 2,
		Strong   = 3
	};

	string portName_ = "";
	SerialPort serialPort_ = null;

	public void Circulate (int circulatorNumber, Command command)
	{
		if ((serialPort_ == null) || !serialPort_.IsOpen) { return; }
		if (circulatorNumber < 0 || circulatorNumber >= maxCirculators) { return; }

		string com = (circulatorNumber * System.Enum.GetNames(typeof(Command)).Length + (int)command).ToString();

		try {
			serialPort_.Write(com);
			Debug.Log(string.Format("write : {0} ({1}, {2})", com, circulatorNumber, command));
		} catch (System.TimeoutException) {
			Debug.LogWarning(string.Format("timeouted to write : {0} ({1}, {2})", com, circulatorNumber, command));
		} catch (System.IndexOutOfRangeException) {
			Debug.LogWarning(string.Format("failed to write : {0} ({1}, {2})", com, circulatorNumber, command));
			//serialPort_.Close();
			serialPort_ = null;
		}
	}

	public void CirculateAll (Command command)
	{
		for (int i = 0; i < maxCirculators; i++) {
			Circulate(i, command);
		}
	}

	void Start ()
	{
		string path = Application.dataPath + "/" + configFileName;
		StreamReader reader = null;
		try {
			reader = new StreamReader(path, Encoding.ASCII);
		} catch (FileNotFoundException) {
			reader = null;
		}

		if (reader != null) {
			Debug.Log("config found : " + path);
			portName_ = reader.ReadLine().Trim();
		} else {
			Debug.LogWarning("config not found : " + path);
			portName_ = defaultPortName;
		}

		if ((serialPort_ = new SerialPort(portName_, baudRate)) != null) {
			serialPort_.WriteTimeout = writeTimeout;
			try {
				serialPort_.Open();
				Debug.Log("open : " + portName_);
			} catch (IOException) {
				serialPort_ = null;
				Debug.LogWarning("failed to open : " + portName_);
			}
		} else {
			Debug.LogWarning("failed to open : " + portName_);
		}
	}

	void OnDestroy ()
	{
		if (serialPort_ != null) {
			CirculateAll(Command.Stop);

			serialPort_.Close();
			serialPort_ = null;

			Debug.Log("close : " + portName_);
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ConfigLoader{

	public string configFile { get; private set; }

	public Dictionary<ArrayList,Vector3> timeTable { get; private set; }
	public Dictionary<Vector3, float> randomPlaces { get; private set; }

	public float satietyFactor { get; private set; }
	public float moodFactor { get; private set; }
	public float hygieneFactor { get; private set; }

	public Vector3 satietyLocation { get; private set; }
	public Vector3 moodLocation { get; private set; }
	public Vector3 hygieneLocation { get; private set; }

	public bool isReady { get; private set; }

	public ConfigLoader(string path)
	{
		configFile = path;
	}

	// Use this for initialization
	public void Initialise () 
	{
		this.randomPlaces = new Dictionary<Vector3, float> ();
		this.timeTable = new Dictionary<ArrayList, Vector3> ();
		this.configFile = System.IO.File.ReadAllText (configFile);

		this.InitializetimeTable();
		this.InitializeFactors ();
		this.InitializerandomPlaces ();
		this.InitializeLocations ();

		this.isReady = true;
	}

	void InitializeLocations()
	{
		int startIndex = configFile.IndexOf ("SL");
		int endIndex = configFile.IndexOf (";");

		var locationSubstr = configFile.Substring (startIndex + 3, endIndex - startIndex - 3);
		var locationList = locationSubstr.Split (',');

		satietyLocation = new Vector3 (float.Parse (locationList [0]), float.Parse (locationList [1]), float.Parse (locationList [2]));

		startIndex = configFile.IndexOf ("ML");
		endIndex = configFile.IndexOf ("HL");

		locationSubstr = configFile.Substring (startIndex + 3, endIndex - startIndex - 3 - 3);
		locationList = locationSubstr.Split (',');

		moodLocation = new Vector3 (float.Parse (locationList [0]), float.Parse (locationList [1]), float.Parse (locationList [2]));

		startIndex = endIndex;
		endIndex = configFile.IndexOf ("//");

		locationSubstr = configFile.Substring (startIndex + 3, endIndex - startIndex - 3 - 3);
		locationList = locationSubstr.Split (',');

		hygieneLocation = new Vector3 (float.Parse (locationList [0]), float.Parse (locationList [1]), float.Parse (locationList [2]));
	}

	void InitializeFactors()
	{
		int startIndex = configFile.IndexOf ("SF");
		int endIndex = configFile.IndexOf ("RP");

		string factorString = configFile.Substring (startIndex, endIndex - startIndex);
		endIndex = factorString.LastIndexOf ("\r\n");
		configFile = configFile.Substring (startIndex + endIndex + 2, configFile.Length - endIndex - startIndex - 2);

		startIndex = 0;
		endIndex = factorString.IndexOf (";");
		satietyFactor = float.Parse(factorString.Substring (startIndex + 3, endIndex - startIndex - 3));

		factorString = factorString.Substring (endIndex + 1, factorString.Length - endIndex - 1);
		startIndex = 0;
		endIndex = factorString.IndexOf (";");
		moodFactor = float.Parse (factorString.Substring(startIndex + 5, endIndex - startIndex - 5));

		factorString = factorString.Substring (endIndex + 1, factorString.Length - endIndex - 1);
		startIndex = 0;
		endIndex = factorString.IndexOf (";");
		hygieneFactor = float.Parse (factorString.Substring(startIndex + 5, endIndex - startIndex - 5));

	}

	void InitializerandomPlaces()
	{
		int startIndex = 0;
		int endIndex = configFile.IndexOf ("RP");

		int randomPlacesCount = int.Parse(configFile.Substring(startIndex, endIndex - startIndex));

		startIndex = configFile.IndexOf("{") + 1;
		while (randomPlacesCount > 0) 
		{
			endIndex = configFile.IndexOf(";");

			var positionList = configFile.Substring(startIndex + 2, endIndex - startIndex - 2).Split(',');
			Vector3 position = new Vector3(float.Parse(positionList[0]), float.Parse(positionList[1]), float.Parse(positionList[2]));

			float chance = float.Parse(configFile.Substring(endIndex + 1, configFile.IndexOf(" ") - endIndex - 1));

			randomPlaces.Add(position, chance);

			configFile = configFile.Substring(configFile.IndexOf(" "), configFile.Length - configFile.IndexOf(" "));
			configFile = configFile.Substring(configFile.IndexOf("\r\n"), configFile.Length - configFile.IndexOf("\r\n"));

			randomPlacesCount--;
			startIndex = 0;
		}
	}

	void InitializetimeTable()
	{
		int startIndex = 0;
		int endIndex= configFile.IndexOf ("P");
		int positionCount = int.Parse(configFile.Substring (startIndex, endIndex - startIndex));
		
		startIndex = configFile.IndexOf ("\r\n");
		endIndex = configFile.IndexOf ("}");
		string timetableString = configFile.Substring(startIndex + 2, endIndex - startIndex - 2);

		configFile = configFile.Substring (endIndex + 2, configFile.Length - endIndex - 2);

		while (positionCount > 0)
		{
			startIndex = 0;
			endIndex = timetableString.IndexOf("=");

			var RangeSubstr = timetableString.Substring (startIndex, endIndex - startIndex);
			var RangeList = RangeSubstr.Split (',');

			ArrayList hourRange = new ArrayList ();

			for (int i = 0; i < RangeList.Length; i++)
			{
				hourRange.Add (int.Parse (RangeList [i]));
			}

			startIndex = timetableString.IndexOf (";");
			RangeSubstr = timetableString.Substring (endIndex + 1, startIndex - endIndex - 1);
			RangeList = RangeSubstr.Split (',');

			Vector3 position = new Vector3 (float.Parse (RangeList [0]), float.Parse (RangeList [1]), float.Parse (RangeList [2]));

			timeTable.Add(hourRange,position);
			positionCount--;
			startIndex = timetableString.IndexOf("\r\n");
			timetableString = timetableString.Substring(startIndex + 2, timetableString.Length - startIndex - 2);

		}
	}
}

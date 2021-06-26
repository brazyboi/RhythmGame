using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AppContext  {
	private static AppContext appContext;
    private int instrument = MusicInstrument.FLUTE_INSTRUMENT;
	public int musicNoteDisplayDuration;
    private int instrumentMelody = MusicInstrument.FLUTE_INSTRUMENT;

	public long totalScore = 0;

	private AppContext() {
		musicNoteDisplayDuration = 2000;
	}
		
	public SongItem songItem;  

	public static AppContext instance() {
		if (appContext == null) {
			appContext = new AppContext ();
		}
		return appContext;
	}


	public int getInstrument() {
		return instrument;
	}

	public static bool  isFluteStyle(int i) {
		return i >= MusicInstrument.BLOW_TYPE_INSTRUMENT;
	}

	public static bool isSoundBank(int i) {
		return i>= MusicInstrument.BLOW_TYPE_INSTRUMENT || i == MusicInstrument.ZITHER_INSTRUMENT;
	}

	public bool isCurrentFluteStyle() {
		return isFluteStyle(instrument);
	}
	public bool isCurrentSoundBank() {
		return isSoundBank(instrument);
	}
		
	public void setInstrument(int instrumentId) {
		instrument = instrumentId;
		//sharePianoPlayer()->adjustBaseNoteByInstrument();
	}

	public void setInstrumentMelody(int instrumentId) {
		instrumentMelody = instrumentId;
	}

	public int getInstrumentMelody() {
		return instrumentMelody;
	}

	/*
	private static string RootPath  
	{  
		get{  
			if(Application.platform == RuntimePlatform.IPhonePlayer)  
			{  
				return Application.dataPath +"/Raw/Data/";  
			}  
			else if(Application.platform == RuntimePlatform.Android)  
			{  
				return "jar:file://" + Application.dataPath + "!/assets/Data/"  
				}  
			else  
			{  
				return Application.dataPath + "/StreamingAssets/Data/";  
			}  
		}  
	}  */
	public static byte[] ReadBinaryFile (string name)  
	{  
		byte[] fileContent = null;   
		if(Application.platform == RuntimePlatform.Android)  
		{  
			WWW www = new WWW(Application.streamingAssetsPath + name);  
			while(!www.isDone){};  
			fileContent = www.bytes;  
		}  
		else  
		{  
			try{  
				fileContent = File.ReadAllBytes (Application.streamingAssetsPath + name);  
			}  
			catch(IOException e){  
				return null;  
			}   
		}  
		return fileContent;  
	}  

	public static string ReadTextFile (string name)  
	{  
		string fileContent = null;   
		if(Application.platform == RuntimePlatform.Android)  
		{  
			WWW www = new WWW(Application.streamingAssetsPath + name);  
			while(!www.isDone){};  
			fileContent = www.text;  
		}  
		else  
		{  
			try{  
				fileContent = File.ReadAllText (Application.streamingAssetsPath + name);  
			}  
			catch(IOException e){  
				return null;  
			}   
		}  
		return fileContent;  
	}  

}

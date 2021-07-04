using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Playlist {
	public string name;
	public List<SongItem> list = new List<SongItem>(); 
	public static Playlist loadPlaylist(string file) {
		string jsontext = FileReaderUtils.ReadTextResourceFile (file);
		return JsonUtility.FromJson<Playlist> (jsontext);
	}
}

[System.Serializable]
public class SongItem {
	public string title;
	public string artist;
	public string path;
	public int instr;
	public int melody;    
}

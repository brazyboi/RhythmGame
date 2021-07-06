using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Playlist {
	public string name;
	public List<SongItem> list = new List<SongItem>();

	public static Playlist loadPlaylist(string file, int level) {
		string jsontext = FileReaderUtils.ReadTextResourceFile (file);
		Playlist playlist = JsonUtility.FromJson<Playlist> (jsontext);
		foreach(SongItem item in playlist.list)
        {
			item.level = level;
		}
		return playlist;
	}
}

[System.Serializable]
public class SongItem {
	public string title;
	public string artist;
	public string path;
	public int instr;
	public int melody;
	public int level;
}

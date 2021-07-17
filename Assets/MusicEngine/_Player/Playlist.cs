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
		int index = 0;
		foreach(SongItem item in playlist.list)
        {
			item.level = level * 10 + index;
			Debug.Log("Level: " + item.level + " Song: " + item.title);
			index++;
			if(item.speed < 0.1)
            {
				item.speed = 1.0f;
            }
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
	public int melody; //melody channel. if add 10000, the melody pitch will add 12.  
	public int level;
	public int melodyTrack;// if > 0 , will use it. otherwise use melody
	public float speed;
}

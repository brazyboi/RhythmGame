using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PlayListViewAdaptor : ListViewBaseAdaptor {

	int selectIndex = -1;
	Playlist playlist;

	public List<string> filePaths;

	// Use this for initialization
	void Start () {
		//loadPlaylist ("playlist/playlist_battle_all");
		addFilePaths();
		UnityEngine.Debug.Log("curSongListLevel: " + AppContext.instance().curSongListLevel);
		loadPlaylist(filePaths[AppContext.instance().curSongListLevel], AppContext.instance().curSongListLevel);
	}

	void addFilePaths()
    {
		filePaths.Add("playlist/playlist_battle_en0");
		filePaths.Add("playlist/playlist_battle_en1");
		filePaths.Add("playlist/playlist_battle_en2");
		filePaths.Add("playlist/playlist_battle_en3");
		filePaths.Add("playlist/playlist_battle_en4");
		filePaths.Add("playlist/playlist_battle_en5");
		filePaths.Add("playlist/playlist_battle_en6");
		filePaths.Add("playlist/playlist_battle_en7");
		filePaths.Add("playlist/playlist_battle_en8");
		filePaths.Add("playlist/playlist_battle_en9");
		filePaths.Add("playlist/playlist_battle_en10");
	}

	public void loadPlaylist(string playlistFile, int level) {
		playlist = Playlist.loadPlaylist (playlistFile, level);
	}

	public override int getTotalCount () {
		return playlist.list.Count;
	}

	public override string getCellPrefab () {
		return "PlayListCell";
	}

	public override void bindCellData (int index, Transform prefabCell) {

		Text info = (Text)prefabCell.Find("info").GetComponent<Text>();
		Text title = prefabCell.Find("title").GetComponent<Text>();
		Text accuracy = prefabCell.Find("accuracy").GetComponent<Text>(); 

		if (PlayerData.getSongStatus(playlist.list[index].level) == PlayerData.SongStatus.SONG_LOCKED)
		{
			prefabCell.Find("LockIcon").GetComponent<Image>().enabled = true;
			title.text = "???";
			accuracy.text = "";
			prefabCell.Find("PlayButton").GetComponent<Image>().color = Color.white;

		}
		else if (PlayerData.getSongStatus(playlist.list[index].level) == PlayerData.SongStatus.SONG_PASSED)
        {
			prefabCell.Find("LockIcon").GetComponent<Image>().enabled = false;

			Button button = (Button)prefabCell.GetComponent<Button>();
			button.onClick.AddListener(() => {
				OnSelectItem(index);
			});

			ColorBlock cb = button.colors;
			if (selectIndex == index)
			{
				cb.normalColor = Color.white;

			}
			else
			{
				cb.normalColor = Color.red;
			}
			//button.colors = cb;

			title.text = "" + (index + 1) + ". " + playlist.list[index].title;
			accuracy.fontStyle = FontStyle.Normal;
			accuracy.text = "" + PlayerData.getSongScore(playlist.list[index].path) + "%";
			prefabCell.Find("PlayButton").GetComponent<Image>().color = Color.white;
			//info.text = playlist.list[index].artist;
			//Debug.Log("cur song level " + playlist.list[index].level + " player level: " + PlayerData.getPlayerLevel());
		}
		else if (PlayerData.getSongStatus(playlist.list[index].level) == PlayerData.SongStatus.SONG_IN_PROGRESS)
        {
			prefabCell.Find("LockIcon").GetComponent<Image>().enabled = false;
			prefabCell.Find("PlayButton").GetComponent<Image>().color = Color.green;

			Button button = (Button)prefabCell.GetComponent<Button>();
			button.onClick.AddListener(() => {
				OnSelectItem(index);
			});

			title.text = "" + (index + 1) + ". " + playlist.list[index].title;
			//accuracy.fontStyle = FontStyle.Italic;
			//accuracy.text = "Current";
		}

	}

	private void OnSelectItem(int index) {
		selectIndex = index;
		AppContext.instance().songItem = playlist.list[index];
		GameManager.gotoSongSelect();
	}



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PlayListViewAdaptor : ListViewBaseAdaptor {

	int selectIndex = -1;
	Playlist playlist;

	// Use this for initialization
	void Start () {
		loadPlaylist ("playlist/playlist_battle_all");
	}

	public void loadPlaylist(string playlistFile) {
		playlist = Playlist.loadPlaylist (playlistFile);
	}

	public override int getTotalCount () {
		return playlist.list.Count;
	}

	public override string getCellPrefab () {
		return "PlayListCell";
	}

	public override void bindCellData (int index, Transform prefabCell) {
		Button button = (Button)prefabCell.GetComponent<Button> ();
		button.onClick.AddListener (() => {
			OnSelectItem(index);
		});

		ColorBlock cb = button.colors;
		if (selectIndex == index) {
			cb.normalColor = Color.white;

		} else {
			cb.normalColor = Color.red;
		}
		//button.colors = cb;
		Text info = (Text) prefabCell.Find("info").GetComponent<Text>();
		Text title = prefabCell.Find("title").GetComponent<Text>();
		title.text = "" + (index + 1) + ". " + playlist.list[index].title;
		info.text = playlist.list[index].artist;

	}

	private void OnSelectItem(int index) {
		selectIndex = index;
		AppContext.instance().songItem = playlist.list[index];
		SceneManager.LoadScene ("SongSelect", LoadSceneMode.Single);
	}



}

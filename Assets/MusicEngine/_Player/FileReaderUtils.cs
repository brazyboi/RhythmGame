using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using UnityEngine.Networking;

public class FileReaderUtils  {


	public static byte[] readMidiZipFile(string filename) {
		/*
		 * char path[512];
				strcpy(path, musicFile);
				strcpy(path + strlen(path) - 4, ".sht");
				size = AppContext::sharedContext()->readPackedDataFromAsset(path, &filedata);
				if(size == 0 || filedata==NULL) {
					strcpy(path + strlen(path) - 4, ".mid");
					size = AppContext::readDataFromAsset(path, &filedata);
				}
				*/
		if(filename.EndsWith(".sht")) {

			return null;
		} else {
			return readStreamingAssetFile (filename);
		}
	}


	public static byte[] readSoundFont(string filename) {
		return readStreamingAssetFile (Application.streamingAssetsPath + "/soundfont/" + filename + ".sab");
	}

	public static byte[] readStreamingAssetFile(string filename) {
		#if UNITY_ANDROID 
		WWW wwwfile = new WWW(filename);
		while (!wwwfile.isDone) { }
		return wwwfile.bytes;
		#else
		byte[] data;
		FileInfo info = new FileInfo(filename);
		if (!info.Exists) {
			UnityEngine.Debug.Log("File " + filename + " does not exist");
			return null;
		}
		if (info.Length == 0) {
			UnityEngine.Debug.Log("File " + filename + " is empty (0 bytes)");
			return null;
		}
		FileStream file = File.Open(filename, FileMode.Open, 
			FileAccess.Read, FileShare.Read);
		data = new byte[ info.Length ];
		int offset = 0;
		int len = (int)info.Length;
		while (true) {
			if (offset == info.Length)
				break;
			int n = file.Read(data, offset, (int)(info.Length - offset));
			if (n <= 0)
				break;
			offset += n;
		}
		file.Close();
		return data;
		#endif
	}
}

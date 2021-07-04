using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
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
			filename = filename.Replace(".mid", "");
			return ReadBinaryResourceFile(filename);
		}
	}


	public static byte[] readSoundFont(string filename) {
		UnityEngine.Debug.Log("read sound font: " + filename);
		return ReadBinaryResourceFile("soundfont/" + filename);
	}

	public static byte[] readStreamingAssetFile(string filename) {
	#if UNITY_ANDROID || UNITY_WEBGL
		UnityWebRequest request = UnityWebRequest.Get(filename);
		request.SendWebRequest();
		while (!request.isDone)
		{

		}
		return request.downloadHandler.data;
	/*	WWW wwwfile = new WWW(filename);
		while (!wwwfile.isDone) {
			UnityEngine.Debug.Log("WWW reading......");
		}
		return wwwfile.bytes;
		*/
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

	public static string ReadTextFile(string filename)
	{
		string fileContent = null;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WebGLPlayer)
		{
			//WWW www = new WWW(Application.streamingAssetsPath + name);  
			//while(!www.isDone){};  
			//fileContent = www.text;
			UnityEngine.Debug.Log("UnityWebRequest reading.....");
			UnityWebRequest request = UnityWebRequest.Get(Application.streamingAssetsPath + filename);
			request.SendWebRequest();
			while (!request.isDone)
			{

			}
			return request.downloadHandler.text;

		}
		else
		{
			try
			{
				fileContent = File.ReadAllText(Application.streamingAssetsPath + filename);
			}
			catch (IOException e)
			{
				return null;
			}
		}
		return fileContent;
	}

	public static string ReadTextResourceFile(string filename)
	{
		var textFile = Resources.Load<TextAsset>(filename);
		return textFile.text;
	}

	public static byte[] ReadBinaryResourceFile(string filename)
	{
		UnityEngine.Debug.Log("ReadBinaryResourceFile: " + filename);
		var textFile = Resources.Load<TextAsset>(filename);
		return textFile.bytes;
	}
}

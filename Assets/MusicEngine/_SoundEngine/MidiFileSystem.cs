using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FMOD;
using System;
using System.Runtime.InteropServices;

public class MidiFileSystem
{

	private byte[] mFileBytes;
	private int cur;

	private static byte[] mFileBytesMidiDls;

	public MidiFileSystem() {
		
    }

	private void openMidiDls(string name)
    {
		if(mFileBytesMidiDls == null)
        {
			mFileBytesMidiDls = FileReaderUtils.ReadBinaryResourceFile(name);
		}
		mFileBytes = mFileBytesMidiDls;

	}

	private FMOD.RESULT open(string name, ref uint filesize)
    {
		cur = 0;
		if (name.Contains("xiaimg"))
		{
			openMidiDls(name);
		}
		else
		{
			mFileBytes = FileReaderUtils.ReadBinaryResourceFile(name);
		}
		if (mFileBytes != null)
		{
			UnityEngine.Debug.Log("MidiFileSystem open successfully: length" + mFileBytes.Length);
			filesize = (uint) mFileBytes.Length;
			return FMOD.RESULT.OK;
		}else
        {
			UnityEngine.Debug.Log("MidiFileSystem open failed");
			return FMOD.RESULT.ERR_FILE_NOTFOUND;
        }
	}

	private FMOD.RESULT close()
	{
		mFileBytes = null;
		return FMOD.RESULT.OK;
	}


	private FMOD.RESULT read(IntPtr buffer, uint sizebytes, ref uint bytesread)
    {
		int requestBytes = (int)sizebytes;
		FMOD.RESULT ret = FMOD.RESULT.OK;
		if(requestBytes + cur > mFileBytes.Length)
        {
			requestBytes =  mFileBytes.Length - (int) cur;
			ret = FMOD.RESULT.ERR_FILE_EOF;
		} 

		if(requestBytes > 0)
        {
			bytesread = (uint) requestBytes;
			Marshal.Copy(mFileBytes,(int) cur, buffer, requestBytes);
			cur = cur + requestBytes;
			return ret;
		} else
        {
			bytesread = 0;
			return FMOD.RESULT.ERR_FILE_EOF;
        }
    }


	private FMOD.RESULT seek(uint pos)
	{
		if(pos > mFileBytes.Length)
        {
			return FMOD.RESULT.ERR_FILE_COULDNOTSEEK;
        } else
        {
			cur = (int) pos;
			return FMOD.RESULT.OK;
		}
		
	}


	public static void setupMidiFileSystem(FMOD.System fs)
	{
		UnityEngine.Debug.Log("mFmodSystem setFileSystem");
		FMOD.RESULT res = fs.setFileSystem(MIDI_FILE_OPEN_CALLBACK, MIDI_FILE_CLOSE_CALLBACK, MIDI_FILE_READ_CALLBACK, MIDI_FILE_SEEK_CALLBACK, null, null, -1);
		if (res == FMOD.RESULT.OK)
		{
			UnityEngine.Debug.Log("mFmodSystem setFileSystem successfully");
		}
		else
		{
			UnityEngine.Debug.Log("mFmodSystem setFileSystem failed:" + res);
		}
	}


	[AOT.MonoPInvokeCallback(typeof(FMOD.FILE_OPEN_CALLBACK))]
	public static FMOD.RESULT MIDI_FILE_OPEN_CALLBACK(IntPtr name, ref uint filesize, ref IntPtr handle, IntPtr userdata)
	{

		UnityEngine.Debug.Log("MIDI_FILE_OPEN_CALLBACK");
		string fileName = Marshal.PtrToStringAuto(name);
		UnityEngine.Debug.Log("MIDI_FILE_OPEN_CALLBACK name:" + fileName);
		MidiFileSystem midiFileSystem = new MidiFileSystem();
		GCHandle gch = GCHandle.Alloc(midiFileSystem);
		handle = GCHandle.ToIntPtr(gch);
		UnityEngine.Debug.Log("MIDI_FILE_OPEN_CALLBACK GCHandle");
		FMOD.RESULT ret= midiFileSystem.open(fileName, ref filesize);
		UnityEngine.Debug.Log("MIDI_FILE_OPEN_CALLBACK open file" + filesize);
		return ret;
	}

	private static MidiFileSystem getMidiFileSystemByHandle(IntPtr handle)
    {
		GCHandle gch = GCHandle.FromIntPtr(handle);
		MidiFileSystem fs = (MidiFileSystem)gch.Target;
		return fs;
	}

	[AOT.MonoPInvokeCallback(typeof(FMOD.FILE_CLOSE_CALLBACK))]
	public static FMOD.RESULT MIDI_FILE_CLOSE_CALLBACK(IntPtr handle, IntPtr userdata)
	{
		UnityEngine.Debug.Log("MIDI_FILE_CLOSE_CALLBACK");
		return getMidiFileSystemByHandle(handle).close();
	}

	[AOT.MonoPInvokeCallback(typeof(FMOD.FILE_READ_CALLBACK))]
	public static FMOD.RESULT MIDI_FILE_READ_CALLBACK(IntPtr handle, IntPtr buffer, uint sizebytes, ref uint bytesread, IntPtr userdata)
	{
		UnityEngine.Debug.Log("MIDI_FILE_READ_CALLBACK");
		//return FMOD.RESULT.ERR_FILE_EOF;
		return getMidiFileSystemByHandle(handle).read(buffer, sizebytes, ref bytesread);
	}

	[AOT.MonoPInvokeCallback(typeof(FMOD.FILE_SEEK_CALLBACK))]
	public static FMOD.RESULT MIDI_FILE_SEEK_CALLBACK(IntPtr handle, uint pos, IntPtr userdata)
	{
		UnityEngine.Debug.Log("MIDI_FILE_SEEK_CALLBACK");
		return getMidiFileSystemByHandle(handle).seek(pos);
	}

	[AOT.MonoPInvokeCallback(typeof(FMOD.FILE_ASYNCREAD_CALLBACK))]
	public static FMOD.RESULT MIDI_FILE_ASYNCREAD_CALLBACK(IntPtr info, IntPtr userdata)
	{
		UnityEngine.Debug.Log("MIDI_FILE_ASYNCREAD_CALLBACK");
		return FMOD.RESULT.OK;
	}

	[AOT.MonoPInvokeCallback(typeof(FMOD.FILE_ASYNCCANCEL_CALLBACK))]
	public static FMOD.RESULT MIDI_FILE_ASYNCCANCEL_CALLBACK(IntPtr info, IntPtr userdata)
	{
		UnityEngine.Debug.Log("MIDI_FILE_ASYNCCANCEL_CALLBACK");
		return FMOD.RESULT.OK;
	}

	[AOT.MonoPInvokeCallback(typeof(FMOD.FILE_ASYNCDONE_FUNC))]
	public static FMOD.RESULT MIDI_FILE_ASYNCDONE_FUNC(IntPtr info, RESULT result)
	{
		UnityEngine.Debug.Log("MIDI_FILE_ASYNCDONE_FUNC");
		return FMOD.RESULT.OK;
	}





}

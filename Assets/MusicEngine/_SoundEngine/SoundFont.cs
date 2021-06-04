using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

public class WaveFormat
{
	public ushort  format;
	public ushort  channels;            //音频通道数                   1：单声道  2：立体声
	public ulong   samplerate;          //声音采样频率
	public ushort  bytes_per_sample;    //每帧数据大小（单声道）
	public ushort  block_align;
	public ulong   data_size;           //数据总长度

};

public class NoteDataCache {
	public byte[] dataNote;
	public WaveFormat waveFormat;
};


public class SoundFont  {


	private NoteDataCache[] noteDataCache = new NoteDataCache[15];
	private int noteDataCacheCount = 15;
	private string rootFolder;
	private int instrument;

	private string[] fileNotes;
	private int baseNoteCount;
	private int[,] instrumentNoteSettings;

	//flute61： 36860 - loop：22365-36828  （41~64）
	//flute70： 31871 - loop: 20205 - 31839 （65~73）
	//flute76：  38053 loop: 26511 - 38021（74~78）
	//flute79： 36039 - loop：25429-36007  （79~83）
	//flute85：  33815 - loop: 27948 - 33783（84~89）
	//flute91：  51288 - loop: 45302 - 51256 (90 ~ ....)
	private const int SIZE_LONG          =             4;
	private const int SIZE_SHORT         =             2;
	private const int SIZE_ID           =               4;
	private const int BITS_PER_BYTE      =             8;

	private const int WAVE_FORMAT_PCM             =    (0x0001);


	//flute
	private const int INSTRUMENT_NOTE_NUMBER_FLUTE = 6;


	private readonly string[] fileNote_Flute = {
		"f_61",
		"f_70",
		"f_76",
		"f_79",
		"f_85",
		"f_91",

	};


	private readonly int[,] instrumentNoteSettings_Flute =    {
		{61, 40, 64, 22365, 36828},  //base note, start note, end note, loopstart, loopend
		{70, 65, 73, 20205, 31839},
		{76, 74, 78, 26511, 38021},
		{79, 79, 83, 25429, 36007},
		{85, 84, 89, 27948, 33517 }, //33783},
		{91, 90, 103, 45302, 51235} //51256}
	};


	private const int INSTRUMENT_NOTE_NUMBER_OCARINA = 2;

	private readonly string[] fileNote_Ocarina = {
		"ocarina_76",
		"ocarina_93",


	};

	private readonly int[,] instrumentNoteSettings_Ocarina  = {

		{76, 45 ,84 , 29195, 43538},
		{93, 85, 108, 14513 , 29025},
	};




	//pan flute
	private const int INSTRUMENT_NOTE_NUMBER_PANFLUTE = 2;


	private string[] fileNote_PanFlute = {
		"pf_71",
		"pf_90"

	};

	private readonly int[,] instrumentNoteSettings_PanFlute = {
		{71,40 ,94 , 16118, 44026},
		{90, 95, 103, 5380 , 6154}
	};

	//pan flute
	private const int INSTRUMENT_NOTE_NUMBER_XIAO = 1;


	private readonly string[] fileNote_Xiao = {
		"xiao_86"

	};

	private readonly int[,] instrumentNoteSettings_Xiao = {
		{86, 45 ,102 , 6625, 21792}
	};

	private const int INSTRUMENT_NOTE_NUMBER_PIANO = 17;

	private readonly string[] fileNote_Piano = {
		"pp_c0",
		"pp_f0",
		"pp_b0",
		"pp_d1",
		"pp_f#1",
		"pp_a1",
		"pp_c#2",
		"pp_e2",
		"pp_a2",
		"pp_c#3",
		"pp_f3",
		"pp_g#3",
		"pp_d4",
		"pp_g4",
		"pp_b4",
		"pp_d5",
		"pp_f5"



	};

	private readonly int[,] instrumentNoteSettings_Piano  = {
		{24, 10 , 27, 0, 0},
		{29, 28 ,33 , 0, 0},
		{35, 34 ,36 , 0, 0},
		{38, 37 ,39 , 0, 0},
		{42, 40 ,44 , 0, 0},
		{45, 45 ,47 , 0, 0},
		{49, 48 ,51 , 0, 0},
		{52, 52 ,54 , 0, 0},
		{57, 55 ,59 , 0, 0},
		{61, 60 ,63 , 0, 0},
		{65, 64 ,66 , 0, 0},
		{68, 67 ,72 , 0, 0},
		{74, 73 ,76 , 0, 0},
		{79, 77 ,79 , 0, 0},
		{83, 80 ,84 , 0, 0},
		{86, 85 ,87 , 0, 0},
		{89, 88 ,118 , 0, 0}


	};


	private const int INSTRUMENT_NOTE_NUMBER_VIOLIN  = 9;


	private readonly string[] fileNote_Violin = {
		"violin_63",
		"violin_66",
		"violin_70",
		"violin_73",
		"violin_77",
		"violin_78",
		"violin_83",
		"violin_87",
		"violin_90",

	};

	private readonly int[,] instrumentNoteSettings_Violin  = {
		{63, 42, 60, 21418, 36345},  //base note, start note, end note, loopstart, loopend
		{66, 61, 67, 20288, 35267},
		{70, 68, 71, 23535, 38123},
		{73, 72, 75, 14809, 29808},
		{77, 76, 77, 22608, 35558},
		{78, 78, 80, 22892, 37260},
		{83, 81, 84, 23850, 37532},
		{87, 85, 89, 20578, 34039},
		{90, 90, 105, 28628, 40669}
	};




	private const int INSTRUMENT_NOTE_NUMBER_DIZI = 5;


	private readonly string[] fileNote_Dizi = {

		"dizi_82",
		"dizi_86",
		"dizi_90",
		"dizi_94",
		"dizi_98"


	};

	private readonly int[,] instrumentNoteSettings_Dizi  = {
		{82, 21 ,84 , 14546 , 19530},
		{86, 85 ,88 , 9577 , 14785},
		{90, 89 ,92 , 9016 , 14264},
		{94, 93 ,95 , 8901 , 14399},
		{98, 96 ,108 , 8453 , 13982},
	};


	private const int INSTRUMENT_NOTE_NUMBER_SAX = 7;


	private readonly string[] fileNote_Sax = {
		"sax_51",
		"sax_55",
		"sax_62",
		"sax_65",
		"sax_69",
		"sax_72",
		"sax_77"



	};

	private readonly int[,] instrumentNoteSettings_Sax = {

		{51, 42 ,53 , 67382, 73310},
		{55, 54 ,58 , 53471, 59044},
		{62, 59 ,63 , 49673, 53249},
		{65, 64 ,67 , 45262, 53036},
		{69, 68 ,70 , 76101, 78594},
		{72, 71 ,74 , 42128, 44133},
		{77, 75 ,127 , 61760, 66317},
	};

	private const int INSTRUMENT_NOTE_NUMBER_TRUMPET = 7;


	private readonly string[] fileNote_Trumpet = {
		"trumpet_53",
		"trumpet_60",
		"trumpet_67",
		"trumpet_67",
		"trumpet_79",
		"trumpet_83",
		"trumpet_85",

	};

	private readonly int[,] instrumentNoteSettings_Trumpet = {
		{54, 40 ,56 , 32350, 42409},
		{60, 57 ,64 , 34054, 41265},
		{67, 65 ,70 , 22549, 28474},
		{67, 71 ,75 , 22549, 28474},
		{79, 76 ,80 , 34303, 40716},
		{83, 81 ,89 , 21155, 34859},
		{85, 90 ,108 , 16655, 31974},
	};

	private const int INSTRUMENT_NOTE_NUMBER_ZITHER = 10;


	private readonly string[] fileNote_Zither = {
		"zheng_48",
		"zheng_53",
		"zheng_60",
		"zheng_65",
		"zheng_72",
		"zheng_77",
		"zheng_84",
		"zheng_89",
		"zheng_96",
		"zheng_101",


	};

	private readonly int[,] instrumentNoteSettings_Zither  = {
		{48, 48 ,52 , 0, 0},
		{53, 53 ,59 , 0, 0},
		{60, 60 ,64 , 0, 0},
		{65, 65 ,71 , 0, 0},
		{72, 72 ,76 , 0, 0},
		{77, 77 ,83 , 0, 0},
		{84, 84 ,88 , 0, 0},
		{89, 89 ,95 , 0, 0},
		{96, 96 ,100 , 0, 0},
		{101, 101 ,112 , 0, 0},

	};



	private const int INSTRUMENT_NOTE_NUMBER_HARMONICA = 3;


	private readonly string[] fileNote_Harmonica = {

		"harmonica_60",
		"harmonica_65",
		"harmonica_72",

	};

	private readonly int[,] instrumentNoteSettings_Harmonica  = {
		{60, 48 ,63 , 28927 , 31977},
		{65, 64 ,70 , 22478 , 27050},
		{72, 71 ,105 , 14859 , 19748},
	};

	private const int INSTRUMENT_NOTE_NUMBER_ACCORDION = 5;


	private readonly string[] fileNote_Accordion = {
		"accordion_21",
		"accordion_60",
		"accordion_65",
		"accordion_70",
		"accordion_75",

	};

	private readonly int[,] instrumentNoteSettings_Accordion  = {
		{21, 45 ,59 , 21965 , 83500},
		{60, 60 ,64 , 13215 , 58758},
		{65, 65 ,69 , 17802 , 50502},
		{70, 70 ,74 , 14176 , 55590},
		{75, 75 ,127 , 2747 , 71638},
	};
		


	/*
 * Function:             fa_read_u32
 * Description:           read from the file stream 4 bytes data
 */
	private static long fa_read_u32(Stream mySreamReader)
	{
		long cx;
		byte[] temp = new byte[SIZE_LONG];

		//fread(temp, sizeof(*temp), SIZE_LONG, fp);
		mySreamReader.Read(temp, 0,  sizeof(byte) * SIZE_LONG);
		cx =  (long)temp[0];
		cx |= (long)temp[1] << 8;
		cx |= (long)temp[2] << 16;
		cx |= (long)temp[3] << 24;
		return cx;
	}

	/*
 * Function:             fa_read_u16
 * Description:           read from the file stream 2 bytes data
 */
	private static short fa_read_u16(Stream mySreamReader)
	{
		short cx;
		byte[]  temp = new byte[SIZE_SHORT];

		//fread(temp, sizeof(*temp), SIZE_SHORT, fp);
		mySreamReader.Read(temp, 0,  sizeof(byte) * SIZE_SHORT);
		cx = (short) ( temp[0] | (temp[1] * 256));
		return cx;
	}


	public SoundFont() {
		instrument = -1;
		for (int i = 0; i < noteDataCacheCount; i++) {
			noteDataCache [i] = new NoteDataCache ();
			noteDataCache [i].dataNote = null;
			noteDataCache [i].waveFormat = new WaveFormat ();
		}
	
	}


	 ~SoundFont() {
		for(int i=0; i< noteDataCacheCount; i++) {
			noteDataCache[i].dataNote = null;
		}
	}

	public bool loadSoundFont(int instrumentId, string folder) {

		switch (instrumentId) {
		case MusicInstrument.FLUTE_INSTRUMENT: //flute
			fileNotes = fileNote_Flute;
			instrumentNoteSettings =  instrumentNoteSettings_Flute;
			baseNoteCount = INSTRUMENT_NOTE_NUMBER_FLUTE;
			break;
		case MusicInstrument.PAN_FLUTE_INSTRUMENT:  //pan flute
			fileNotes = fileNote_PanFlute;
			instrumentNoteSettings =  instrumentNoteSettings_PanFlute;
			baseNoteCount = INSTRUMENT_NOTE_NUMBER_PANFLUTE;
			break;
		case MusicInstrument.VIOLIN_INSTRUMENT:
			fileNotes = fileNote_Violin;
			instrumentNoteSettings =  instrumentNoteSettings_Violin;
			baseNoteCount = INSTRUMENT_NOTE_NUMBER_VIOLIN;
			break;
		case MusicInstrument.DIZI_INSTRUMENT:
			fileNotes = fileNote_Dizi;
			instrumentNoteSettings =  instrumentNoteSettings_Dizi;
			baseNoteCount = INSTRUMENT_NOTE_NUMBER_DIZI;
			break;
		case MusicInstrument.XIAO_INSTRUMENT:
			fileNotes = fileNote_Xiao;
			instrumentNoteSettings =  instrumentNoteSettings_Xiao;
			baseNoteCount = INSTRUMENT_NOTE_NUMBER_XIAO;
			break;
		case MusicInstrument.OCARINA_INSTRUMENT:
			fileNotes = fileNote_Ocarina;
			instrumentNoteSettings =  instrumentNoteSettings_Ocarina;
			baseNoteCount = INSTRUMENT_NOTE_NUMBER_OCARINA;
			break;
		case MusicInstrument.SAX_INSTRUMENT:
			fileNotes = fileNote_Sax;
			instrumentNoteSettings =  instrumentNoteSettings_Sax;
			baseNoteCount = INSTRUMENT_NOTE_NUMBER_SAX;
			break;
		case MusicInstrument.TRUMPET_INSTRUMENT:
			fileNotes = fileNote_Trumpet;
			instrumentNoteSettings =  instrumentNoteSettings_Trumpet;
			baseNoteCount = INSTRUMENT_NOTE_NUMBER_TRUMPET;
			break;
		case MusicInstrument.ZITHER_INSTRUMENT:
			/*	fileNotes = fileNote_Piano;
            instrumentNoteSettings =  instrumentNoteSettings_Piano;
            baseNoteCount = INSTRUMENT_NOTE_NUMBER_PIANO;*/
			/* fileNotes = fileNote_Zither;
			instrumentNoteSettings =  instrumentNoteSettings_Zither;
			baseNoteCount = INSTRUMENT_NOTE_NUMBER_ZITHER;*/
			break;
		case MusicInstrument.HARMONICA_INSTRUMENT:
			fileNotes = fileNote_Harmonica;
			instrumentNoteSettings =  instrumentNoteSettings_Harmonica;
			baseNoteCount = INSTRUMENT_NOTE_NUMBER_HARMONICA;
			break;
		case MusicInstrument.ACCORDION_INSTRUMENT:
			fileNotes = fileNote_Accordion;
			instrumentNoteSettings =  instrumentNoteSettings_Accordion;
			baseNoteCount = INSTRUMENT_NOTE_NUMBER_ACCORDION;
			break;
		default:
			//default
			fileNotes = fileNote_Flute;
			instrumentNoteSettings =  instrumentNoteSettings_Flute;
			baseNoteCount = INSTRUMENT_NOTE_NUMBER_FLUTE;
			break;
		}

		instrument = instrumentId;

		rootFolder = folder;
		//save the audio file
		//https://bitbucket.org/Unity-Technologies/assetbundledemo/src/464697bcff9b5422e13950b9a9664cd0b3496d10?at=default

		/*
		 * 
	std::string extension = (fileNotes[0] + strlen(fileNotes[0]) - 3);
		if(extension == "snd" || extension == "wav") {
		for (int i =0; i< baseNoteCount; i++) {
			char waveFilePath[256*2];
			strcpy(waveFilePath, rootFolder);
			strcat(waveFilePath , fileNotes[i]);
			char* fileData = NULL;
			int offset = 16;
			 int size = AppContext::readDataFromAsset(waveFilePath, &fileData);
    
			if(size == 0 || fileData == NULL) {
				return true;
			}
			std::string destPath = CCFileUtils::sharedFileUtils()->getWriteablePath();
			 destPath.append(fileNotes[i]);
			//destPath.append("-1");
			destPath.replace (destPath.size() - 4, 4, "");
			remove(destPath.c_str());
			FILE *fp = fopen(destPath.c_str(),"wb");
			if(fp) {
			   fwrite(fileData + offset,sizeof(unsigned char),size-offset,fp);
			   fclose(fp);
			}
		}
	}
	*/
		return true;
	}



	public SoundNote makeSoundNote(int note) {
		SoundNote soundNote = new SoundNote();
		if(!prepareNoteWithInstrument(soundNote, note)) {
			soundNote = null;
		} 
		return soundNote;
	}

	public bool prepareNoteWithInstrument(SoundNote instrumentNote, int note) {

		int instrIndex = 0;
		bool found = false;
		for(; instrIndex < baseNoteCount; instrIndex++) {
			if( instrumentNoteSettings [instrIndex,1] <= note && instrumentNoteSettings[instrIndex , 2] >= note) {
				found = true;
				break;
			}
		}

		if(!found) {
			return false;
		}

		if(!loadWaveFile(instrIndex)) {
			return false;
		}

		instrumentNote.init(noteDataCache[instrIndex].dataNote, noteDataCache[instrIndex].waveFormat);
		instrumentNote.baseNote = instrumentNoteSettings[instrIndex , 0];
		instrumentNote.startNote = instrumentNoteSettings[instrIndex , 1];
		instrumentNote.endNote = instrumentNoteSettings[instrIndex , 2];
		instrumentNote.loopStart = instrumentNoteSettings[instrIndex , 3] * 4;
		instrumentNote.loopEnd = instrumentNoteSettings[instrIndex , 4] * 4;
		instrumentNote.setNote(note, 0);
		return true;
	}

	private bool Equality(byte[] a1, byte[] b1, int length)
	{
		if(a1 == null || b1 == null)
			return false;
		if(b1.Length < length || a1.Length < length)
			return false;
		while(length >0) {
			length--;
			if(a1[length] != b1[length])
				return false;           
		}
		return true;        
	}

	private bool loadWaveFile(int index) {

		string waveFilePath;
		long            nskip, x_size;
		int  format;
		int  channels, block_align;
		long   samplerate;
		long   bits_per_sample;
		long   bytes_per_sample;
		long   data_size;
		byte[]   temp = new byte[SIZE_ID];

		waveFilePath = rootFolder + fileNotes[index];

		if(noteDataCache[index].dataNote != null) {
			//  return true;;
			//delete noteDataCache[index].dataNote;
			//noteDataCache[index].dataNote = NULL;
			noteDataCache[index].dataNote = null;
		}

		//TextAsset asset = Resources.Load(fileNotes[index]) as TextAsset;
		//byte[] fileData = asset.bytes;
		byte[] fileData = FileReaderUtils.readSoundFont(fileNotes[index]);


		int size = fileData.Length;

		if(size == 0 || fileData == null) {
			return false;
		}

		MemoryStream mySreamReader = new MemoryStream(fileData, 0, size);

		bool isWav = false;

		if(instrument ==  MusicInstrument.ZITHER_INSTRUMENT || instrument == MusicInstrument.VIOLIN_INSTRUMENT) {
			isWav = true;
		}
		byte[] b;
		if(isWav) {
			//wav的文件标识符为RIFF，对标识符进行判断
			mySreamReader.Read(temp, 0, sizeof(byte) * SIZE_ID);
			b = Encoding.ASCII.GetBytes ("RIFF");
			if (!Equality(temp, b, b.Length)) {
				Debug.Log("file is not WAVE format!\n");
				return false;
			}
			mySreamReader.Read(temp, 0, sizeof(byte) * SIZE_LONG);

			//文件的格式类型为WAVE
			mySreamReader.Read(temp, 0, sizeof(byte) * SIZE_ID);
			b = Encoding.ASCII.GetBytes ("WAVE");
			if (!Equality(temp, b, b.Length)) {
				Debug.Log( "file is not WAVE format!\n");
				return false;
			}

			//fmt标志，最后一位为空 /
			mySreamReader.Read(temp, 0, sizeof(char) * SIZE_ID);
			// skip chunks except for "fmt " or "data" 
			b = Encoding.ASCII.GetBytes ("fmt ");
			while (!Equality(temp, b, b.Length)) {
				nskip = fa_read_u32(mySreamReader);
				if (nskip!=0) {
					mySreamReader.Seek(nskip, SeekOrigin.Current);
				}
				mySreamReader.Read(temp, 0, sizeof(char) * SIZE_ID);
			}
		}

		/*sizeof(PCMWAVEFORMAT)*/
		x_size = fa_read_u32(mySreamReader);
		//        printf("LINE[%d]#### %ld\n",__LINE__,x_size);
		/*  1（WAVE_FORMAT_PCM） 格式类别，1表示为PCM形式的声音数据*/
		format = fa_read_u16(mySreamReader);
		x_size -= SIZE_SHORT;
		if (WAVE_FORMAT_PCM != format) {
			Debug.Log( "error! unsupported WAVE file format.\n");
			return false;
		}

		/*声音通道数*/
		channels = 2 * fa_read_u16(mySreamReader);
		x_size -= SIZE_SHORT;
		/*声音采样率*/
		samplerate = fa_read_u32(mySreamReader);
		x_size -= SIZE_LONG;

		fa_read_u32(mySreamReader);                                            /* skip bytes/s          每秒数据量                   */
		block_align     = 2*fa_read_u16(mySreamReader);                          /* skip block align  数据块的调整数                 */
		bits_per_sample = 2*fa_read_u16(mySreamReader);                          /* bits/sample                  每样本的数据位数        */
		bytes_per_sample= (bits_per_sample + BITS_PER_BYTE - 1)/BITS_PER_BYTE;
		block_align     = (int) bytes_per_sample * channels;

		x_size -= SIZE_LONG + SIZE_SHORT + SIZE_SHORT;              /* skip additional part of "fmt " header */
		//        printf("LINE[%d]#### %ld\n",__LINE__,x_size);
		if (x_size!=0) {
			//fseek(fp, x_size, SEEK_CUR);
			mySreamReader.Seek(x_size, SeekOrigin.Current);
		}

		/* skip chunks except for "data" */
		//fread(temp, sizeof(*temp), SIZE_ID, fp);
		mySreamReader.Read(temp, 0, sizeof(byte) * SIZE_ID);

		b = Encoding.ASCII.GetBytes ("data");
		while (!Equality(temp, b, b.Length)) {
			nskip = fa_read_u32(mySreamReader);                                /* read chunk size */
			//fseek(fp, nskip, SEEK_CUR);
			mySreamReader.Seek(nskip, SeekOrigin.Current);
			//fread(temp, sizeof(*temp), SIZE_ID, fp);
			mySreamReader.Read(temp, 0, sizeof(byte) * SIZE_ID);
		}
		data_size = 2* fa_read_u32(mySreamReader); /*语音数据大 小*/
		long dataOffSet = mySreamReader.Position;// ->TellPosition();//ftell(fp);
		noteDataCache[index].waveFormat.format           = (ushort) format;
		noteDataCache[index].waveFormat.channels         = (ushort) channels;
		noteDataCache[index].waveFormat.samplerate       = (ulong) samplerate;
		noteDataCache[index].waveFormat.bytes_per_sample = (ushort) bytes_per_sample;
		noteDataCache[index].waveFormat.block_align      = (ushort) block_align;
		noteDataCache[index].waveFormat.data_size        = (ulong) (data_size / block_align);             /* byte to short */

		int readed = 0;
		if(data_size>0) {
			noteDataCache[index].dataNote = new byte[data_size];
			//fseek(fp, dataOffSet , SEEK_SET);
			mySreamReader.Seek(dataOffSet, SeekOrigin.Begin);
			//readed = fread(noteDataCache[index].dataNote, sizeof(char) , data_size, fp);
			byte[] data = new byte[data_size/2];
			readed = mySreamReader.Read(data, 0, (int) (sizeof(byte) * data_size/2));
			for(int i=0; i< readed/2; i++) {
				noteDataCache[index].dataNote[4*i] = data[2*i];
				noteDataCache[index].dataNote[4*i+1] = data[2*i+1];
				noteDataCache[index].dataNote[4*i+2] = data[2*i];
				noteDataCache[index].dataNote[4*i+3] = data[2*i+1];
			}

			//  readed = mySreamReader->Read(noteDataCache[index].dataNote, sizeof(char) * data_size);
			// fclose(fp);
		}

		mySreamReader = null;
		return true;

	}




}

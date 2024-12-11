using NAudio.Wave;
using System.IO;

namespace Qboard.Systems;

public class SoundManager
{
    public const string soundPath = "sounds/";
    int currentPage = 0;
    List<Page> pages = new List<Page>();
    IWavePlayer[] _wavePlayers;
    AudioFileReader[] _audioFileReaders;
    public SoundManager()
    {
        if (!Directory.Exists(soundPath) || !File.Exists(soundPath + "config.cfg"))
            CreateConfig();
        else
            LoadConfig();
    }

    public void PlaySound(int index)
    {
        if (pages[currentPage].sounds[index] == "") return;
        string soundPath = SoundManager.soundPath + pages[currentPage].sounds[index];
        IWavePlayer waveOutDevice = new WaveOut();
        AudioFileReader audioFileReader = new AudioFileReader(soundPath);

        waveOutDevice.Init(audioFileReader);
        waveOutDevice.Play();
        waveOutDevice.PlaybackStopped += (sender, args) => {
            waveOutDevice.Dispose();
            audioFileReader.Dispose();
            Console.WriteLine("Sound disposed!");
        };
    }

    public void SetSound(int index, string sound)
    {
        string fileName = Path.GetFileName(sound);
        if(!File.Exists(soundPath + fileName))
            File.Copy(sound, soundPath + fileName);
        //Handle later when a sound is present to either copy or set to the sound
        pages[currentPage].SetSound(index, fileName);
    }

    private void CreateConfig()
    {
        Console.WriteLine("Directory doesn't exist, creating...");
        if(!Directory.Exists(soundPath))
            Directory.CreateDirectory(soundPath);
        var stream = File.CreateText(soundPath + "config.cfg");
        stream.Close();
    }

    private void LoadConfig()
    {
        string data = File.ReadAllText(soundPath + "config.cfg");
        Console.WriteLine(data);
        data = data.Replace("\r", "");
        
        //Read header
        string[] pageNames;
        (Block, string) header = ConfigSerializer.ReadBlock(data);
        data = header.Item2;
        if(header.Item1.header != "pages")
        {
            throw new Exception("Invalid config file.");
        }
        pageNames = header.Item1.data;

        //Read pages
        while (data != "")
        {
            (Block, string) tBlock = ConfigSerializer.ReadBlock(data);
            data = tBlock.Item2;
            Block block = tBlock.Item1;
            pages.Add(new Page(block.header, block.data));
        }
    }
}
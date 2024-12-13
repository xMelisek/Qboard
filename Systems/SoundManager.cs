using NAudio.Wave;
using System.IO;
using System.Windows.Controls;

namespace Qboard.Systems;

public class SoundManager
{
    MainWindow _window;
    public const string soundPath = "sounds/";
    int currentPage = 0;
    List<Page> pages = new List<Page>();
    public string PageName { get => pages[currentPage].name; set => pages[currentPage].name = value; }
    IWavePlayer[] _wavePlayers = {null, null, null, null, null, null};
    AudioFileReader[] _audioFileReaders = {null, null, null, null, null, null};
    public SoundManager(MainWindow mainWindow)
    {
        _window = mainWindow;
        if (!Directory.Exists(soundPath) || !File.Exists(soundPath + "config.cfg"))
            CreateConfig();
        else
            LoadConfig();
    }

    public void InitializeContent()
    {
        _window.TabName.Text = PageName;
        foreach (var page in pages)
        {
            var button = new Button();
            button.Content = page.name;
            button.Click += (s, e) =>
            {
                ChangePage(pages.IndexOf(page));
            };
            _window.Pages.Children.Add(button);
        }
    }

    public void PlaySound(int index)
    {
        if (pages[currentPage].sounds[index] == "") return;
        string soundPath = SoundManager.soundPath + pages[currentPage].sounds[index];
        if (_wavePlayers[index] == null)
        {
            //No wave player associated
            IWavePlayer waveOutDevice = new WaveOut();
            AudioFileReader audioFileReader = new AudioFileReader(soundPath);
            waveOutDevice.Init(audioFileReader);
            waveOutDevice.Play();
            _wavePlayers[index] = waveOutDevice;
            _audioFileReaders[index] = audioFileReader;
        }
        else
        {
            if (_audioFileReaders[index].FileName != soundPath)
            {
                _wavePlayers[index].Dispose();
                _audioFileReaders[index].Dispose();
                IWavePlayer waveOutDevice = new WaveOut();
                AudioFileReader audioFileReader = new AudioFileReader(soundPath);
                waveOutDevice.Init(audioFileReader);
                waveOutDevice.Play();
                _wavePlayers[index] = waveOutDevice;
                _audioFileReaders[index] = audioFileReader;
            }
            else
            {
                _wavePlayers[index].Play();
            }
        }
    }

    public void ChangePage(int page)
    {
        if (page == currentPage) return;
        _window.TabName.Text = pages[currentPage].name;
        for (int i = 0; i < _wavePlayers.Length; i++)
        {
            if (_wavePlayers[i] == null)
                continue;
            _wavePlayers[i].Dispose();
            _audioFileReaders[i].Dispose();
        }
        currentPage = page;
        _window.TabName.Text = PageName;
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
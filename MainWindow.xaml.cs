using System.IO;
using System.Windows;
using System.Windows.Input;
using Qboard.Systems;
using Button = System.Windows.Controls.Button;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace Qboard;

public partial class MainWindow : Window
{
    SerialThread serialT = new SerialThread();
    SoundManager soundManager;

    public MainWindow()
    { 
        soundManager = new SoundManager(this);
        InitializeComponent();
        soundManager.InitializeContent();
        serialT.soundManager = soundManager;
        serialT.playSound += (int index) =>
        {
            soundManager.PlaySound(index);
        };
        // serialT.updateDebug += (string data) =>
        // {
        //     bool acc = DebugValue.Dispatcher.CheckAccess();
        //     if (acc)
        //         DebugValue.Content = data;
        //     else
        //         DebugValue.Dispatcher.Invoke(() => {DebugValue.Content = data;});
        // };
        serialT.Start();
    }

    public void OnClick(object sender, RoutedEventArgs e)
    {
        Button? b = sender as Button;
        int but = Convert.ToInt32(b.Tag);
        soundManager.PlaySound(but);
    }

    protected override void OnDrop(DragEventArgs e)
    {
        Button? b = e.Source as Button;
        int but = Convert.ToInt32(b.Tag);
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            //Get dropped files paths
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            //Throw an error message when there are more than 1 sound sent because multiple sounds on a bind are not supported
            if (files.Length > 1)
            {
                MessageBoxResult result = MessageBox.Show("You can't upload many sounds into a single bind", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                    return;
            }
            string[] soundExts = {".wav", ".mp3", ".mp4"};
            if(!soundExts.Contains(Path.GetExtension(files[0])))
            {
                MessageBoxResult result = MessageBox.Show($"The file extension isn't supported.\nSupported extensions are: {string.Join(", ", soundExts)}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                    return;
            }
            soundManager.SetSound(but, files[0]);
        }
    }

    private void OnQuit(object sender, EventArgs e)
    {
        //TODO:Save preset
    }

    private void OnBoxKeyPressed(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        var box = sender as TextBox;
        soundManager.PageName = box.Text;
    }
}
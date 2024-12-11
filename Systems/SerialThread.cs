using System.IO.Ports;
using Qboard.Systems;

namespace Qboard;

class SerialThread
{
    public SoundManager soundManager;
    public delegate void UpdateDebug(string data);
    public delegate void PlaySound(int index);
    public UpdateDebug updateDebug;
    public PlaySound playSound;
    private string previousData;

    private SerialPort port = new SerialPort("COM6", 9600, Parity.None, 8, StopBits.One);

    public void Start()
    {
        try {
            port.Open();
        }
        catch(System.IO.FileNotFoundException) {
            Console.WriteLine("Port not open, running only the interface without any hardware");
        }
        Thread thread = new Thread(new ThreadStart(SerialLoop));
        thread.Start();
    }

    private void SerialLoop()
    {
        while(port.IsOpen)
        {
            string data = port.ReadLine();
            if(data == previousData)
                continue;
            if (data == null)
                continue;
            try
            {
                if(Convert.ToInt32(data) > 0 && Convert.ToInt32(data) < 6)
                    playSound(Convert.ToInt32(data));
                    // soundManager.PlaySound(Convert.ToInt32(data));
            }
            catch(FormatException)
            {

            }
            
            // updateDebug(data);
            previousData = data;
        }
    }
}
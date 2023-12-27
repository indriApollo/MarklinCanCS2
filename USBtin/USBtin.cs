using System.IO.Ports;

namespace USBtin;

public class USBtin
{
    private readonly SerialPort _port;
    private readonly bool _loggingEnabled;
    
    public USBtin(string portName, bool enableLogging = false)
    {
        _port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
        
        _port.NewLine = "\r";

        _port.WriteTimeout = 1000;
        _port.ReadTimeout = 1000;

        _loggingEnabled = enableLogging;
    }

    public void Open()
    {
        _port.Open();
        WaitTillReady();
    }

    public string GetHardwareVersion()
    {
        WriteLine("V");
        return ReadLine();
    }
    
    public string GetSerialNumber()
    {
        WriteLine("N");
        return ReadLine();
    }
    
    public string GetFirmwareVersion()
    {
        WriteLine("v");
        return ReadLine();
    }

    public void SetCanBaudRate(CanBaudRate baudRate)
    {
        WriteLine($"S{(int)baudRate}");
    }

    public void OpenCanChannel()
    {
        WriteLine("O");
    }
    
    public void OpenLoopBackMode()
    {
        WriteLine("l");
    }
    
    public void OpenCanChannelListenOnly()
    {
        WriteLine("L");
    }

    public void CloseCanChannel()
    {
        WriteLine("C");
    }

    private void WaitTillReady()
    {
        // Close the CAN Channel,
        // then query the firmware version
        // and wait for the response to confirm device is ready

        var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
        while (!cancellationToken.IsCancellationRequested)
        {
            CloseCanChannel();
            WriteLine("v");
            
            try
            {
                ReadLine();
                _port.DiscardInBuffer();
                return;
            }
            catch (TimeoutException)
            {
                Log("Not yet ready ...");
            }
        }

        throw new TimeoutException("Device took too long to get ready");
    }

    private void WriteLine(string text)
    {
        if (_loggingEnabled)
            Log($"out : {text}");
        _port.WriteLine(text);
    }
    
    private string ReadLine()
    {
        var text = _port.ReadLine();
        if (_loggingEnabled)
            Log($"in  : {text}");
        return text;
    }

    private void Log(string msg)
    {
        if (_loggingEnabled)
            Console.WriteLine(msg);
    }
}

public enum CanBaudRate
{
    Br10K = 0,
    Br20K = 1,
    Br50K = 2,
    Br100K = 3,
    Br125K = 4,
    Br250K = 5,
    Br500K = 6,
    Br800K = 7,
    Br1M = 8
}
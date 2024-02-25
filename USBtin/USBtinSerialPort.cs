using System.IO.Ports;

namespace USBtin;

public class USBtinSerialPort : IUSBtinSerialPort
{
    private readonly SerialPort _port;

    public USBtinSerialPort(string portName)
    {
        _port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);

        _port.NewLine = "\r";

        _port.WriteTimeout = 1000;
        _port.ReadTimeout = 1000;
    }

    public void Open() => _port.Open();

    public void WriteLine(string text) => _port.WriteLine(text);

    public string ReadLine() => _port.ReadLine();

    public int BytesToRead => _port.BytesToRead;

    public string ReadExisting() => _port.ReadExisting();
}

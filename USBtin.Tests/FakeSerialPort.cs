using System.IO;

namespace USBtin.Tests;

public class FakeSerialPort : IUSBtinSerialPort
{
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;

    private long _writePosition;
    private long _readPosition;

    public FakeSerialPort()
    {
        var ms = new MemoryStream();
        _reader = new StreamReader(ms);
        _writer = new StreamWriter(ms);
        _writer.AutoFlush = true;
    }

    public void Open()
    {
    }

    public void WriteLine(string text)
    {
        _writer.BaseStream.Position = _writePosition;
        _writer.WriteLine(text);
        _writePosition = _writer.BaseStream.Position;
    }
    
    public string ReadLine()
    {
        _reader.BaseStream.Position = _readPosition;
        var line = _reader.ReadLine() ?? "";
        _readPosition = _reader.BaseStream.Position;
        return line;
    }

    public int BytesToRead => (int)(_reader.BaseStream.Length - _reader.BaseStream.Position - 1);
    
    public string ReadExisting()
    {
        return _reader.ReadToEnd();
    }
}
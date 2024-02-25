using System.Collections.Generic;

namespace USBtin.Tests;

public class FakeSerialPort : IUSBtinSerialPort
{
    private readonly Queue<string> _queue = new();

    public void Open()
    {
    }

    public void WriteLine(string text)
    {
        _queue.Enqueue(text);
    }
    
    public string ReadLine()
    {
        return _queue.Dequeue();
    }

    public int BytesToRead => _queue.Count;
    
    public string ReadExisting()
    {
        _queue.Clear();
        return "";
    }
}

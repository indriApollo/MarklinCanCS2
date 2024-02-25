namespace USBtin;

public interface IUSBtinSerialPort
{
    void Open();
    void WriteLine(string text);
    string ReadLine();
    int BytesToRead { get; }
    string ReadExisting();
}

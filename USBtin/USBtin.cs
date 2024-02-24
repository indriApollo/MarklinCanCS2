using System.Globalization;
using System.IO.Ports;

namespace USBtin;

public class USBtin
{
    /* ASCII commands https://www.fischl.de/usbtin/
     Sx[CR] | Set baudrate
            | x: Bitrate id (0-8)
            | S0 = 10 kBaud
            | S1 = 20 kBaud
            | S2 = 50 kBaud
            | S3 = 100 kBaud
            | S4 = 125 kBaud
            | S5 = 250 kBaud
            | S6 = 500 kBaud
            | S7 = 800 kBaud
            | S8 = 1 MBaud
            
     sxxyyzz[CR] | Set can bitrate registers of MCP2515. You can set non-standard baudrates which are not supported by the "Sx" command.
                 | xx: CNF1 as hexadecimal value (00-FF)
                 | yy: CNF2 as hexadecimal value (00-FF)
                 | zz: CNF3 as hexadecimal value
                 
     Gxx[CR] | Read MCP2515 register. xx: Address of MCP2515 register to read as hexadecimal value (00-FF).
     
     Wxxyy[CR] | Write MCP2515 register. xx: Address of MCP2515 register to write. Hexadecimal value (00-FF).
               | yy: Data to write to the register. Hexadecimal value (00-FF).
               
     V[CR] | Get hardware version.
     
     v[CR] | Get firmware version.
     
     N[CR] | Get serial number. Returns always 0xffff.
     
     O[CR] | Open CAN channel.
     
     l[CR] | Open device in loop back mode.
     
     L[CR] | Open CAN channel in listen-only mode.
     
     C[CR] | Close CAN channel
     
     tiiildd..[CR] | Transmit standard (11 bit) frame.
                   | iii: Identifier in hexadecimal format (000-7FF)
                   | l: Data length (0-8)
                   | dd: Data byte value in hexadecimal format (00-FF)
                   
     Tiiiiiiiildd..[CR] | Transmit extended (29 bit) frame.
                        | iiiiiiii: Identifier in hexadecimal format (0000000-1FFFFFFF)
                        | l: Data length (0-8)
                        | dd: Data byte value in hexadecimal format (00-FF)
                        
     riiil[CR] | Transmit standard RTR (11 bit) frame.
               | iii: Identifier in hexadecimal format (000-7FF)
               | l: Data length (0-8)
               
     Riiiiiiiil[CR] | Transmit extended RTR (29 bit) frame.
                    | iiiiiiii: Identifier in hexadecimal format (0000000-1FFFFFFF)
                    | l: Data length (0-8)
                    
     F[CR] | Read status/error flag of can controller
           | Return: Fxx[CR] with xx as hexadecimal byte with following error flags:
           | Bit 0 - not used
           | Bit 1 - not used
           | Bit 2 - Error warning (Bit EWARN of MCP2515)
           | Bit 3 - Data overrun (Bit RX1OVR or RX0OVR of MCP2515)
           | Bit 4 - not used
           | Bit 5 - Error-Passive (Bit TXEP or RXEP of MCP2515)
           | Bit 6 - not used
           | Bit 7 - Bus error (Bit TXBO of MCP2515)
           
      Zx[CR] | Set timestamping on/off.
             | x: 0=off, 1=on
             
       mxxxxxxxx[CR] | Set acceptance filter mask. SJA1000 format (AM0..AM3). Only first 11bit are relevant.
                     | xxxxxxxx: Acceptance filter mask
                     
       Mxxxxxxxx[CR] | Set acceptance filter code. SJA1000 format (AC0..AC3). Only first 11bit are relevant.
                     | xxxxxxxx: Acceptance filter code
     */
    
    private readonly IUSBtinSerialPort _port;
    private readonly bool _loggingEnabled;
    
    public USBtin(string portName, bool enableLogging = false)
    {
        _port = new USBtinSerialPort(portName);
        _loggingEnabled = enableLogging;
    }

    public USBtin(IUSBtinSerialPort port, bool enableLogging = false)
    {
        _port = port;
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
        ReadLine();
    }

    public void OpenCanChannel()
    {
        WriteLine("O");
        ReadLine();
    }
    
    public void OpenLoopBackMode()
    {
        WriteLine("l");
        ReadLine();
    }
    
    public void OpenCanChannelListenOnly()
    {
        WriteLine("L");
        ReadLine();
    }

    public void CloseCanChannel()
    {
        WriteLine("C");
    }

    public void TransmitStandard(ushort identifier, byte[] data)
    {
        MustBe11BitsIdentifier(identifier);
        Transmit("t", identifier, data);
    }

    public void TransmitExtended(uint identifier, byte[] data)
    {
        MustBe29BitsIdentifier(identifier);
        Transmit("T", identifier, data);
    }
    
    public void TransmitStandardRtr(ushort identifier, byte dataLength)
    {
        MustBe11BitsIdentifier(identifier);
        TransmitRtr("r", identifier, dataLength);
    }

    public void TransmitExtendedRtr(uint identifier, byte dataLength)
    {
        MustBe29BitsIdentifier(identifier);
        TransmitRtr("R", identifier, dataLength);
    }

    public CanFrame? ReadCanFrame()
    {
        var frame = ReadLine();
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // smallest valid frame is riiil
        if (frame.Length < 5)
            return null;

        var cmd = frame[0];

        if (cmd == 't')
        {
            // tiiildd..
            return new CanFrame(uint.Parse(frame[1..4], NumberStyles.HexNumber),
                ParseCharNum(frame[4]),
                Convert.FromHexString(frame[5..]),
                timestamp);
        }
        
        if (cmd == 'T')
        {
            // Tiiiiiiiildd..
            return new CanFrame(uint.Parse(frame[1..9], NumberStyles.HexNumber),
                ParseCharNum(frame[9]),
                Convert.FromHexString(frame[10..]),
                timestamp);
        }
        
        if (cmd == 'r')
        {
            // riiil
            return new CanFrame(uint.Parse(frame[1..4], NumberStyles.HexNumber),
                ParseCharNum(frame[4]),
                null,
                timestamp);
        }
        
        if (cmd == 'R')
        {
            // Riiiiiiiil
            return new CanFrame(uint.Parse(frame[1..9], NumberStyles.HexNumber),
                ParseCharNum(frame[9]),
                null,
                timestamp);
        }

        return null;
    }

    public bool HasDataToRead()
    {
        return _port.BytesToRead > 1;
    }

    public async Task ListenAndLogFrames(Stream logStream, CancellationToken cancellationToken)
    {
        await using var log = new BinaryWriter(logStream);
        while (!cancellationToken.IsCancellationRequested)
        {
            var frame = ReadCanFrame();
            if (frame is null)
                continue;
            
            log.WriteFrame(frame);
        }
    }

    public static IEnumerable<CanFrame> ReadFramesFromLog(Stream logStream)
    {
        using var log = new BinaryReader(logStream);
        while (true)
        {
            var frame = log.ReadFrame();

            if (frame is null)
                yield break;
            yield return frame;
        }
    }

    private static byte ParseCharNum(char c) => (byte)(c - 48);

    private void Transmit(string cmd, uint identifier, byte[] data)
    {
        if (data.Length > 8)
            throw new ArgumentException("max 8 data bytes", nameof(data));
        
        WriteLine($"{cmd}{ToIdentifierString(identifier)}{data.Length}{Convert.ToHexString(data)}");
    }
    
    private void TransmitRtr(string cmd, uint identifier, byte dataLength)
    {
        if (dataLength > 8)
            throw new ArgumentException("max 8 data bytes", nameof(dataLength));
        
        WriteLine($"{cmd}{ToIdentifierString(identifier)}{dataLength}");
    }
    
    private string ToIdentifierString(uint identifier) => identifier > 0XFFF ? $"{identifier:X8}" : $"{identifier:X3}";

    private static void MustBe11BitsIdentifier(ushort identifier)
    {
        if (identifier > 0x7FF)
            throw new ArgumentException("must be 11 bits", nameof(identifier));
    }

    private static void MustBe29BitsIdentifier(uint identifier)
    {
        if (identifier > 0x1FFFFFFF)
            throw new ArgumentException("must be 29 bits", nameof(identifier));
    }

    private void WaitTillReady()
    {
        // close the CAN Channel to start fresh,
        // then query the firmware version
        // and wait for the response to confirm device is ready

        var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
        while (!cancellationToken.IsCancellationRequested)
        {
            CloseCanChannel();
            WriteLine("v");
            
            try
            {
                var text = ReadLine();
                // check that we didn't read any random garbage
                if (text.StartsWith("v") && text.Length > 1)
                {
                    Thread.Sleep(1000);
                    _port.ReadExisting();
                    return;
                }
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

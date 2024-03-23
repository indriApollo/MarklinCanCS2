using Protocol.Commands;
using Protocol.Enums;

// linux: /dev/ttyACM0
// macos: /dev/tty.usbmodemA02173041

using var f = File.OpenRead("marklin.canlog");
var readFramesFromLog = USBtin.USBtin.ReadFramesFromLog(f);

foreach (var frame in readFramesFromLog)
{
    var message = Protocol.Decoder.Decode(frame.Identifier, frame.Data ?? []);
    
    if (message.Command != CommandType.LocomotiveFunction)
        continue;

    var msg = message.Command switch
    {
        CommandType.LocomotiveFunction => new LocomotiveFunctionCommand(message).ToString(),
        CommandType.LocomotiveDirection => new LocomotiveDirectionCommand(message).ToString(),
        CommandType.LocomotiveSpeed => new LocomotiveSpeedCommand(message, 14).ToString(),
        CommandType.SystemCommand => new SystemCommand(message).ToString(),
        _ => new Command(message).ToString()
    };

    Console.WriteLine($"{frame.Identifier:X11} {frame.DataLength} {Convert.ToHexString(frame.Data ?? [])}");
    Console.WriteLine(msg);
    Console.WriteLine();
}
using Protocol.Commands;
using Protocol.Enums;
using Protocol.Extensions;
using USBtin;

// linux: /dev/ttyACM0
// macos: /dev/tty.usbmodemA02173041

using var f = File.OpenRead("marklin.canlog");
var readFramesFromLog = USBtin.USBtin.ReadFramesFromLog(f);

foreach (var frame in readFramesFromLog)
{
    var message = Protocol.Decoder.Decode(frame.Identifier, frame.Data ?? []);
    
    if (message.Uid != 24)
        continue;
    
    if (message.Command != CommandType.LocomotiveSpeed)
        continue;
    
    Console.WriteLine($"{frame.Identifier:X11} {frame.DataLength} {Convert.ToHexString(frame.Data ?? [])}");
    Console.WriteLine($"cmd:{message.Command} addr:{message.Uid} sub:{Convert.ToHexString(message.CommandData)} {new LocomotiveSpeedCommand(message).Speed.ToSpeedLevel(14)}");
    Console.WriteLine();
}
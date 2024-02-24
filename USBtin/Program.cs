using USBtin;

// linux: /dev/ttyACM0
// macos: /dev/tty.usbmodemA02173041

var usbTin = new USBtin.USBtin("/dev/ttyACM0", true);

usbTin.Open();

usbTin.SetCanBaudRate(CanBaudRate.Br250K);

usbTin.OpenLoopBackMode();

usbTin.TransmitStandard(1, [1,2,3]);

while (!usbTin.HasDataToRead())
{
    Console.Write('.');
    await Task.Delay(100);
}
Console.WriteLine();

while (true)
{
    var frame = usbTin.ReadCanFrame();
    if (frame is not null)
    {
        Console.WriteLine($"{frame.Identifier} {frame.DataLength} {Convert.ToHexString(frame.Data!)}");
        break;
    }
}

usbTin.TransmitStandard(2, [4,5,6]);

while (true)
{
    var frame = usbTin.ReadCanFrame();
    if (frame is not null)
    {
        Console.WriteLine($"{frame.Identifier} {frame.DataLength} {Convert.ToHexString(frame.Data!)}");
        break;
    }
}

using USBtin;

// linux: /dev/ttyACM0
// macos: /dev/tty.usbmodemA02173041

var usbTin = new USBtin.USBtin("/dev/ttyACM0", enableLogging: true);

usbTin.Open();

usbTin.SetCanBaudRate(CanBaudRate.Br250K);

usbTin.OpenLoopBackMode();

for (byte i = 0; i < 100; i++)
    usbTin.TransmitStandard(1, [i]);

var frames = new List<CanFrame>();
await foreach (var frame in usbTin.ListenAndReadFrames(new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token))
{
    frames.Add(frame);
}

Console.WriteLine(frames.Count);

var usbTin = new USBtin.USBtin("/dev/tty.usbmodemA02173041", true);

usbTin.Open();

var v = usbTin.GetFirmwareVersion();
Console.WriteLine(v);
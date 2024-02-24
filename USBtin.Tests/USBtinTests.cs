using System;
using Xunit;

namespace USBtin.Tests;

public class USBtinTests
{
    [Fact]
    public void GivenACanFrame_WhenTransmitStandard_ThenSerialCommandIsCorrect()
    {
        var port = new FakeSerialPort();

        var usbTin = new USBtin(port);
        
        usbTin.TransmitStandard(123, [1]);

        var line = port.ReadLine();

        Assert.Equal("t07B101", line);
    }
    
    [Fact]
    public void GivenACanFrame_WhenReadCanFrame_ThenCorrectlyReadFrame()
    {
        var port = new FakeSerialPort();

        var usbTin = new USBtin(port);
        
        usbTin.TransmitStandard(123, [1]);

        var frame = usbTin.ReadCanFrame();

        Assert.NotNull(frame);
        Assert.Equal((uint)123, frame.Identifier);
        Assert.Equal(1, frame.DataLength);
        Assert.Equal([1], frame.Data);
        Assert.Equal(DateTimeOffset.UtcNow, DateTimeOffset.FromUnixTimeMilliseconds(frame.Timestamp),
            TimeSpan.FromMinutes(1));
    }
}
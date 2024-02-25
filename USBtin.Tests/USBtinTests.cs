using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace USBtin.Tests;

public class USBtinTests(ITestOutputHelper output)
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
    }

    [Fact]
    public async void GivenCanFrames_WhenLoggingFrames_ThenLogAndReadFromLogSuccessfully()
    {
        var port = new FakeSerialPort();

        var usbTin = new USBtin(port, enableLogging: true, logger: output.WriteLine);
        
        usbTin.TransmitStandard(123, [1]);
        usbTin.TransmitExtended(456789, [1,2,3]);
        usbTin.TransmitStandardRtr(321, 0);

        const string logFileName = "log-test.can";

        await using var fw = File.OpenWrite(logFileName);
        await usbTin.ListenAndLogFrames(fw, new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token);

        Assert.True(File.Exists(logFileName));
        await using var fr = File.OpenRead(logFileName);

        var frames = USBtin.ReadFramesFromLog(fr).ToArray();

        Assert.Equal(3, frames.Length);

        var frame1 = frames[0];
        Assert.Equal((uint)123, frame1.Identifier);
        Assert.Equal(1, frame1.DataLength);
        Assert.Equal([1], frame1.Data);
        
        var frame2 = frames[1];
        Assert.Equal((uint)456789, frame2.Identifier);
        Assert.Equal(3, frame2.DataLength);
        Assert.Equal([1,2,3], frame2.Data);
        
        var frame3 = frames[2];
        Assert.Equal((uint)321, frame3.Identifier);
        Assert.Equal(0, frame3.DataLength);
        Assert.Null(frame3.Data);
        
        File.Delete(logFileName);
    }

    [Fact]
    public async void GivenCanFrames_WhenListening_ThenReadFramesSuccessfully()
    {
        var port = new FakeSerialPort();

        var usbTin = new USBtin(port);

        usbTin.TransmitStandard(123, [1]);
        usbTin.TransmitExtended(456789, [1,2,3]);
        usbTin.TransmitStandardRtr(321, 0);

        var frames = new List<CanFrame>();
        await foreach (var frame in usbTin.ListenAndReadFrames(new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token))
        {
            frames.Add(frame);
        }

        Assert.Equal(3, frames.Count);

        var frame1 = frames[0];
        Assert.Equal((uint)123, frame1.Identifier);
        Assert.Equal(1, frame1.DataLength);
        Assert.Equal([1], frame1.Data);
        
        var frame2 = frames[1];
        Assert.Equal((uint)456789, frame2.Identifier);
        Assert.Equal(3, frame2.DataLength);
        Assert.Equal([1,2,3], frame2.Data);
        var frame3 = frames[2];
        Assert.Equal((uint)321, frame3.Identifier);
        Assert.Equal(0, frame3.DataLength);
        Assert.Null(frame3.Data);
    }
}

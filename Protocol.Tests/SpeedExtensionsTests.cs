using Protocol.Extensions;
using Xunit;

namespace Protocol.Tests;

public class SpeedExtensionsTests
{
    [Theory]
    [InlineData(0, 14, 0)]
    [InlineData(771, 14, 11)]
    [InlineData(381, 27, 11)]
    [InlineData(371, 28, 11)]
    [InlineData(331, 31, 11)]
    [InlineData(81, 126, 11)]
    public void GivenSystemSpeed_WhenToSpeedLevel_ThenCorrectLevel(ushort systemSpeed, byte speedLevels, byte expectedSpeedLevel)
    {
        var actual = systemSpeed.ToSpeedLevel(speedLevels);
        Assert.Equal(expectedSpeedLevel, actual);
    }
    
    [Theory]
    [InlineData(0, 14, 0)]
    [InlineData(11, 14, 771)]
    [InlineData(11, 27, 381)]
    [InlineData(11, 28, 371)]
    [InlineData(11, 31, 331)]
    [InlineData(11, 126, 81)]
    public void GivenSpeedLevel_WhenToSpeedLevel_ThenCorrectLevel(byte speedLevel, byte speedLevels, ushort expectedSystemSpeed)
    {
        var actual = speedLevel.ToSystemSpeed(speedLevels);
        Assert.Equal(expectedSystemSpeed, actual);
    }
}
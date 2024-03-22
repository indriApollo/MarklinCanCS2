namespace Protocol.Extensions;

public static class SpeedExtensions
{
    public static byte ToSpeedLevel(this ushort systemSpeed, byte speedLevels)
    {
        if (systemSpeed == 0) return 0;
        return (byte)((systemSpeed - 1) / GetSpeedStepSize(speedLevels) + 1);
    }

    public static ushort ToSystemSpeed(this byte speedStep, byte speedLevels)
    {
        if (speedStep == 0) return 0;
        return (ushort)(1 + (speedStep - 1) * GetSpeedStepSize(speedLevels));
    }

    private static byte GetSpeedStepSize(byte speedLevels) =>
        speedLevels switch
        {
            14 => 77,
            27 => 38,
            28 => 37,
            31 => 33,
            126 => 8,
            _ => throw new ArgumentException($"Invalid speed levels {speedLevels}", nameof(speedLevels))
        };
}
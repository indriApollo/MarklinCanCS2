using System.Buffers.Binary;
using Protocol.Enums;
using Protocol.Extensions;

namespace Protocol.Commands;

public class LocomotiveSpeedCommand(Message message, byte speedLevels) : Command(message, CommandType.LocomotiveSpeed)
{
    public ushort? Speed => Message.CommandData.Length == 1
        ? BinaryPrimitives.ReadUInt16BigEndian(Message.CommandData)
        : null;

    public override string ToString() =>
        base.ToString() + $" speed:{Speed?.ToSpeedLevel(speedLevels)}";
}
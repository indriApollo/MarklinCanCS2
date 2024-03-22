using System.Buffers.Binary;
using Protocol.Enums;

namespace Protocol.Commands;

public class LocomotiveSpeedCommand(Message message) : Command(message, CommandType.LocomotiveSpeed)
{
    public ushort Speed => BinaryPrimitives.ReadUInt16BigEndian(Message.CommandData);
}
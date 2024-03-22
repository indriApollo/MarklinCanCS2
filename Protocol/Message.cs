using Protocol.Enums;

namespace Protocol;

public record Message
{
    public IdentifierPriorityType Priority { get; set; }
    public CommandType Command { get; set; }
    public byte Response { get; set; }
    public ushort Hash { get; set; }
    public uint Uid { get; set; }
    public byte[] CommandData { get; set; } = []; // Big-endian
}
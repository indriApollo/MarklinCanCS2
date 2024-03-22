using System.Buffers.Binary;
using Protocol.Enums;

namespace Protocol;

public static class Decoder
{
    public static Message Decode(uint identifier, byte[] data)
    {
        // Extended CAN frame 29 bits identifier
        // Priority | Command | Response |    Hash |    DLC | Data
        //   4 bits |  8 bits |    1 bit | 16 bits | 4 bits | 0 to 8 bytes
        
        var message = new Message();
        DecodeIdentifier(identifier, ref message);
        DecodeUid(data, ref message);
        DecodeCommandData(data, ref message);
        
        return message;
    }
    
    private static void DecodeIdentifier(uint identifier, ref Message message)
    {
        message.Priority = (IdentifierPriorityType)((identifier & 0x1E000000) >> 25);
        message.Command = (CommandType)((identifier & 0x1FE0000) >> 17);
        message.Response = (byte)((identifier & 0x10000) >> 16);
        message.Hash = (ushort)(identifier & 0xFFFF);
    }

    private static void DecodeUid(ReadOnlySpan<byte> data, ref Message message)
    {
        if (data.Length < 4)
            return;

        message.Uid = BinaryPrimitives.ReadUInt32BigEndian(data[..4]);
    }

    private static void DecodeCommandData(ReadOnlySpan<byte> data, ref Message message)
    {
        if (data.Length < 5)
            return;

        message.CommandData = data[4..].ToArray();
    }
}
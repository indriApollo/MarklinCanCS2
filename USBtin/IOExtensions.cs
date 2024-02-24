namespace USBtin;

public static class IOExtensions
{
    public static void WriteFrame(this BinaryWriter writer, CanFrame frame)
    {
        if (frame is { DataLength: > 0, Data: null })
            throw new ArgumentException("frame has data but length is reported as 0", nameof(frame));
        
        writer.Write(frame.Identifier);
        writer.Write(frame.DataLength);
        if (frame.Data is not null)
            writer.Write(frame.Data);
        writer.Write(frame.Timestamp);
    }

    public static CanFrame ReadFrame(this BinaryReader reader)
    {
        var identifier = reader.ReadUInt32();
        var dataLength = reader.ReadByte();
        byte[]? data = null;
        if (dataLength > 0)
            data = reader.ReadBytes(dataLength);
        var timestamp = reader.ReadInt64();
        
        return new CanFrame(identifier, dataLength, data, timestamp);
    }
}
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
    }

    public static CanFrame? ReadFrame(this BinaryReader reader)
    {
        if (reader.PeekChar() == -1)
            return null;
        
        var identifier = reader.ReadUInt32();
        var dataLength = reader.ReadByte();
        byte[]? data = null;
        if (dataLength > 0)
            data = reader.ReadBytes(dataLength);
        
        return new CanFrame(identifier, dataLength, data);
    }
}

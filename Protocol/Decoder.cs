namespace Protocol;

public static class Decoder
{
    public static void Decode(uint identifier, byte[] data)
    {
        DecodeIdentifier(identifier);
    }

    private static void DecodeIdentifier(uint identifier)
    {
        byte priority = (byte)(identifier & 0x1E000000);
        byte command = (byte)(identifier & 0x1FE0000);
        byte response = (byte)(identifier & 0x10000);
        ushort hash = (ushort)(identifier & 0xFFFF);
    }
}

public enum IdentifierPriority : byte
{
    StopGoShortCircuit = 1,
    Ack = 2,
    StopLoco = 3,
    LocoAccessoryCommand = 4
}
namespace USBtin;

public record CanFrame(uint Identifier, byte DataLength, byte[]? Data, long Timestamp);
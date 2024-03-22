namespace Protocol.Enums;

public enum SystemSubCommandType : byte
{
    SystemStop = 0x00,
    SystemGo = 0x01,
    SystemHalt = 0x02,
    LocomotiveEmergencyStop = 0x03,
    LocomotiveCycleStop = 0x04,
    LocomotiveDataProtocol = 0x05,
    AccessoriesSwitchingTime = 0x06,
    MfxFastRead = 0x07,
    EnableTrackProtocol = 0x08,
    SetMfxNewRegistrationCounter = 0x09,
    SystemOverload = 0x0A,
    SystemStatus = 0x0B,
    DeviceIdentifier = 0x0C,
    SystemReset = 0x80
}
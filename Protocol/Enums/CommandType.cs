namespace Protocol.Enums;

public enum CommandType : byte
{
    SystemCommand = 0x00,
    LocomotiveDiscovery = 0x01,
    MfxBind = 0x02,
    MfxVerify = 0x03,
    LocomotiveSpeed = 0x04,
    LocomotiveDirection = 0x05,
    LocomotiveFunction = 0x06,
    ReadConfig = 0x07,
    WriteConfig = 0x08,
    SwitchingAccessories = 0x0B,
    AccessoriesConfig = 0x0C,
    S88Polling = 0x10,
    S88Event = 0x11,
    Sx1Event = 0x12,
    SoftwareVersionRequestParticipantPing = 0x18,
    UpdateOffer = 0x19,
    ReadConfigData = 0x1A,
    BootloaderCanBound = 0x1B,
    BootloaderRailsBound = 0x1C,
    StatusDataConfiguration = 0x1D,
    RequestConfigData = 0x20,
    ConfigDataStream = 0x21,
    Connect6021DataStream = 0x22
}
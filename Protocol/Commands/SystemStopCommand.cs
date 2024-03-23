using Protocol.Enums;

namespace Protocol.Commands;

public class SystemStopCommand(Message message) : SystemCommand(message, SystemSubCommandType.SystemStop)
{
    
}
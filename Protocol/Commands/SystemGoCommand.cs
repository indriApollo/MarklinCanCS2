using Protocol.Enums;

namespace Protocol.Commands;

public class SystemGoCommand(Message message) : SystemCommand(message, SystemSubCommandType.SystemGo)
{
    
}
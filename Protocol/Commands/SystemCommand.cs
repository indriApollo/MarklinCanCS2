using Protocol.Enums;

namespace Protocol.Commands;

public class SystemCommand(Message message) : Command(message, CommandType.SystemCommand)
{
    public SystemSubCommandType SubCommand => (SystemSubCommandType)Message.CommandData[0];
}
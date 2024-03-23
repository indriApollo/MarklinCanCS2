using Protocol.Enums;

namespace Protocol.Commands;

public class SystemCommand : Command
{
    private readonly SystemSubCommandType _subCommand;

    public SystemCommand(Message message) : base(message, CommandType.SystemCommand)
    {
        _subCommand = (SystemSubCommandType)Message.CommandData[0];
    }
    
    protected SystemCommand(Message message, SystemSubCommandType subCommandType) : this(message)
    {
        if (subCommandType != _subCommand)
            throw new ArgumentException($"Invalid sub command {_subCommand}", nameof(message));
    }
    
    public override string ToString() =>
        base.ToString() + $" subcmd:{Enum.GetName(_subCommand)}";
}
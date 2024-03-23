using Protocol.Enums;

namespace Protocol.Commands;

public class Command(Message message)
{
    public readonly Message Message = message;

    protected Command(Message message, CommandType commandType) : this(message)
    {
        if (message.Command != commandType)
            throw new ArgumentException($"Invalid command {message.Command}", nameof(message));
    }

    public override string ToString() =>
        $"cmd:{Enum.GetName(Message.Command)} resp:{Message.Response} uid:{Message.Uid}";
}
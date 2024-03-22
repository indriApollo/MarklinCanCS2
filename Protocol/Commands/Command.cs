using Protocol.Enums;

namespace Protocol.Commands;

public abstract class Command
{
    public readonly Message Message;

    protected Command(Message message, CommandType commandType)
    {
        if (message.Command != commandType)
            throw new ArgumentException($"Invalid command {message.Command}", nameof(message));

        Message = message;
    }
}
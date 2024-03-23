using Protocol.Enums;

namespace Protocol.Commands;

public class LocomotiveFunctionCommand(Message message) : Command(message, CommandType.LocomotiveFunction)
{
    public byte Function => Message.CommandData[0];
    public byte? FunctionValue => Message.CommandData.Length >= 2
        ? Message.CommandData[1]
        : null;

    public override string ToString()
    {
        return base.ToString() + $" func:{Function} value:{FunctionValue}";
    }
}
using Protocol.Enums;

namespace Protocol.Commands;

public class LocomotiveDirectionCommand(Message message) : Command(message, CommandType.LocomotiveDirection)
{
    public LocomotiveDirection? Direction => Message.CommandData.Length == 1
        ? (LocomotiveDirection)Message.CommandData[0]
        : null;
    
    public override string ToString()
    {
        return base.ToString() + $" dir:{Direction}";
    }
}
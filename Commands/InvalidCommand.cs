namespace ShiftLog.Commands;

public sealed record InvalidCommand(string Message) : Command
{
    public override bool Execute(PlanningStore planningStore)
    {
        ShowInvalidMessage(Message);
        return true;
    }
}

namespace ShiftLog.Commands;

public sealed record EchoInput(string Input) : Command
{
    public override bool Execute(PlanningStore planningStore)
    {
        Console.WriteLine($"You entered: {Input}");
        return true;
    }
}

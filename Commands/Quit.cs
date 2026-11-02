namespace ShiftLog.Commands;

public sealed record Quit : Command
{
    public override bool Execute(PlanningStore planningStore) => false;
}

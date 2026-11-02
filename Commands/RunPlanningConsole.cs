namespace ShiftLog.Commands;

public sealed record RunPlanningConsole : Command
{
    public override bool Execute(PlanningStore planningStore)
    {
        new PlanningConsole().Run();
        return true;
    }
}

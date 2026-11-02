namespace ShiftLog.Commands;

public abstract record Command
{
    public abstract bool Execute(PlanningStore planningStore);

    protected static void ShowInvalidMessage(string message)
    {
        Console.WriteLine($"Invalid command: {message}");
    }
}
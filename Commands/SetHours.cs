namespace ShiftLog.Commands;

public record SetHours (CellKey Key, decimal Hours) : Command
{
    public override bool Execute(PlanningStore planningStore)
    {
        planningStore.Upsert(new PlanningEntry
        {
            EmployeeId = Key.EmployeeId,
            RecordId = Key.RecordId,
            RecordType = Key.RecordType,
            Week = Key.Week,
            Year = Key.Year,
            Minutes = Hours * 60m
        });

        Console.WriteLine($"Set {Hours:0.##} hours for {Key.EmployeeId} in week {Key.Week}/{Key.Year}.");
        return true;
    }

    public override string ToString() => $"SetHours {Key} {Hours}";
}
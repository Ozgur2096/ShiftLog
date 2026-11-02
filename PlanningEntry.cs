public sealed class PlanningEntry
{
    public required string EmployeeId {get; init;}
    public required string RecordId {get; init;}
    public required RecordType RecordType {get; init;}
    public decimal Minutes {get; set;}
    public int Week, Year;
    public string? Comment;
    public decimal Hours => Minutes / 60m;
    public CellKey CellKey => new CellKey(EmployeeId, RecordId, RecordType, Week, Year);
    
}
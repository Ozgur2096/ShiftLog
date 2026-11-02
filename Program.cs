using ShiftLog.Commands;

var employee1 = new Employee
{
    Id = "E001",
    Name = "John Doe",
    ContractHoursPerWeek = 40.0m
};

var PlanningEntry1 = new PlanningEntry
{
    EmployeeId = employee1.Id,
    RecordId = "R001",
    RecordType = RecordType.Order,
    Comment = "Worked on project X",
    Week = 12,
    Year = 2024,
    Minutes = 240
};

PlanningEntry1.Minutes = 300; // Update minutes worked

Console.WriteLine($"Employee: {employee1.Name}, Contract Hours: {employee1.ContractHoursPerWeek}, Minutes Worked: {PlanningEntry1.Minutes}, Hours Worked: {PlanningEntry1.Hours}");

var entry2 = new PlanningEntry
{
    EmployeeId = employee1.Id,
    RecordId = "R001",
    RecordType = RecordType.Order,
    Comment = "Worked on project Y",
    Week = 12,
    Year = 2024,
    Minutes = 180
};
Console.WriteLine($"Employee ID: {entry2.EmployeeId}, Record ID: {entry2.RecordId}, Record Type: {entry2.RecordType}, Minutes Worked: {entry2.Minutes}, Hours Worked: {entry2.Hours}");

Console.WriteLine($"Entries match: {PlanningEntry1.CellKey.Matches(entry2)}");
Console.WriteLine($"a == b: {PlanningEntry1 == entry2}");
Console.WriteLine($"a.Equals(b): {PlanningEntry1.Equals(entry2)}");

Console.WriteLine($"GetHashCode(a): {PlanningEntry1.GetHashCode()}");
Console.WriteLine($"GetHashCode(b): {entry2.GetHashCode()}");
Console.WriteLine($"GetHashCode(a) == GetHashCode(b): {PlanningEntry1.GetHashCode() == entry2.GetHashCode()}");

// Week list
var weeks = WeekCalculator.Range(DateOnly.FromDateTime(DateTime.Today), 5);
Console.WriteLine("Upcoming Weeks:");
foreach (var week in weeks)
{
    Console.WriteLine($"Week {week.Week}, Year {week.Year}, Thursday: {week.Thursday}, Is Current Week: {week.IsCurrentWeek}");
}













Console.BackgroundColor = ConsoleColor.DarkBlue;
Console.ForegroundColor = ConsoleColor.White;

string welcomeMessage = "Welcome to the Shift Log Application!";

var planningStore = new PlanningStore();


Console.WriteLine("Type 'run console app1' to try the planning console example, 'set hours' to update a planning cell, or 'quit' to exit.");
while (true)
{
    Console.WriteLine("Enter a command:");
    string? input = Console.ReadLine();
    var command = CommandParser.Parse(input);
    if (!command.Execute(planningStore))
    {
        break;
    }
}

// Reset console colors before exiting
Console.ResetColor();
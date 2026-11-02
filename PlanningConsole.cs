using System.Globalization;

internal sealed class PlanningConsole
{
    private readonly Dictionary<string, Employee> _employees = new(StringComparer.OrdinalIgnoreCase);
    private readonly PlanningStore _planningStore = new();

    internal void Run()
    {
        Console.WriteLine("ShiftLog");
        Console.WriteLine("Type 'help' to see available commands.");
        DrawTable();

        while (true)
        {
            Console.Write("\n> ");
            var command = Console.ReadLine();
            if (command is null || command.Trim().Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            switch (command.Trim().ToLowerInvariant())
            {
                case "help":
                    ShowHelp();
                    break;
                case "employee add":
                    AddEmployee();
                    break;
                case "record add":
                    AddPlanningRecord();
                    break;
                default:
                    Console.WriteLine("Unknown command. Type 'help' for available commands.");
                    break;
            }
        }
    }

    private void AddEmployee()
    {
        Console.WriteLine("Add employee (type 'cancel' at any prompt to stop).");
        var id = ReadRequired("Employee ID: ");
        if (id is null)
        {
            return;
        }

        if (_employees.ContainsKey(id))
        {
            Console.WriteLine($"An employee with ID '{id}' already exists.");
            return;
        }

        var name = ReadRequired("Employee name: ");
        if (name is null)
        {
            return;
        }

        var contractHours = ReadPositiveDecimal("Contract hours per week: ");
        if (contractHours is null)
        {
            return;
        }

        _employees.Add(id, new Employee
        {
            Id = id,
            Name = name,
            ContractHoursPerWeek = contractHours.Value
        });

        Console.WriteLine($"Added employee {name} ({id}).");
        DrawTable();
    }

    private void AddPlanningRecord()
    {
        if (_employees.Count == 0)
        {
            Console.WriteLine("Add an employee before adding a planning record.");
            return;
        }

        Console.WriteLine("Add planning record (type 'cancel' at any prompt to stop).");
        var employeeId = ReadRequired("Employee ID: ");
        if (employeeId is null)
        {
            return;
        }

        if (!_employees.TryGetValue(employeeId, out var employee))
        {
            Console.WriteLine($"No employee with ID '{employeeId}' was found.");
            return;
        }

        var recordType = ReadRecordType();
        if (recordType is null)
        {
            return;
        }

        var recordId = ReadRequired("Record ID: ");
        if (recordId is null)
        {
            return;
        }

        var currentWeek = WeekCalculator.WeekOf(DateOnly.FromDateTime(DateTime.Today));
        var week = ReadInteger($"ISO week number [{currentWeek.Week}]: ", 1, 53, currentWeek.Week);
        if (week is null)
        {
            return;
        }

        var year = ReadInteger($"ISO week year [{currentWeek.Year}]: ", 1, 9999, currentWeek.Year);
        if (year is null)
        {
            return;
        }

        if (week.Value > ISOWeek.GetWeeksInYear(year.Value))
        {
            Console.WriteLine($"ISO week year {year.Value} has no week {week.Value}.");
            return;
        }

        var hours = ReadPositiveDecimal("Planned hours: ");
        if (hours is null)
        {
            return;
        }

        Console.Write("Comment (optional): ");
        var comment = Console.ReadLine();
        if (comment is null)
        {
            return;
        }

        _planningStore.Upsert(new PlanningEntry
        {
            EmployeeId = employee.Id,
            RecordId = recordId,
            RecordType = recordType.Value,
            Week = week.Value,
            Year = year.Value,
            Minutes = hours.Value * 60m,
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim()
        });

        Console.WriteLine($"Saved {hours.Value:0.##} hours for {employee.Name}.");
        DrawTable();
    }

    private RecordType? ReadRecordType()
    {
        while (true)
        {
            Console.Write("Record type (Order/Contract): ");
            var input = Console.ReadLine();
            if (input is null || input.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (input.Trim().Equals(nameof(RecordType.Order), StringComparison.OrdinalIgnoreCase))
            {
                return RecordType.Order;
            }

            if (input.Trim().Equals(nameof(RecordType.Contract), StringComparison.OrdinalIgnoreCase))
            {
                return RecordType.Contract;
            }

            Console.WriteLine("Enter 'Order' or 'Contract', or type 'cancel'.");
        }
    }

    private static string? ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (input is null || input.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }

            Console.WriteLine("A value is required.");
        }
    }

    private static decimal? ReadPositiveDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (input is null || input.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (TryParseDecimal(input, out var value) && value > 0)
            {
                return value;
            }

            Console.WriteLine("Enter a number greater than zero, or type 'cancel'.");
        }
    }

    private static int? ReadInteger(string prompt, int minimum, int maximum, int defaultValue)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (input is null || input.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                return defaultValue;
            }

            if (int.TryParse(input, out var value) && value >= minimum && value <= maximum)
            {
                return value;
            }

            Console.WriteLine($"Enter a number from {minimum} to {maximum}, or type 'cancel'.");
        }
    }

    private static bool TryParseDecimal(string input, out decimal value) =>
        decimal.TryParse(input, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.CurrentCulture, out value)
        || decimal.TryParse(input, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out value);

    private void DrawTable()
    {
        var weeks = WeekCalculator.Range(DateOnly.FromDateTime(DateTime.Today), 4);
        var employees = _employees.Values.OrderBy(employee => employee.Name, StringComparer.OrdinalIgnoreCase).ToArray();
        var cells = employees.Select(employee => weeks
            .Select(week => _planningStore.GetAll
                .Where(entry => entry.EmployeeId.Equals(employee.Id, StringComparison.OrdinalIgnoreCase)
                    && entry.Week == week.Week
                    && entry.Year == week.Year)
                .Sum(entry => entry.Hours))
            .ToArray())
            .ToArray();

        var employeeWidth = Math.Max("Employee".Length, employees.Select(employee => employee.Name.Length).DefaultIfEmpty(0).Max());
        var weekWidths = weeks.Select((week, index) =>
        {
            var label = $"W{week.Week}/{week.Year}";
            var longestValue = cells.Select(row => row[index].ToString("0.##", CultureInfo.CurrentCulture).Length)
                .DefaultIfEmpty(0)
                .Max();
            return Math.Max(label.Length, longestValue);
        }).ToArray();

        var separator = $"+-{new string('-', employeeWidth)}-" +
            string.Concat(weekWidths.Select(width => $"+-{new string('-', width)}-")) + "+";

        Console.WriteLine();
        Console.WriteLine("Planning hours for the current and next 3 weeks");
        Console.WriteLine(separator);
        Console.Write($"| {"Employee".PadRight(employeeWidth)} ");
        foreach (var (week, index) in weeks.Select((week, index) => (week, index)))
        {
            Console.Write($"| {$"W{week.Week}/{week.Year}".PadLeft(weekWidths[index])} ");
        }
        Console.WriteLine("|");
        Console.WriteLine(separator);

        foreach (var (employee, employeeIndex) in employees.Select((employee, index) => (employee, index)))
        {
            Console.Write($"| {employee.Name.PadRight(employeeWidth)} ");
            for (var weekIndex = 0; weekIndex < weeks.Count; weekIndex++)
            {
                var formattedHours = cells[employeeIndex][weekIndex].ToString("0.##", CultureInfo.CurrentCulture);
                Console.Write($"| {formattedHours.PadLeft(weekWidths[weekIndex])} ");
            }
            Console.WriteLine("|");
        }

        if (employees.Length == 0)
        {
            Console.WriteLine("| No employees yet.".PadRight(separator.Length - 1) + "|");
        }

        Console.WriteLine(separator);
        Console.WriteLine("Each cell is the total planned hours for that employee and ISO week.");
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("  employee add  - add an employee");
        Console.WriteLine("  record add    - add or update a planning record");
        Console.WriteLine("  help          - show this help");
        Console.WriteLine("  quit          - exit ShiftLog");
        Console.WriteLine("When adding a record, leave the week and year blank to use the current ISO week.");
    }
}

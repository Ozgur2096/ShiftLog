using System.Globalization;

namespace ShiftLog.Commands;

public static class CommandParser
{
    public static Command Parse(string? input)
    {
        if (input is null)
        {
            return new Quit();
        }

        var trimmedInput = input.Trim();
        if (trimmedInput.Equals("quit", StringComparison.OrdinalIgnoreCase))
        {
            return new Quit();
        }

        if (trimmedInput.Equals("run console app1", StringComparison.OrdinalIgnoreCase))
        {
            return new RunPlanningConsole();
        }

        if (trimmedInput.StartsWith("set hours", StringComparison.OrdinalIgnoreCase))
        {
            return ParseSetHours(trimmedInput);
        }

        return new EchoInput(input);
    }

    private static Command ParseSetHours(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 8
            || !Enum.TryParse<RecordType>(parts[4], true, out var recordType)
            || !Enum.IsDefined(recordType)
            || !int.TryParse(parts[5], NumberStyles.None, CultureInfo.InvariantCulture, out var week)
            || !int.TryParse(parts[6], NumberStyles.None, CultureInfo.InvariantCulture, out var year)
            || !TryParseDecimal(parts[7], out var hours))
        {
            return new InvalidCommand(
                "Use: set hours <employeeId> <recordId> <Order|Contract> <week> <year> <hours>");
        }

        if (week < 1 || week > 53 || year < 1 || year > 9999
            || week > ISOWeek.GetWeeksInYear(year) || hours < 0 || hours > decimal.MaxValue / 60m)
        {
            return new InvalidCommand("Week, year, or hours is outside the valid range.");
        }

        var key = new CellKey(parts[2], parts[3], recordType, week, year);
        return new SetHours(key, hours);
    }

    private static bool TryParseDecimal(string input, out decimal value)
    {
        const NumberStyles styles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
        return decimal.TryParse(input, styles, CultureInfo.CurrentCulture, out value)
            || decimal.TryParse(input, styles, CultureInfo.InvariantCulture, out value);
    }
}

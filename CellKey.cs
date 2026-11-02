public readonly record struct CellKey (
    string EmployeeId,
    string RecordId,
    RecordType RecordType,
    int Week,
    int Year
)
    {
        public static CellKey FromEntry(PlanningEntry entry) => new CellKey(entry.EmployeeId, entry.RecordId, entry.RecordType, entry.Week, entry.Year);
        public bool Matches(PlanningEntry entry) => 
            EmployeeId == entry.EmployeeId && RecordId == entry.RecordId && RecordType == entry.RecordType && Week == entry.Week && Year == entry.Year;    
    };
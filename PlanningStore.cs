public sealed class PlanningStore
{
    private Dictionary<CellKey, PlanningEntry> _entries = new Dictionary<CellKey, PlanningEntry>();
    
    public PlanningEntry? Get (CellKey key) => _entries.TryGetValue(key, out var entry) ?  entry :  null;
    
    public IEnumerable<PlanningEntry> GetAll => _entries.Values;
    public void Upsert (PlanningEntry entry)
    {
        var key = entry.CellKey;
        if (entry.Minutes <= 0)
        {
            _entries.Remove(key);
        }
        else if (_entries.TryGetValue(key, out var existingEntry))
        {
            existingEntry.Minutes = entry.Minutes;
            existingEntry.Comment = entry.Comment;
        }
        else
        {
            _entries[key] = entry;
        }
    }
}
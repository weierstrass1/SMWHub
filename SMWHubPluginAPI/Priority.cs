namespace SMWHubPluginAPI;

public record Priority : IComparable<Priority>
{
    public readonly int DefaultPriority;
    public int CurrentPriority { get; set; }
    public Priority()
    {
        DefaultPriority = 0;
        CurrentPriority = 0;
    }
    public Priority(int defaultPriority)
    {
        DefaultPriority = defaultPriority;
        CurrentPriority = defaultPriority;
    }
    public Priority(int defaultPriority, int priority)
    {
        DefaultPriority = defaultPriority;
        CurrentPriority = priority;
    }
    public static implicit operator Priority(int p)
    {
        return new(p);
    }
    public static implicit operator int(Priority p)
    {
        return p.CurrentPriority;
    }
    public int CompareTo(Priority? other)
    {
        if(other is null)
            return 1;
        if (CurrentPriority < other.CurrentPriority)
            return -1;
        if (CurrentPriority > other.CurrentPriority)
            return 1;
        return 0;
    }
}

namespace Mt.Core;

public sealed record Device
{
    public int Index { get; init; }
    public string? Vendor { get; init; }
    public string? Product { get; init; }
}

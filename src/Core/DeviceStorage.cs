namespace Mt.Core;

public sealed record DeviceStorage
{
    public string? Description { get; init; }
    public ulong Capacity { get; init; }
    public ulong FreeSpace { get; init; }
}

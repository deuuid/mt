namespace Mt.Core;

public sealed record DeviceInfo
{
    public string? Manufacturer { get; init; }
    public string? Model { get; init; }
    public string? SerialNumber { get; init; }
    public string? Version { get; init; }
    public string? FriendlyName { get; init; }
    public IReadOnlyList<DeviceStorage> Storages { get; init; } = [];
}

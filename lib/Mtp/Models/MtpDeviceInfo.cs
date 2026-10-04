namespace OpenMtp.Models;

public sealed record MtpDeviceInfo
{
    public string? Manufacturer { get; init; }
    public string? Model { get; init; }
    public string? SerialNumber { get; init; }
    public string? Version { get; init; }
    public string? FriendlyName { get; init; }
    public IReadOnlyList<MtpStorage> Storages { get; init; } = [];
}

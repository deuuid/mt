namespace OpenMtp.Models;

public sealed record MtpDevice
{
    public string Id { get; init; } = "";
    public string? Vendor { get; init; }
    public string? Product { get; init; }
}

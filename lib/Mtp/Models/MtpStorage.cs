namespace OpenMtp.Models;

public sealed record MtpStorage
{
    public uint Id { get; init; }
    public string? Description { get; init; }
    public ulong Capacity { get; init; }
    public ulong FreeSpace { get; init; }
}

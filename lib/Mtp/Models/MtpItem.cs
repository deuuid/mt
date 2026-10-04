namespace OpenMtp.Models;

public sealed record MtpItem
{
    public uint Id { get; init; }
    public string? Name { get; init; }
    public ulong Size { get; init; }
    public bool IsFolder { get; init; }
}

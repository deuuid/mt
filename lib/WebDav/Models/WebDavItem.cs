namespace WebDav.Models;

public sealed record WebDavItem
{
    public string Name { get; init; } = "";
    public ulong Size { get; init; }
    public bool IsFolder { get; init; }
}

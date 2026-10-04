using WebDav.Models;

namespace WebDav;

public interface IWebDavStorage
{
    WebDavItem? Get(IReadOnlyList<string> path);

    IReadOnlyList<WebDavItem>? List(IReadOnlyList<string> path);

    Stream Read(IReadOnlyList<string> path);

    void Write(IReadOnlyList<string> path, Stream content, long length);

    void Delete(IReadOnlyList<string> path);

    void Copy(IReadOnlyList<string> source, IReadOnlyList<string> destination);

    void Move(IReadOnlyList<string> source, IReadOnlyList<string> destination);

    void CreateFolder(IReadOnlyList<string> path);
}

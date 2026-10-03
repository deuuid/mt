using Mt.Core;

namespace Mt.Pres.Commands;

public class VersionCommand(IAppRepository repository, OutputService output) : ICommand
{
    public int Run(string[] args)
    {
        var app = repository.Get();
        return output.Print($"{app.Name ?? "-"} {app.Version ?? "-"}");
    }
}

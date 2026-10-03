using Mt.Core;

namespace Mt.Pres.Commands;

public class ListDevicesCommand(IDeviceRepository repository, OutputService output) : ICommand
{
    public int Run(string[] args)
    {
        var devices = repository.Get();
        return output.Print(devices.Count == 0
            ? "No devices found. Connect a device."
            : string.Join(Environment.NewLine, devices.Select(d => $"{d.Index} - {d.Vendor} {d.Product}")));
    }
}

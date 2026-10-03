using Mt.Core;

namespace Mt.Pres.Commands;

public class DeviceInfoCommand(IDeviceRepository repository, OutputService output) : ICommand
{
    public int Run(string[] args)
    {
        if (args.Length < 2)
            return output.Fail("Missing device number. Usage: mt -i <n>");

        if (!int.TryParse(args[1], out var index))
            return output.Fail($"Invalid device number: {args[1]}. Usage: mt -i <n>");

        var info = repository.GetInfo(index);
        return info == null
            ? output.Fail($"Device {index} not found. Run 'mt -l' to list devices.")
            : output.Print(Format(info));
    }

    private static string Format(DeviceInfo info)
    {
        var lines = new List<string>
        {
            $"Manufacturer:  {ValueOrDash(info.Manufacturer)}",
            $"Model:         {ValueOrDash(info.Model)}",
            $"Name:          {ValueOrDash(info.FriendlyName)}",
            $"Serial number: {ValueOrDash(info.SerialNumber)}",
            $"Version:       {ValueOrDash(info.Version)}",
            "Storages:",
        };

        if (info.Storages.Count == 0)
            lines.Add("  none");

        foreach (var s in info.Storages)
            lines.Add($"  {ValueOrDash(s.Description)}: {FormatSize(s.FreeSpace)} free of {FormatSize(s.Capacity)}");

        return string.Join(Environment.NewLine, lines);
    }

    private static string ValueOrDash(string? value)
    {
        return string.IsNullOrEmpty(value) ? "-" : value;
    }

    private static string FormatSize(ulong bytes)
    {
        return $"{bytes / 1_000_000_000.0:F1} GB";
    }
}

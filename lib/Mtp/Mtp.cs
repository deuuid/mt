using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using OpenMtp.Models;

namespace OpenMtp;

public sealed class Mtp
{
    private static readonly Lock LockObject = new();
    private static bool _initialized;
    private readonly ILogger<Mtp> _logger;

    public Mtp(ILogger<Mtp> logger)
    {
        _logger = logger;

        lock (LockObject)
        {
            if (_initialized) return;
            Native.Init();
            _initialized = true;
        }
    }

    public IReadOnlyList<MtpDevice> GetDevices()
    {
        try
        {
            var devices = DetectDevices();

            var result = new List<MtpDevice>(devices.Length);
            for (var i = 0; i < devices.Length; i++)
            {
                result.Add(new MtpDevice
                {
                    Id = IdOf(devices[i]),
                    Vendor = Native.PtrToString(devices[i].DeviceEntry.Vendor),
                    Product = Native.PtrToString(devices[i].DeviceEntry.Product),
                });
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get MTP devices");
            throw;
        }
    }

    public MtpConnection? Open(string deviceId)
    {
        try
        {
            var matches = DetectDevices().Where(d => IdOf(d) == deviceId).ToArray();
            if (matches.Length == 0)
                return null;

            if (matches.Length > 1)
                throw new MtpException($"Device id {deviceId} matches {matches.Length} devices.");

            var raw = matches[0];
            raw.DeviceEntry.DeviceFlags &= ~Native.DeviceFlagForceResetOnClose;

            var device = Native.OpenRawDeviceUncached(ref raw);
            if (device == 0)
                throw new MtpException($"Could not open device {deviceId}. Another app may be using it.");

            return new MtpConnection(device);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to open MTP device {DeviceId}", deviceId);
            throw;
        }
    }

    private static RawDevice[] DetectDevices()
    {
        var err = Native.DetectRawDevices(out var list, out var count);
        try
        {
            if (err == ErrorNumber.NoDeviceAttached)
                return [];

            if (err != ErrorNumber.None)
                throw new MtpException($"Device detection failed: {err}");

            var size = Marshal.SizeOf<RawDevice>();
            var devices = new RawDevice[count];
            for (var i = 0; i < count; i++)
                devices[i] = Marshal.PtrToStructure<RawDevice>(list + i * size);

            return devices;
        }
        finally
        {
            if (list != 0)
                Native.FreeMemory(list);
        }
    }
    
    private static string IdOf(RawDevice device) => $"{device.BusLocation}-{device.DevNum}";
}
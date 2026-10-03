using Mt.Core;

namespace Mt.Data.OpenMtp;

public static unsafe class Mtp
{
    private static readonly Lock LockObject = new();
    private static bool _initialized;

    private static void EnsureInit()
    {
        lock (LockObject)
        {
            if (_initialized) return;
            Native.Init();
            _initialized = true;
        }
    }

    public static IReadOnlyList<Device> Detect()
    {
        DetectRaw(out var raw, out var count);
        try
        {
            var result = new List<Device>(count);
            for (var i = 0; i < count; i++)
            {
                if (raw == null) continue;
                
                var d = raw[i];
                var vendor = Native.PtrToString(d.DeviceEntry.Vendor);
                var product = Native.PtrToString(d.DeviceEntry.Product);
                if (string.IsNullOrEmpty(vendor) || string.IsNullOrEmpty(product))
                {
                    vendor = "Unknown device";
                    product = $"({d.DeviceEntry.VendorId:x4}:{d.DeviceEntry.ProductId:x4})";
                }

                result.Add(new Device
                {
                    Index = i,
                    Vendor = vendor,
                    Product = product,
                });
            }
            return result;
        }
        finally
        {
            if (raw != null) Native.FreeMemory(raw);
        }
    }

    public static DeviceInfo? GetDeviceInfo(int index)
    {
        DetectRaw(out var raw, out var count);
        try
        {
            if (raw == null || index < 0 || index >= count) return null;

            var savedStdout = Libc.SilenceStdout();
            nint device;
            try
            {
                device = Native.OpenRawDeviceUncached(&raw[index]);
            }
            finally
            {
                Libc.RestoreStdout(savedStdout);
            }

            if (device == 0)
                throw new MtException(
                    $"Could not open device {index}. Another app may be using it; try 'killall ptpcamerad'.");

            try
            {
                return ReadDeviceInfo(device);
            }
            finally
            {
                Native.ReleaseDevice(device);
            }
        }
        finally
        {
            if (raw != null) Native.FreeMemory(raw);
        }
    }

    private static DeviceInfo ReadDeviceInfo(nint device)
    {
        var storages = new List<DeviceStorage>();
        for (var s = (NativeStorage*)((DeviceHeader*)device)->Storage; s != null; s = (NativeStorage*)s->Next)
        {
            storages.Add(new DeviceStorage
            {
                Description = Native.PtrToString(s->StorageDescription),
                Capacity = s->MaxCapacity,
                FreeSpace = s->FreeSpaceInBytes,
            });
        }

        var info = new DeviceInfo
        {
            Manufacturer = Native.TakeString(Native.GetManufacturerName(device)),
            Model = Native.TakeString(Native.GetModelName(device)),
            SerialNumber = Native.TakeString(Native.GetSerialNumber(device)),
            Version = Native.TakeString(Native.GetDeviceVersion(device)),
            FriendlyName = Native.TakeString(Native.GetFriendlyName(device)),
            Storages = storages,
        };

        Native.ClearErrorStack(device);
        return info;
    }

    private static void DetectRaw(out RawDevice* raw, out int count)
    {
        EnsureInit();
        RawDevice* devices = null;
        var n = 0;
        ErrorNumber err;
        var savedStdout = Libc.SilenceStdout();
        try
        {
            err = Native.DetectRawDevices(&devices, &n);
        }
        finally
        {
            Libc.RestoreStdout(savedStdout);
        }

        if (err == ErrorNumber.NoDeviceAttached)
        {
            raw = null;
            count = 0;
            return;
        }

        if (err != ErrorNumber.None)
        {
            if (devices != null) Native.FreeMemory(devices);
            throw new MtException($"Device detection failed: {err}");
        }

        raw = devices;
        count = n;
    }
}

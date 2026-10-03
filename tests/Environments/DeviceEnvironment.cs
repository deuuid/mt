using Mt.Core;

namespace Mt.Tests.Environments;

public class DeviceEnvironment
{
    public List<Device> Devices { get; } = [];

    public Dictionary<int, DeviceInfo> DeviceInfos { get; } = [];

    public Exception? Error { get; set; }

    public Device Setup(Func<Device, Device>? configure = null)
    {
        var device = new Device
        {
            Index = Devices.Count,
            Vendor = "Vendor",
            Product = "Product",
        };
        device = configure?.Invoke(device) ?? device;
        Devices.Add(device);
        return device;
    }

    public void SetupInfo(Device device, Func<DeviceInfo, DeviceInfo>? configure = null)
    {
        var info = new DeviceInfo
        {
            Manufacturer = device.Vendor,
            Model = device.Product,
        };
        info = configure?.Invoke(info) ?? info;
        DeviceInfos[device.Index] = info;
    }
}

namespace Mt.Core;

public interface IDeviceRepository
{
    IReadOnlyList<Device> Get();

    DeviceInfo? GetInfo(int index);
}

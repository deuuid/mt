using Mt.Core;
using Mt.Data.OpenMtp;

namespace Mt.Data;

public class DeviceRepository : IDeviceRepository
{
    public IReadOnlyList<Device> Get() => Mtp.Detect();

    public DeviceInfo? GetInfo(int index) => Mtp.GetDeviceInfo(index);
}

using Mt.Core;
using Mt.Tests.Environments;

namespace Mt.Tests.Stubs;

public class DeviceRepositoryStub(DeviceEnvironment environment) : IDeviceRepository
{
    public IReadOnlyList<Device> Get()
    {
        if (environment.Error != null) throw environment.Error;
        return environment.Devices;
    }

    public DeviceInfo? GetInfo(int index)
    {
        if (environment.Error != null) throw environment.Error;
        return environment.DeviceInfos.GetValueOrDefault(index);
    }
}

using System.Runtime.InteropServices;
using OpenMtp.Models;

namespace OpenMtp;

public sealed class MtpConnection : IDisposable
{
    private const uint RootFolder = 0xffffffff;
    private const int FolderFileType = 0;

    private readonly Lock _lock = new();
    private nint _device;

    internal MtpConnection(nint device)
    {
        _device = device;
    }

    public MtpDeviceInfo GetDeviceInfo()
    {
        lock (_lock)
        {
            DisposeGuard();
            var info = new MtpDeviceInfo
            {
                Manufacturer = Native.TakeStringAndFree(Native.GetManufacturerName(_device)),
                Model = Native.TakeStringAndFree(Native.GetModelName(_device)),
                SerialNumber = Native.TakeStringAndFree(Native.GetSerialNumber(_device)),
                Version = Native.TakeStringAndFree(Native.GetDeviceVersion(_device)),
                FriendlyName = Native.TakeStringAndFree(Native.GetFriendlyName(_device)),
                Storages = GetDevicesStorages(_device),
            };

            Native.ClearErrorStack(_device);
            return info;
        }
    }

    public IReadOnlyList<MtpItem> GetItems(uint storageId, uint? parentItemId = null)
    {
        lock (_lock)
        {
            DisposeGuard();
            var head = Native.GetFilesAndFolders(_device, storageId, parentItemId ?? RootFolder);
            if (head == 0)
            {
                var errors = GetErrors(_device);
                if (errors.Count == 0)
                    return [];

                Native.ClearErrorStack(_device);
                throw new MtpException($"Get items failed: {string.Join("; ", errors)}");
            }

            var result = new List<MtpItem>();
            for (var address = head; address != 0;)
            {
                var file = Marshal.PtrToStructure<NativeFile>(address);
                result.Add(new MtpItem
                {
                    Id = file.ItemId,
                    Name = Native.PtrToString(file.Filename),
                    Size = file.FileSize,
                    IsFolder = file.FileType == FolderFileType,
                });

                Native.DestroyFile(address);
                address = file.Next;
            }

            return result;
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_device == 0) return;
            Native.ReleaseDevice(_device);
            _device = 0;
        }
    }

    private void DisposeGuard()
    {
        ObjectDisposedException.ThrowIf(_device == 0, this);
    }

    private static List<MtpStorage> GetDevicesStorages(nint device)
    {
        var header = Marshal.PtrToStructure<DeviceHeader>(device);

        var storages = new List<MtpStorage>();
        for (var address = header.Storage; address != 0;)
        {
            var storage = Marshal.PtrToStructure<NativeStorage>(address);
            storages.Add(new MtpStorage
            {
                Id = storage.Id,
                Description = Native.PtrToString(storage.StorageDescription),
                Capacity = storage.MaxCapacity,
                FreeSpace = storage.FreeSpaceInBytes,
            });

            address = storage.Next;
        }

        return storages;
    }

    private static List<string> GetErrors(nint device)
    {
        var errors = new List<string>();
        for (var address = Native.GetErrorStack(device); address != 0;)
        {
            var error = Marshal.PtrToStructure<ErrorEntry>(address);
            errors.Add(Native.PtrToString(error.ErrorText) ?? error.ErrorNumber.ToString());
            address = error.Next;
        }

        return errors;
    }
}

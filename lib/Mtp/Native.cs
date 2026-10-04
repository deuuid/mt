using System.Runtime.InteropServices;

namespace OpenMtp;

internal enum ErrorNumber
{
    None,
    General,
    PtpLayer,
    UsbLayer,
    MemoryAllocation,
    NoDeviceAttached,
    StorageFull,
    Connecting,
    Cancelled,
}

[StructLayout(LayoutKind.Sequential)]
internal struct DeviceEntry
{
    public nint Vendor;
    public ushort VendorId;
    public nint Product;
    public ushort ProductId;
    public uint DeviceFlags;
}

[StructLayout(LayoutKind.Sequential)]
internal struct RawDevice
{
    public DeviceEntry DeviceEntry;
    public uint BusLocation;
    public byte DevNum;
}

[StructLayout(LayoutKind.Sequential)]
internal struct DeviceHeader
{
    public byte ObjectBitsize;
    public nint Params;
    public nint UsbInfo;
    public nint Storage;
    public nint ErrorStack;
}

[StructLayout(LayoutKind.Sequential)]
internal struct NativeStorage
{
    public uint Id;
    public ushort StorageType;
    public ushort FilesystemType;
    public ushort AccessCapability;
    public ulong MaxCapacity;
    public ulong FreeSpaceInBytes;
    public ulong FreeSpaceInObjects;
    public nint StorageDescription;
    public nint VolumeIdentifier;
    public nint Next;
    public nint Prev;
}

[StructLayout(LayoutKind.Sequential)]
internal struct ErrorEntry
{
    public ErrorNumber ErrorNumber;
    public nint ErrorText;
    public nint Next;
}

[StructLayout(LayoutKind.Sequential)]
internal struct NativeFile
{
    public uint ItemId;
    public uint ParentId;
    public uint StorageId;
    public nint Filename;
    public ulong FileSize;
    public long ModificationDate;
    public int FileType;
    public nint Next;
}

internal static partial class Native
{
    private const string Lib = "mtp";

    public const uint DeviceFlagForceResetOnClose = 0x10000000;

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Init")]
    public static partial void Init();

    [LibraryImport(Lib, EntryPoint = "LIBMTP_FreeMemory")]
    public static partial void FreeMemory(nint ptr);

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Detect_Raw_Devices")]
    public static partial ErrorNumber DetectRawDevices(out nint devices, out int count);

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Open_Raw_Device_Uncached")]
    public static partial nint OpenRawDeviceUncached(ref RawDevice rawDevice);

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Release_Device")]
    public static partial void ReleaseDevice(nint device);

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Get_Manufacturername")]
    public static partial nint GetManufacturerName(nint device);

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Get_Modelname")]
    public static partial nint GetModelName(nint device);

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Get_Serialnumber")]
    public static partial nint GetSerialNumber(nint device);

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Get_Deviceversion")]
    public static partial nint GetDeviceVersion(nint device);

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Get_Friendlyname")]
    public static partial nint GetFriendlyName(nint device);

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Get_Files_And_Folders")]
    public static partial nint GetFilesAndFolders(nint device, uint storageId, uint parentId);

    [LibraryImport(Lib, EntryPoint = "LIBMTP_destroy_file_t")]
    public static partial void DestroyFile(nint file);

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Get_Errorstack")]
    public static partial nint GetErrorStack(nint device);

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Clear_Errorstack")]
    public static partial void ClearErrorStack(nint device);

    public static string? PtrToString(nint p) => Marshal.PtrToStringUTF8(p);

    public static string? TakeStringAndFree(nint p)
    {
        if (p == 0) return null;
        try { return Marshal.PtrToStringUTF8(p); }
        finally { FreeMemory(p); }
    }
}

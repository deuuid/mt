using System.Runtime.InteropServices;

namespace Mt.Data.OpenMtp;

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

internal static unsafe partial class Native
{
    private const string Lib = "mtp";

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Init")]
    public static partial void Init();

    [LibraryImport(Lib, EntryPoint = "LIBMTP_FreeMemory")]
    public static partial void FreeMemory(void* ptr);

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Detect_Raw_Devices")]
    public static partial ErrorNumber DetectRawDevices(RawDevice** devices, int* count);

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Open_Raw_Device_Uncached")]
    public static partial nint OpenRawDeviceUncached(RawDevice* rawDevice);

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

    [LibraryImport(Lib, EntryPoint = "LIBMTP_Clear_Errorstack")]
    public static partial void ClearErrorStack(nint device);

    public static string? PtrToString(nint p) => Marshal.PtrToStringUTF8(p);

    public static string? TakeString(nint p)
    {
        if (p == 0) return null;
        try { return Marshal.PtrToStringUTF8(p); }
        finally { FreeMemory((void*)p); }
    }
}

internal static partial class Libc
{
    private const string Lib = "libc";

    private const int StdoutFd = 1;
    private const int OWrOnly = 1;

    [LibraryImport(Lib, EntryPoint = "dup")]
    private static partial int Dup(int fd);

    [LibraryImport(Lib, EntryPoint = "dup2")]
    private static partial int Dup2(int fd, int fd2);

    [LibraryImport(Lib, EntryPoint = "open", StringMarshalling = StringMarshalling.Utf8)]
    private static partial int Open(string path, int flags);

    [LibraryImport(Lib, EntryPoint = "close")]
    private static partial int Close(int fd);

    [LibraryImport(Lib, EntryPoint = "fflush")]
    private static partial int Fflush(nint stream);

    public static int SilenceStdout()
    {
        Fflush(0);
        int saved = Dup(StdoutFd);
        if (saved < 0) return -1;

        int devNull = Open("/dev/null", OWrOnly);
        if (devNull < 0)
        {
            Close(saved);
            return -1;
        }

        Dup2(devNull, StdoutFd);
        Close(devNull);
        return saved;
    }

    public static void RestoreStdout(int saved)
    {
        if (saved < 0) return;
        Fflush(0);
        Dup2(saved, StdoutFd);
        Close(saved);
    }
}

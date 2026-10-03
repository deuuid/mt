# mt — Media Transfer

Command-line tool for accessing MTP devices (Android phones, e-readers, media players) over USB on macOS.
It is written in C# (.NET 10) and talks to devices through [libmtp](https://libmtp.sourceforge.net).

## Arguments

```
-h        Show this help message
-v        Show name and version
-l        List connected MTP devices
-i <n>    Show information about device <n> from the -l list
```

## Examples

```
mt        Show this help message
mt -h     Show this help message
mt -v     Show name and version, e.g.: mt 0.1.0
mt -l     List connected devices, e.g.:
          0 - Google Pixel 8
mt -i 0   Show device info
```

## Requirements

- macOS on Apple Silicon (`libmtp.dylib` in the repo is built for arm64)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Build, run, test

```sh
dotnet build
dotnet run --project src -- -l
dotnet test
```

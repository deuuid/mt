namespace Mt.Pres.Commands;

public class HelpCommand(OutputService output) : ICommand
{
    public int Run(string[] args)
    {
        return output.Print("""
                            Media Transfer - access MTP devices (Android phones, media players) over USB.

                            Usage: mt [argument]

                            Arguments:
                              -h        Show this help message
                              -v        Show name and version
                              -l        List connected MTP devices
                              -i <n>    Show information about device <n> from the -l list

                            Examples:
                              mt        Show this help message
                              mt -h     Show this help message
                              mt -v     Show name and version, e.g.: mt 0.1.0
                              mt -l     List connected devices, e.g.:
                                        0 - Google Pixel 8
                              mt -i 0   Show device info

                            Library:
                              libmtp 1.1.23 - MTP implementation used to talk to devices over USB
                              https://libmtp.sourceforge.net
                            """);
    }
}
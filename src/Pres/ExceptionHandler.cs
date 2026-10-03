using Mt.Core;

namespace Mt.Pres;

public class ExceptionHandler
{
    public string Handle(Exception exception)
    {
        if (Environment.GetEnvironmentVariable("MT_DEBUG") == "1")
            return exception.ToString();

        return exception switch
        {
            MtException e => e.Message,
            DllNotFoundException e when e.Message.Contains("'mtp'") =>
                "libmtp.dylib could not be loaded. mt requires macOS on Apple Silicon.",
            _ => $"Unexpected error: {exception.Message}",
        };
    }
}

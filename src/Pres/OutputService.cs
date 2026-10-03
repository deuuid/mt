namespace Mt.Pres;

public class OutputService
{
    public int Print(string text)
    {
        Console.WriteLine(text);
        return 0;
    }

    public int Fail(string text)
    {
        Console.Error.WriteLine(text);
        return 1;
    }
}

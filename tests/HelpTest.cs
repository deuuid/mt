using Mt.Tests.Base;

namespace Mt.Tests;

public class HelpTest : TestBase
{
    [Test]
    public void Print([Values] bool byArgument)
    {
        var (exitCode, output) = byArgument ? Run("-h") : Run();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exitCode, Is.EqualTo(0));
            Assert.That(output, Does.StartWith("Media Transfer"));
        }
    }
}

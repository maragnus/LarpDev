namespace Larp.Data.Tests;

[TestClass]
public class EmailNormalizationTests
{
    [TestMethod]
    public void NonGmail_Works()
    {
        Assert.AreEqual("test@sample.com", AccountEmail.NormalizeEmail("test@sample.com"));
        Assert.AreEqual("test.test@sample.com", AccountEmail.NormalizeEmail("test.test@sample.com"));
        Assert.AreEqual("test.test.test@sample.com", AccountEmail.NormalizeEmail("test.test.test@sample.com"));
        Assert.AreEqual("test.test+test@sample.com", AccountEmail.NormalizeEmail("test.test+test@sample.com"));
    }

    [TestMethod]
    public void Gmail_Works()
    {
        Assert.AreEqual("test@gmail.com", AccountEmail.NormalizeEmail("test@gmail.com"));
        Assert.AreEqual("testtest@gmail.com", AccountEmail.NormalizeEmail("test.test@gmail.com"));
        Assert.AreEqual("testtesttest@gmail.com", AccountEmail.NormalizeEmail("test.test.test@gmail.com"));
        Assert.AreEqual("testtest+test@gmail.com", AccountEmail.NormalizeEmail("test.test+test@gmail.com"));
    }
}
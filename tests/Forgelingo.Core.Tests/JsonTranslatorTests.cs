using Forgelingo.Core;
using Xunit;

public class JsonTranslatorTests
{
    [Fact]
    public void ExtractsAndApplies()
    {
        var json = "{" +
            "\"pages\":[\"Page one\", \"Page two\"],\"title\":\"Guide\"}";
        var extracted = JsonTranslator.ExtractTexts(json);
        Assert.Contains("title", extracted.Keys); // title.path
        Assert.Contains("pages[0]", extracted.Keys);

        var translated = new System.Collections.Generic.Dictionary<string,string>
        {
            {"title","Guia"},
            {"pages[0]","Página um"},
            {"pages[1]","Página dois"}
        };
        var applied = JsonTranslator.ApplyTranslations(json, translated);
        Assert.Contains("Guia", applied);
        Assert.Contains("Página um", applied);
    }
}

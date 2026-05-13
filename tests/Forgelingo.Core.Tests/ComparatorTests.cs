using Forgelingo.Core;
using Xunit;

public class ComparatorTests
{
    [Fact]
    public void DetectsMissingPlaceholder()
    {
        var orig = "You got %d XP";
        var trans = "Você recebeu XP";
        var (need, reason) = TranslationComparator.NeedsCorrection(orig, trans);
        Assert.True(need);
        Assert.Contains("faltam", reason);
    }

    [Fact]
    public void AcceptsCorrectTranslation()
    {
        var orig = "You got %d XP";
        var trans = "Você ganhou %d XP";
        var (need, _) = TranslationComparator.NeedsCorrection(orig, trans);
        Assert.False(need);
    }
}

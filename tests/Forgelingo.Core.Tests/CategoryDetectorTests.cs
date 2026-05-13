using Forgelingo.Core;
using Xunit;

public class CategoryDetectorTests
{
    [Fact]
    public void DetectsMagic()
    {
        var (cat, conf) = CategoryDetector.DetectCategoryFromContent("Ritual of mana and arcane essence to summon");
        Assert.Equal("magic", cat);
        Assert.True(conf > 0.5);
    }

    [Fact]
    public void DetectsTech()
    {
        var (cat, conf) = CategoryDetector.DetectCategoryFromContent("Generator produces 100 RF/tick. Tier 3 multiblock.");
        Assert.Equal("tech", cat);
        Assert.True(conf > 0.5);
    }
}
